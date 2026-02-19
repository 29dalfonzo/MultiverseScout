using Microsoft.Extensions.Options;
using MultiverseScout.Api.Data;
using MultiverseScout.Api.Grains;
using MultiverseScout.Api.HostedServices;
using MultiverseScout.Api.Models;
using MultiverseScout.Api.Repositories;
using MultiverseScout.Api.Services;
using MultiverseScout.Contracts;
using Orleans;
using Orleans.Hosting;
using RethinkDb.Driver.Net;
using static RethinkDb.Driver.RethinkDB;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RethinkDbOptions>(builder.Configuration.GetSection(RethinkDbOptions.SectionName));

builder.Services.AddSingleton<IConnection>(sp =>
{
    var opt = sp.GetRequiredService<IOptions<RethinkDbOptions>>().Value;
    return R.Connection().Hostname(opt.Host).Port(opt.Port).Timeout(opt.Timeout).Connect();
});

builder.Services.AddSingleton<IPersonajesRepository>(sp =>
{
    var conn = sp.GetRequiredService<IConnection>();
    var db = sp.GetRequiredService<IOptions<RethinkDbOptions>>().Value.Database;
    return new PersonajesRepository(conn, db);
});
builder.Services.AddSingleton<IBatallasRepository>(sp =>
{
    var conn = sp.GetRequiredService<IConnection>();
    var db = sp.GetRequiredService<IOptions<RethinkDbOptions>>().Value.Database;
    return new BatallasRepository(conn, db);
});

builder.Services.AddHostedService<RethinkDbStartupService>();
builder.Services.AddSingleton<IGuionService, GuionService>();

builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
});

var allowedOrigins = (builder.Configuration["Cors:AllowedOrigins"] ?? "https://localhost:5001;http://localhost:5000")
    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Where(o => !string.IsNullOrWhiteSpace(o))
    .ToArray();
if (allowedOrigins.Length == 0)
    allowedOrigins = new[] { "http://localhost:5000", "https://localhost:5001" };

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
    app.MapGet("/", () => Results.Ok("Multiverse Scout API - Orleans + RethinkDB"));

// Fase 2: Minimal APIs que llaman a los grains
const string PersonajesGrainKey = "personajes";
const string BatallasFactoryGrainKey = "factory";

app.MapGet("/api/personajes", async (IGrainFactory grainFactory, string? universo) =>
{
    Universo? u = null;
    if (string.Equals(universo, "Marvel", StringComparison.OrdinalIgnoreCase)) u = Universo.Marvel;
    else if (string.Equals(universo, "DC", StringComparison.OrdinalIgnoreCase)) u = Universo.DC;
    var grain = grainFactory.GetGrain<IPersonajesGrain>(PersonajesGrainKey);
    var list = await grain.ListarPersonajesAsync(u);
    return Results.Ok(list);
});

app.MapGet("/api/personajes/{id}", async (IGrainFactory grainFactory, string id) =>
{
    if (string.IsNullOrWhiteSpace(id)) return Results.BadRequest();
    var grain = grainFactory.GetGrain<IPersonajesGrain>(PersonajesGrainKey);
    var personaje = await grain.GetPersonajeAsync(id);
    return personaje is null ? Results.NotFound() : Results.Ok(personaje);
});

app.MapPost("/api/batallas", async (IGrainFactory grainFactory, CrearBatallaRequest body, ILogger<Program> logger) =>
{
    if (string.IsNullOrWhiteSpace(body.PersonajeAId) || string.IsNullOrWhiteSpace(body.PersonajeBId))
        return Results.BadRequest("PersonajeAId y PersonajeBId son obligatorios.");
    try
    {
        var grain = grainFactory.GetGrain<IBatallasFactoryGrain>(BatallasFactoryGrainKey);
        var batalla = await grain.CrearBatallaAsync(body.PersonajeAId, body.PersonajeBId);
        return Results.Created($"/api/batallas/{batalla.Id}", batalla);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al crear batalla");
        return Results.Problem(
            detail: app.Environment.IsDevelopment() ? ex.ToString() : "Error al crear la batalla.",
            statusCode: 500);
    }
});

app.MapGet("/api/batallas/{id}", async (IGrainFactory grainFactory, string id, ILogger<Program> logger) =>
{
    if (string.IsNullOrWhiteSpace(id)) return Results.BadRequest();
    try
    {
        var grain = grainFactory.GetGrain<IBatallaGrain>(id);
        var batalla = await grain.GetBatallaAsync();
        var resultados = await grain.GetResultadosBatallaAsync();
        if (batalla is null) return Results.NotFound();
        return Results.Ok(new BatallaDetalleResponse(batalla, resultados));
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al obtener batalla {BatallaId}", id);
        return Results.Problem(
            detail: app.Environment.IsDevelopment() ? ex.ToString() : "Error al cargar la batalla.",
            statusCode: 500);
    }
});

app.MapPost("/api/batallas/{id}/voto", async (IGrainFactory grainFactory, string id, RegistrarVotoRequest body, HttpContext ctx) =>
{
    if (string.IsNullOrWhiteSpace(id)) return Results.BadRequest();
    if (string.IsNullOrWhiteSpace(body.PersonajeId))
        return Results.BadRequest("PersonajeId es obligatorio.");
    var usuarioSesion = body.UsuarioSesion ?? ctx.Request.Headers["X-Session-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString("N");
    var grain = grainFactory.GetGrain<IBatallaGrain>(id);
    var ok = await grain.RegistrarVotoAsync(body.PersonajeId, usuarioSesion);
    if (!ok) return Results.Conflict("Ya has votado en esta batalla o la batalla/personaje no es válido.");
    return Results.Ok(new { registrado = true });
});

// Fase 5: guion narrativo "Encuentro en el Multiverso"
app.MapGet("/api/batallas/{id}/guion", async (IGrainFactory grainFactory, IGuionService guionService, string id) =>
{
    if (string.IsNullOrWhiteSpace(id)) return Results.BadRequest();
    var batallaGrain = grainFactory.GetGrain<IBatallaGrain>(id);
    var batalla = await batallaGrain.GetBatallaAsync();
    if (batalla is null) return Results.NotFound();
    var personajesGrain = grainFactory.GetGrain<IPersonajesGrain>(PersonajesGrainKey);
    var personajeA = await personajesGrain.GetPersonajeAsync(batalla.PersonajeAId);
    var personajeB = await personajesGrain.GetPersonajeAsync(batalla.PersonajeBId);
    if (personajeA is null || personajeB is null) return Results.NotFound();
    var guion = guionService.Generar(batalla, personajeA, personajeB);
    return Results.Ok(new GuionResponse(guion));
});

app.Run();

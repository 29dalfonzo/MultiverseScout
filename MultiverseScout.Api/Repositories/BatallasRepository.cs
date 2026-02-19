using MultiverseScout.Api.Data;
using MultiverseScout.Contracts;
using RethinkDb.Driver.Net;
using static RethinkDb.Driver.RethinkDB;

namespace MultiverseScout.Api.Repositories;

public sealed class BatallasRepository : IBatallasRepository
{
    private readonly IConnection _conn;
    private readonly string _db;

    public BatallasRepository(IConnection conn, string databaseName)
    {
        _conn = conn;
        _db = databaseName;
    }

    public async Task<BatallaDto> CrearBatallaAsync(string personajeAId, string personajeBId, CancellationToken ct = default)
    {
        var id = Guid.NewGuid().ToString("N");
        var batalla = new BatallaDto
        {
            Id = id,
            PersonajeAId = personajeAId,
            PersonajeBId = personajeBId,
            Fecha = DateTime.UtcNow,
            VotosA = 0,
            VotosB = 0
        };
        var doc = ToBatallaDoc(batalla);
        await R.Db(_db).Table(RethinkDbSetup.TableBatallas).Insert(doc).RunWriteAsync(_conn, ct);
        return batalla;
    }

    public async Task<BatallaDto?> GetBatallaAsync(string batallaId, CancellationToken ct = default)
    {
        var doc = await R.Db(_db).Table(RethinkDbSetup.TableBatallas).Get(batallaId).RunResultAsync<BatallaDoc?>(_conn, ct);
        return doc is null ? null : ToBatallaDto(doc);
    }

    public async Task<ResultadosBatallaDto> GetResultadosBatallaAsync(string batallaId, CancellationToken ct = default)
    {
        var votos = await R.Db(_db).Table(RethinkDbSetup.TableVotos)
            .GetAll(batallaId).OptArg("index", "batallaId")
            .RunResultAsync<List<VotoDoc>>(_conn, ct);
        var lista = votos ?? new List<VotoDoc>();
        var votosA = 0;
        var votosB = 0;
        var batalla = await GetBatallaAsync(batallaId, ct);
        if (batalla is null)
            return new ResultadosBatallaDto { BatallaId = batallaId, VotosPersonajeA = 0, VotosPersonajeB = 0 };
        foreach (var v in lista)
        {
            var pid = v.personajeId ?? "";
            if (pid == batalla.PersonajeAId) votosA++;
            else if (pid == batalla.PersonajeBId) votosB++;
        }
        return new ResultadosBatallaDto { BatallaId = batallaId, VotosPersonajeA = votosA, VotosPersonajeB = votosB };
    }

    public async Task<bool> RegistrarVotoAsync(string batallaId, string personajeId, string usuarioSesion, CancellationToken ct = default)
    {
        var batalla = await GetBatallaAsync(batallaId, ct);
        if (batalla is null) return false;
        if (personajeId != batalla.PersonajeAId && personajeId != batalla.PersonajeBId) return false;

        var votosUsuario = await R.Db(_db).Table(RethinkDbSetup.TableVotos)
            .GetAll(batallaId).OptArg("index", "batallaId")
            .RunResultAsync<List<VotoDoc>>(_conn, ct);
        var yaVoto = (votosUsuario ?? new List<VotoDoc>()).Any(v => string.Equals(v.usuarioSesion, usuarioSesion, StringComparison.Ordinal));
        if (yaVoto) return false;

        var id = Guid.NewGuid().ToString("N");
        var doc = new { id, batallaId, personajeId, usuarioSesion };
        await R.Db(_db).Table(RethinkDbSetup.TableVotos).Insert(doc).RunWriteAsync(_conn, ct);
        return true;
    }

    private static object ToBatallaDoc(BatallaDto b) => new
    {
        id = b.Id,
        personajeAId = b.PersonajeAId,
        personajeBId = b.PersonajeBId,
        fecha = b.Fecha.ToString("o"),
        votosA = b.VotosA,
        votosB = b.VotosB
    };

    private static BatallaDto ToBatallaDto(BatallaDoc d)
    {
        var fecha = ParseFecha(d.fecha);
        return new BatallaDto
        {
            Id = d.id ?? "",
            PersonajeAId = d.personajeAId ?? "",
            PersonajeBId = d.personajeBId ?? "",
            Fecha = fecha,
            VotosA = d.votosA,
            VotosB = d.votosB
        };
    }

    private static DateTime ParseFecha(object? value)
    {
        if (value is string s && !string.IsNullOrEmpty(s) &&
            DateTime.TryParse(s, null, System.Globalization.DateTimeStyles.RoundtripKind, out var parsed))
            return parsed;
        if (value is DateTime dt) return dt;
        // RethinkDB TIME puede venir como objeto con epoch_time (segundos desde epoch)
        if (value is IDictionary<string, object> dict && dict.TryGetValue("epoch_time", out var epochObj))
        {
            if (epochObj is double d) return DateTimeOffset.FromUnixTimeSeconds((long)d).UtcDateTime;
            if (epochObj is long l) return DateTimeOffset.FromUnixTimeSeconds(l).UtcDateTime;
        }
        if (value is IConvertible conv)
            try { return conv.ToDateTime(System.Globalization.CultureInfo.InvariantCulture); } catch { }
        return DateTime.UtcNow;
    }

    private sealed class BatallaDoc
    {
        public string? id { get; set; }
        public string? personajeAId { get; set; }
        public string? personajeBId { get; set; }
        public object? fecha { get; set; }
        public int votosA { get; set; }
        public int votosB { get; set; }
    }

    private sealed class VotoDoc
    {
        public string? id { get; set; }
        public string? batallaId { get; set; }
        public string? personajeId { get; set; }
        public string? usuarioSesion { get; set; }
    }
}

using Orleans;

namespace MultiverseScout.Contracts;

/// <summary>DTO de personaje (Marvel/DC).</summary>
[GenerateSerializer]
public sealed record PersonajeDto
{
    [Id(0)] public string Id { get; init; } = string.Empty;
    [Id(1)] public string Nombre { get; init; } = string.Empty;
    [Id(2)] public Universo Universo { get; init; }
    [Id(3)] public AtributosPersonaje Atributos { get; init; } = new();
}

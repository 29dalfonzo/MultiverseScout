using Orleans;

namespace MultiverseScout.Contracts;

/// <summary>DTO de batalla entre dos personajes.</summary>
[GenerateSerializer]
public sealed record BatallaDto
{
    [Id(0)] public string Id { get; init; } = string.Empty;
    [Id(1)] public string PersonajeAId { get; init; } = string.Empty;
    [Id(2)] public string PersonajeBId { get; init; } = string.Empty;
    [Id(3)] public DateTime Fecha { get; init; }
    [Id(4)] public int VotosA { get; init; }
    [Id(5)] public int VotosB { get; init; }
}

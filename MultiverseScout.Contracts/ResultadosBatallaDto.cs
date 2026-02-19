using Orleans;

namespace MultiverseScout.Contracts;

/// <summary>Resultados de votación de una batalla.</summary>
[GenerateSerializer]
public sealed record ResultadosBatallaDto
{
    [Id(0)] public string BatallaId { get; init; } = string.Empty;
    [Id(1)] public int VotosPersonajeA { get; init; }
    [Id(2)] public int VotosPersonajeB { get; init; }
}

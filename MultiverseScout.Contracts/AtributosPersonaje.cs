using Orleans;

namespace MultiverseScout.Contracts;

/// <summary>Atributos de combate del personaje (stats).</summary>
[GenerateSerializer]
public sealed record AtributosPersonaje
{
    [Id(0)] public int Fuerza { get; init; }
    [Id(1)] public int Velocidad { get; init; }
    [Id(2)] public int Inteligencia { get; init; }
    [Id(3)] public int Resistencia { get; init; }
    [Id(4)] public int PoderEspecial { get; init; }
}

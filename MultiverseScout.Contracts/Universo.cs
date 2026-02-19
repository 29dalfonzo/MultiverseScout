using Orleans;

namespace MultiverseScout.Contracts;

/// <summary>Universo del personaje: Marvel o DC.</summary>
[GenerateSerializer]
public enum Universo
{
    Marvel = 0,
    DC = 1
}

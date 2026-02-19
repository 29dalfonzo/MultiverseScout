using MultiverseScout.Contracts;
using Orleans;

namespace MultiverseScout.Api.Grains;

/// <summary>Grain para crear batallas (singleton, clave fija).</summary>
public interface IBatallasFactoryGrain : IGrainWithStringKey
{
    Task<BatallaDto> CrearBatallaAsync(string personajeAId, string personajeBId);
}

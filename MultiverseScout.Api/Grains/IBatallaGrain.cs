using MultiverseScout.Contracts;
using Orleans;

namespace MultiverseScout.Api.Grains;

/// <summary>Grain por batalla (clave = batallaId).</summary>
public interface IBatallaGrain : IGrainWithStringKey
{
    Task<ResultadosBatallaDto> GetResultadosBatallaAsync();
    Task<bool> RegistrarVotoAsync(string personajeId, string usuarioSesion);
    Task<BatallaDto?> GetBatallaAsync();
}

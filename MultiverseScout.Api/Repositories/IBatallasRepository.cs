using MultiverseScout.Contracts;

namespace MultiverseScout.Api.Repositories;

public interface IBatallasRepository
{
    Task<BatallaDto> CrearBatallaAsync(string personajeAId, string personajeBId, CancellationToken ct = default);
    Task<ResultadosBatallaDto> GetResultadosBatallaAsync(string batallaId, CancellationToken ct = default);
    Task<bool> RegistrarVotoAsync(string batallaId, string personajeId, string usuarioSesion, CancellationToken ct = default);
    Task<BatallaDto?> GetBatallaAsync(string batallaId, CancellationToken ct = default);
}

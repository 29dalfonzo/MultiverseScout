using MultiverseScout.Api.Repositories;
using MultiverseScout.Contracts;
using Orleans;

namespace MultiverseScout.Api.Grains;

public sealed class BatallaGrain : Grain, IBatallaGrain
{
    private readonly IBatallasRepository _repo;

    public BatallaGrain(IBatallasRepository repo)
    {
        _repo = repo;
    }

    private string BatallaId => this.GetPrimaryKeyString();

    public Task<ResultadosBatallaDto> GetResultadosBatallaAsync() => _repo.GetResultadosBatallaAsync(BatallaId);

    public Task<bool> RegistrarVotoAsync(string personajeId, string usuarioSesion) =>
        _repo.RegistrarVotoAsync(BatallaId, personajeId, usuarioSesion);

    public Task<BatallaDto?> GetBatallaAsync() => _repo.GetBatallaAsync(BatallaId);
}

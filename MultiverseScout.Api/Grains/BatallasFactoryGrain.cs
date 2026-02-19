using MultiverseScout.Api.Repositories;
using MultiverseScout.Contracts;
using Orleans;

namespace MultiverseScout.Api.Grains;

public sealed class BatallasFactoryGrain : Grain, IBatallasFactoryGrain
{
    private readonly IBatallasRepository _repo;

    public BatallasFactoryGrain(IBatallasRepository repo)
    {
        _repo = repo;
    }

    public Task<BatallaDto> CrearBatallaAsync(string personajeAId, string personajeBId) =>
        _repo.CrearBatallaAsync(personajeAId, personajeBId);
}

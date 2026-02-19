using MultiverseScout.Api.Repositories;
using MultiverseScout.Contracts;
using Orleans;

namespace MultiverseScout.Api.Grains;

public sealed class PersonajesGrain : Grain, IPersonajesGrain
{
    private readonly IPersonajesRepository _repo;

    public PersonajesGrain(IPersonajesRepository repo)
    {
        _repo = repo;
    }

    public Task<PersonajeDto?> GetPersonajeAsync(string id) => _repo.GetPersonajeAsync(id);

    public Task<IReadOnlyList<PersonajeDto>> ListarPersonajesAsync(Universo? universo) => _repo.ListarPersonajesAsync(universo);
}

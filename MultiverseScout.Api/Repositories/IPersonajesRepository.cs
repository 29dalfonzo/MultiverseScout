using MultiverseScout.Contracts;

namespace MultiverseScout.Api.Repositories;

public interface IPersonajesRepository
{
    Task<PersonajeDto?> GetPersonajeAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<PersonajeDto>> ListarPersonajesAsync(Universo? universo, CancellationToken ct = default);
}

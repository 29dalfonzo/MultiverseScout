using MultiverseScout.Contracts;
using Orleans;

namespace MultiverseScout.Api.Grains;

/// <summary>Grain para consultas de personajes (singleton, clave fija).</summary>
public interface IPersonajesGrain : IGrainWithStringKey
{
    Task<PersonajeDto?> GetPersonajeAsync(string id);
    Task<IReadOnlyList<PersonajeDto>> ListarPersonajesAsync(Universo? universo);
}

using MultiverseScout.Contracts;

namespace MultiverseScout.Api.Services;

public interface IGuionService
{
    string Generar(BatallaDto batalla, PersonajeDto personajeA, PersonajeDto personajeB);
}

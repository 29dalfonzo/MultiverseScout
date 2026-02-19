using MultiverseScout.Contracts;

namespace MultiverseScout.Api.Models;

public sealed record BatallaDetalleResponse(BatallaDto? Batalla, ResultadosBatallaDto Resultados);

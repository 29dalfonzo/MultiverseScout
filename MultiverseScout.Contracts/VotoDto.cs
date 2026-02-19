namespace MultiverseScout.Contracts;

/// <summary>DTO de voto en una batalla (usuario/sesión vota por un personaje).</summary>
public sealed record VotoDto
{
    public string Id { get; init; } = string.Empty;
    public string BatallaId { get; init; } = string.Empty;
    public string PersonajeId { get; init; } = string.Empty;
    /// <summary>Identificador de usuario o sesión (evitar doble voto por batalla).</summary>
    public string UsuarioSesion { get; init; } = string.Empty;
}

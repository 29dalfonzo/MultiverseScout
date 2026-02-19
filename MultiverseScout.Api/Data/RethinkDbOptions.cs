namespace MultiverseScout.Api.Data;

/// <summary>Opciones de conexión a RethinkDB (bind desde appsettings).</summary>
public sealed class RethinkDbOptions
{
    public const string SectionName = "RethinkDB";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 28015;
    public int Timeout { get; set; } = 30;
    public string Database { get; set; } = "multiverse";
}

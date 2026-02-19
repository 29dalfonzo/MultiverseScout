using RethinkDb.Driver.Net;
using static RethinkDb.Driver.RethinkDB;

namespace MultiverseScout.Api.Data;

/// <summary>Configura tablas e índices en RethinkDB al arranque.</summary>
public static class RethinkDbSetup
{
    public const string TablePersonajes = "personajes";
    public const string TableBatallas = "batallas";
    public const string TableVotos = "votos";

    public static async Task EnsureDatabaseAsync(IConnection conn, string dbName, CancellationToken ct = default)
    {
        var dbList = await R.DbList().RunResultAsync<List<string>>(conn, ct);
        if (dbList.Contains(dbName))
            return;
        await R.DbCreate(dbName).RunWriteAsync(conn, ct);
    }

    public static async Task EnsureTableAsync(IConnection conn, string dbName, string tableName, CancellationToken ct = default)
    {
        var tableList = await R.Db(dbName).TableList().RunResultAsync<List<string>>(conn, ct);
        if (tableList.Contains(tableName))
            return;
        await R.Db(dbName).TableCreate(tableName).RunWriteAsync(conn, ct);
    }

    public static async Task EnsureIndexAsync(IConnection conn, string dbName, string tableName, string indexName, CancellationToken ct = default)
    {
        var indexList = await R.Db(dbName).Table(tableName).IndexList().RunResultAsync<List<string>>(conn, ct);
        if (indexList.Contains(indexName))
            return;
        await R.Db(dbName).Table(tableName).IndexCreate(indexName).RunWriteAsync(conn, ct);
    }

    public static async Task EnsureAllAsync(IConnection conn, string dbName, CancellationToken ct = default)
    {
        await EnsureDatabaseAsync(conn, dbName, ct).ConfigureAwait(false);
        await EnsureTableAsync(conn, dbName, TablePersonajes, ct).ConfigureAwait(false);
        await EnsureTableAsync(conn, dbName, TableBatallas, ct).ConfigureAwait(false);
        await EnsureTableAsync(conn, dbName, TableVotos, ct).ConfigureAwait(false);

        await EnsureIndexAsync(conn, dbName, TablePersonajes, "universo", ct).ConfigureAwait(false);
        await EnsureIndexAsync(conn, dbName, TableBatallas, "fecha", ct).ConfigureAwait(false);
        await EnsureIndexAsync(conn, dbName, TableVotos, "batallaId", ct).ConfigureAwait(false);
    }
}

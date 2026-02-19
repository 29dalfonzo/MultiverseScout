using RethinkDb.Driver.Net;
using static RethinkDb.Driver.RethinkDB;

const string DbName = "multiverse";
const string TablePersonajes = "personajes";

var host = Environment.GetEnvironmentVariable("RETHINKDB_HOST") ?? "localhost";
var port = int.TryParse(Environment.GetEnvironmentVariable("RETHINKDB_PORT"), out var p) ? p : 28015;

Console.WriteLine($"Conectando a RethinkDB en {host}:{port}...");
var conn = R.Connection().Hostname(host).Port(port).Timeout(30).Connect();
Console.WriteLine("Conectado.");

await EnsureDatabaseAndTableAsync(conn);
await InsertPersonajesAsync(conn);
Console.WriteLine("Listo. 10 personajes insertados (5 Marvel, 5 DC).");
return 0;

async Task EnsureDatabaseAndTableAsync(IConnection c)
{
    var dbList = await R.DbList().RunResultAsync<List<string>>(c);
    if (!dbList.Contains(DbName))
    {
        await R.DbCreate(DbName).RunWriteAsync(c);
        Console.WriteLine($"Base de datos '{DbName}' creada.");
    }
    var tableList = await R.Db(DbName).TableList().RunResultAsync<List<string>>(c);
    if (!tableList.Contains(TablePersonajes))
    {
        await R.Db(DbName).TableCreate(TablePersonajes).RunWriteAsync(c);
        await R.Db(DbName).Table(TablePersonajes).IndexCreate("universo").RunWriteAsync(c);
        Console.WriteLine($"Tabla '{TablePersonajes}' e índice 'universo' creados.");
    }
}

async Task InsertPersonajesAsync(IConnection c)
{
    var personajes = new[]
    {
        // Marvel (universo = 0)
        new { id = "marvel-ironman", nombre = "Iron Man", universo = 0, atributos = new { fuerza = 70, velocidad = 60, inteligencia = 95, resistencia = 75, poderEspecial = 90 } },
        new { id = "marvel-capitan", nombre = "Capitán América", universo = 0, atributos = new { fuerza = 75, velocidad = 65, inteligencia = 70, resistencia = 80, poderEspecial = 75 } },
        new { id = "marvel-thor", nombre = "Thor", universo = 0, atributos = new { fuerza = 95, velocidad = 70, inteligencia = 65, resistencia = 90, poderEspecial = 95 } },
        new { id = "marvel-hulk", nombre = "Hulk", universo = 0, atributos = new { fuerza = 100, velocidad = 55, inteligencia = 70, resistencia = 95, poderEspecial = 90 } },
        new { id = "marvel-spiderman", nombre = "Spider-Man", universo = 0, atributos = new { fuerza = 65, velocidad = 85, inteligencia = 80, resistencia = 70, poderEspecial = 75 } },
        // DC (universo = 1)
        new { id = "dc-superman", nombre = "Superman", universo = 1, atributos = new { fuerza = 98, velocidad = 95, inteligencia = 85, resistencia = 95, poderEspecial = 98 } },
        new { id = "dc-batman", nombre = "Batman", universo = 1, atributos = new { fuerza = 70, velocidad = 75, inteligencia = 95, resistencia = 72, poderEspecial = 70 } },
        new { id = "dc-wonderwoman", nombre = "Wonder Woman", universo = 1, atributos = new { fuerza = 90, velocidad = 88, inteligencia = 82, resistencia = 88, poderEspecial = 90 } },
        new { id = "dc-flash", nombre = "Flash", universo = 1, atributos = new { fuerza = 60, velocidad = 100, inteligencia = 78, resistencia = 65, poderEspecial = 85 } },
        new { id = "dc-aquaman", nombre = "Aquaman", universo = 1, atributos = new { fuerza = 85, velocidad = 70, inteligencia = 72, resistencia = 85, poderEspecial = 80 } },
    };

    var table = R.Db(DbName).Table(TablePersonajes);
    foreach (var p in personajes)
    {
        try
        {
            await table.Insert(p).RunWriteAsync(c);
            Console.WriteLine($"  Insertado: {p.nombre} ({(p.universo == 0 ? "Marvel" : "DC")})");
        }
        catch (Exception ex) when (ex.Message.Contains("Duplicate", StringComparison.OrdinalIgnoreCase) || ex.Message.Contains("primary key", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"  Ya existe: {p.nombre}, omitido.");
        }
    }
}

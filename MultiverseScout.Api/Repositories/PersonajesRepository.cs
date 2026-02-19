using MultiverseScout.Api.Data;
using MultiverseScout.Contracts;
using RethinkDb.Driver.Net;
using static RethinkDb.Driver.RethinkDB;

namespace MultiverseScout.Api.Repositories;

public sealed class PersonajesRepository : IPersonajesRepository
{
    private readonly IConnection _conn;
    private readonly string _db;

    public PersonajesRepository(IConnection conn, string databaseName)
    {
        _conn = conn;
        _db = databaseName;
    }

    public async Task<PersonajeDto?> GetPersonajeAsync(string id, CancellationToken ct = default)
    {
        var doc = await R.Db(_db).Table(RethinkDbSetup.TablePersonajes).Get(id).RunResultAsync<PersonajeDoc?>(_conn, ct);
        return doc is null ? null : ToPersonajeDto(doc);
    }

    public async Task<IReadOnlyList<PersonajeDto>> ListarPersonajesAsync(Universo? universo, CancellationToken ct = default)
    {
        List<PersonajeDoc> docs;
        if (universo.HasValue)
            docs = await R.Db(_db).Table(RethinkDbSetup.TablePersonajes)
                .GetAll((int)universo.Value).OptArg("index", "universo")
                .RunResultAsync<List<PersonajeDoc>>(_conn, ct) ?? new List<PersonajeDoc>();
        else
            docs = await R.Db(_db).Table(RethinkDbSetup.TablePersonajes)
                .RunResultAsync<List<PersonajeDoc>>(_conn, ct) ?? new List<PersonajeDoc>();
        return docs.ConvertAll(ToPersonajeDto);
    }

    private static PersonajeDto ToPersonajeDto(PersonajeDoc d) => new()
    {
        Id = d.id ?? "",
        Nombre = d.nombre ?? "",
        Universo = (Universo)(d.universo ?? 0),
        Atributos = new AtributosPersonaje
        {
            Fuerza = d.atributos?.fuerza ?? 0,
            Velocidad = d.atributos?.velocidad ?? 0,
            Inteligencia = d.atributos?.inteligencia ?? 0,
            Resistencia = d.atributos?.resistencia ?? 0,
            PoderEspecial = d.atributos?.poderEspecial ?? 0
        }
    };

    private sealed class PersonajeDoc
    {
        public string? id { get; set; }
        public string? nombre { get; set; }
        public int? universo { get; set; }
        public AtributosDoc? atributos { get; set; }
    }

    private sealed class AtributosDoc
    {
        public int fuerza { get; set; }
        public int velocidad { get; set; }
        public int inteligencia { get; set; }
        public int resistencia { get; set; }
        public int poderEspecial { get; set; }
    }
}

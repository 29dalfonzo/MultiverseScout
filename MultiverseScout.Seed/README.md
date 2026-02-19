# Multiverse Scout – Seed de personajes

Rellena la base de datos RethinkDB con **5 héroes de Marvel** y **5 de DC** para poder usarlos en la app.

## Requisitos

- RethinkDB en ejecución (por ejemplo con `docker compose up -d` en la raíz del repo).
- .NET 8 SDK.

## Uso

### Opción 1: Desde la raíz del repo

```powershell
cd C:\Projects\ART\TareaIA
dotnet run --project MultiverseScout.Seed
```

### Opción 2: Script PowerShell (tras levantar Docker)

```powershell
cd C:\Projects\ART\TareaIA
.\scripts\seed-database.ps1
```

### Opción 3: Variables de entorno (RethinkDB en otro host/puerto)

```powershell
$env:RETHINKDB_HOST = "localhost"
$env:RETHINKDB_PORT = "28015"
dotnet run --project MultiverseScout.Seed
```

## Publicar como ejecutable

Para generar un .exe que puedas ejecutar cuando quieras (sin necesidad de `dotnet run`):

```powershell
cd C:\Projects\ART\TareaIA
dotnet publish MultiverseScout.Seed\MultiverseScout.Seed.csproj -c Release -o publish\seed
```

El ejecutable quedará en `publish\seed\MultiverseScout.Seed.exe`. Copia esa carpeta donde quieras y ejecuta el .exe cuando Docker (RethinkDB) esté levantado.

## Personajes insertados

| Marvel (5)      | DC (5)        |
|-----------------|---------------|
| Iron Man        | Superman      |
| Capitán América | Batman        |
| Thor            | Wonder Woman  |
| Hulk            | Flash         |
| Spider-Man      | Aquaman       |

Si un personaje ya existe (mismo `id`), se omite y no se duplica.

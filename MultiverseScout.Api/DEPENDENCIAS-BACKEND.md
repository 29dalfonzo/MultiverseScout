# Dependencias NuGet – Backend (Orleans + RethinkDB + API)

Proyecto: **MultiverseScout.Api** – ASP.NET Core con Orleans (silo), Minimal APIs y RethinkDB.

---

## 1. Orleans con hosting (ASP.NET Core)

Un solo paquete **metapaquete** incluye silo, runtime y capacidad de actuar como cliente en el mismo proceso:

| Paquete | Versión recomendada | Propósito |
|--------|----------------------|-----------|
| `Microsoft.Orleans.Server` | **8.2.0** | Hosting del silo en ASP.NET Core; incluye dependencias para grains y, en el mismo host, uso de `IClusterClient` (no hace falta un paquete “solo cliente” para la API). |

**Comando:**

```bash
cd MultiverseScout.Api
dotnet add package Microsoft.Orleans.Server --version 8.2.0
```

**Nota:** Para .NET 9 existe `Microsoft.Orleans.Server` 9.x; para este proyecto se usa **8.2.0** para alinear con .NET 8 (igual que el frontend Blazor).

---

## 2. Cliente Orleans para que el frontend o un API Gateway llamen a los grains

- **Frontend Blazor** no se conecta directamente a Orleans: llama por **HTTP** a la API REST del backend.
- La **API (Minimal APIs)** corre en el mismo proceso que el silo y usa **`IClusterClient`** para llamar a los grains. Ese cliente viene incluido con **Microsoft.Orleans.Server** (el host ya puede resolver `IClusterClient`).
- Si en el futuro tuvieras un **proyecto separado** que solo consuma grains (por ejemplo otro servicio), ahí sí añadirías:

| Paquete | Versión | Uso |
|--------|---------|-----|
| `Microsoft.Orleans.Client` | **8.2.0** | Solo si tienes un proceso que no hospeda silo y solo se conecta al cluster (no es el caso actual). |

**Resumen:** Con **Microsoft.Orleans.Server** en el host de la API no necesitas instalar un paquete extra para que la API llame a los grains.

---

## 3. Persistencia: conector/cliente RethinkDB desde C#

**No existe driver oficial** de RethinkDB para .NET (RethinkDB no mantiene un cliente .NET). Se usan paquetes **de comunidad**:

| Paquete | Versión recomendada | Origen | Notas |
|--------|----------------------|--------|--------|
| **RethinkDb.Driver** | **2.3.150** | Comunidad (bchavez) | Cobertura ReQL amplia, .NET Standard 2.0. Repo: [bchavez/RethinkDb.Driver](https://github.com/bchavez/RethinkDb.Driver). |
| rethinkdb-net | 0.12.1 | Comunidad (mfenniak) | Alternativa; depende de protobuf-net. |

**Recomendación:** **RethinkDb.Driver** (más usado y mantenido).

**Comando:**

```bash
dotnet add package RethinkDb.Driver --version 2.3.150
```

**Configuración típica** (ejemplo en `appsettings.json`):

```json
"RethinkDB": {
  "Host": "localhost",
  "Port": 28015,
  "Timeout": 30
}
```

Conexión en código: uso de `RethinkDB.R.Connection().Hostname(...).Port(...).Connect()` (ver documentación del driver).

---

## 4. API REST o Minimal APIs

- **Minimal APIs** forman parte de **ASP.NET Core** y no requieren paquetes adicionales.
- El SDK del proyecto es **Microsoft.NET.Sdk.Web** (ya en el `.csproj`), que incluye todo lo necesario para `MapGet`, `MapPost`, enrutamiento, CORS, etc.

No hay que instalar ningún paquete extra para exponer operaciones de batallas y votación vía REST.

---

## Resumen de comandos (solo backend)

Desde la raíz del repo o desde la carpeta del backend:

```bash
cd C:\Projects\ART\TareaIA\MultiverseScout.Api

# Orleans (silo + cliente en proceso)
dotnet add package Microsoft.Orleans.Server --version 8.2.0

# RethinkDB (comunidad)
dotnet add package RethinkDb.Driver --version 2.3.150
```

---

## Restaurar y ejecutar

```bash
cd C:\Projects\ART\TareaIA\MultiverseScout.Api
dotnet restore
dotnet run
```

La API quedará disponible en la URL configurada (por defecto en `launchSettings.json`, por ejemplo `http://localhost:5050`). Ajusta `Cors:AllowedOrigins` en `appsettings.json` con la URL del frontend Blazor (por ejemplo `https://localhost:7xxx`) cuando la tengas.

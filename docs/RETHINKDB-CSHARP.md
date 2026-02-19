# RethinkDB en C# (.NET) – Driver, configuración y Docker

---

## 1. ¿Existe driver/cliente oficial o mantenido para RethinkDB en .NET?

**No.** RethinkDB no ofrece un driver oficial para .NET/C#. En la documentación oficial solo figuran como drivers oficiales: **Java, Python, Ruby y JavaScript**. Para .NET hay que usar drivers **de comunidad**.

---

## 2. Si no hay oficial, ¿cuál es el paquete NuGet recomendado (comunidad)?

**Recomendado: RethinkDb.Driver** (autor: bchavez).

| Paquete           | Versión   | NuGet / repo |
|-------------------|-----------|--------------|
| **RethinkDb.Driver** | **2.3.150** (o última estable) | [NuGet](https://www.nuget.org/packages/RethinkDb.Driver) · [GitHub bchavez/RethinkDb.Driver](https://github.com/bchavez/RethinkDb.Driver) |

**Por qué este:**  
Cobertura amplia de la API ReQL, .NET Standard 2.0 (compatible con .NET 8), documentación en wiki (conexión, pooling, SSL), y es el más usado en la comunidad.

**Alternativa:**  
`rethinkdb-net` (mfenniak) – [NuGet](https://www.nuget.org/packages/rethinkdb-net) – versión 0.12.1; depende de protobuf-net. También es de comunidad.

**Instalación:**

```bash
dotnet add package RethinkDb.Driver --version 2.3.150
```

**Dependencias adicionales:**  
No hace falta ningún otro paquete NuGet para la conexión básica. La serialización JSON se hace con el driver.

---

## 3. Configuración de la conexión (host, port, “connection string”)

RethinkDB **no usa un “connection string” único** como SQL Server o MongoDB. Se configura por **opciones**: host, puerto, timeout, etc.

### Valores por defecto

- **Puerto de clientes (driver):** 28015  
- **Consola web (admin):** 8080  
- **Cluster:** 29015  

### Opción A: Desde código (RethinkDb.Driver)

**Conexión simple (una sola instancia):**

```csharp
using RethinkDb.Driver;

var R = RethinkDB.R;
var conn = R.Connection()
    .Hostname("localhost")   // o IP, ej. "192.168.0.100"
    .Port(28015)            // opcional; por defecto 28015
    .Timeout(60)            // segundos
    .Connect();

// Uso
var result = R.Now().Run<DateTimeOffset>(conn);
```

**Conexión asíncrona:**

```csharp
var conn = await R.Connection()
    .Hostname("localhost")
    .Port(28015)
    .Timeout(60)
    .ConnectAsync();
```

**Pool de conexiones (varios nodos):**

```csharp
var conn = R.ConnectionPool()
    .Seed(new[] { "192.168.0.11:28015", "192.168.0.12:28015" })
    .PoolingStrategy(new RoundRobinHostPool())
    .Discover(true)
    .Connect();
```

El driver no acepta un único string tipo `"Host=localhost;Port=28015"`; hay que usar el builder (`.Hostname()`, `.Port()`, etc.).

### Opción B: Desde configuración (appsettings.json) en ASP.NET Core

En **appsettings.json**:

```json
{
  "RethinkDB": {
    "Host": "localhost",
    "Port": 28015,
    "Timeout": 30
  }
}
```

En código (registro como singleton y uso):

```csharp
// En Program.cs o donde registres servicios
var config = builder.Configuration.GetSection("RethinkDB");
var host = config["Host"] ?? "localhost";
var port = int.Parse(config["Port"] ?? "28015", System.Globalization.CultureInfo.InvariantCulture);
var timeout = int.Parse(config["Timeout"] ?? "30", System.Globalization.CultureInfo.InvariantCulture);

var conn = RethinkDB.R.Connection()
    .Hostname(host)
    .Port(port)
    .Timeout(timeout)
    .Connect();

builder.Services.AddSingleton(conn);
```

Así tienes el equivalente a un “connection string” pero repartido en opciones (Host, Port, Timeout). Puedes añadir más claves (AuthKey, SSL, etc.) si el driver lo soporta y leerlas desde `IConfiguration`.

### Resumen “connection string”

- No hay un solo connection string estándar para RethinkDB en .NET.
- **Host:** `localhost` o IP.  
- **Port:** normalmente **28015**.  
- **Timeout:** en segundos (ej. 30).  
- Configuración recomendada: leer **Host**, **Port** y **Timeout** de `appsettings.json` (o variables de entorno) y construir la conexión con el builder del driver.

---

## 4. Docker: ¿existe opción y es más fácil de configurar?

**Sí.** La imagen **oficial** de RethinkDB está en Docker Hub: **[rethinkdb](https://hub.docker.com/_/rethinkdb)**.

### Arrancar un contenedor (desarrollo local)

```bash
docker run --name rethinkdb -d -p 28015:28015 -p 8080:8080 rethinkdb
```

- **28015:** puerto del driver (tu app C# se conecta aquí).  
- **8080:** consola web de administración (abres en el navegador).

Con persistencia en un volumen:

```bash
docker run --name rethinkdb -v "$PWD/rethinkdb_data:/data" -d -p 28015:28015 -p 8080:8080 rethinkdb
```

En Windows (PowerShell), si usas volumen local:

```powershell
docker run --name rethinkdb -v "${PWD}/rethinkdb_data:/data" -d -p 28015:28015 -p 8080:8080 rethinkdb
```

### ¿Es más fácil con Docker?

**Sí**, para desarrollo y pruebas:

1. No instalas RethinkDB en el sistema, solo Docker.
2. Un solo comando (`docker run`) deja el servidor listo.
3. Mismo entorno en todos los equipos (dev/CI).
4. Puedes usar la misma imagen en `docker-compose` junto con tu API (Orleans) y el frontend.

### Ejemplo con docker-compose (opcional)

En la raíz del repo puedes tener un `docker-compose.yml`:

```yaml
services:
  rethinkdb:
    image: rethinkdb:2.4
    container_name: multiverse-rethinkdb
    ports:
      - "28015:28015"
      - "8080:8080"
    volumes:
      - rethinkdb_data:/data
    restart: unless-stopped

volumes:
  rethinkdb_data:
```

Tu app C# usaría **Host: `localhost`**, **Port: `28015`** (o el nombre del servicio `rethinkdb` si la API corre también en Docker en la misma red).

---

## Resumen

| Pregunta | Respuesta |
|----------|-----------|
| **Driver oficial .NET** | No existe. |
| **Paquete recomendado** | **RethinkDb.Driver** (NuGet, comunidad). |
| **Configuración** | Host + Port + Timeout vía builder (`.Hostname()`, `.Port()`, `.Timeout()`); no hay un solo “connection string”; se puede leer de `appsettings.json`. |
| **Dependencias extra** | Ninguna para uso básico. |
| **Docker** | Sí; imagen oficial `rethinkdb`; puertos 28015 (driver) y 8080 (admin). Suele ser la opción más sencilla para desarrollo. |

# Cómo iniciar Multiverse Scout (guía para nuevos desarrolladores)

Esta guía indica el **orden** en que hay que levantar cada parte del proyecto y cómo usar Docker para la base de datos.

---

## Requisitos previos

- **.NET 8 SDK** — [Descargar](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Docker Desktop** (o Docker + Docker Compose) — para RethinkDB
- **Git** (si clonas el repositorio)

En Windows, si ves errores de “Control de aplicaciones bloqueó este archivo” al ejecutar `dotnet run`, revisa [esta guía de Microsoft](https://support.microsoft.com/es-es/windows/control-de-aplicaciones-inteligentes-ha-bloqueado-parte-de-esta-aplicaci%C3%B3n-0729fff1-48bf-4b25-aa97-632fe55ccca2) o desactiva temporalmente **Control de aplicaciones inteligentes** en Configuración → Privacidad y seguridad → Seguridad de Windows.

---

## Orden de inicio (resumen)

| Paso | Qué levantar        | Comando / Acción                          |
|------|---------------------|-------------------------------------------|
| 1    | RethinkDB (Docker)  | `docker compose up -d`                    |
| 2    | Datos iniciales     | `dotnet run --project MultiverseScout.Seed` (solo la primera vez) |
| 3    | API backend         | `dotnet run --project MultiverseScout.Api` |
| 4    | Frontend Blazor     | `dotnet run --project MultiverseScout`    |

---

## Paso 1: Base de datos (RethinkDB con Docker)

La API y el Seed usan **RethinkDB**. La forma más sencilla de tenerla en local es con Docker.

Desde la **raíz del repositorio** (donde está `docker-compose.yml`):

```bash
docker compose up -d
```

- **Puerto de conexión:** `28015` (lo usa la API y el Seed).
- **Consola web de RethinkDB:** http://localhost:8080 (opcional, para ver datos).

Comprobar que el contenedor está en marcha:

```bash
docker compose ps
```

Debe aparecer el servicio `rethinkdb` (o `multiverse-rethinkdb`) en estado “Up”.

---

## Paso 2: Datos iniciales (Seed, solo la primera vez)

El proyecto **MultiverseScout.Seed** crea la base `multiverse`, la tabla `personajes` e inserta 10 personajes (Marvel y DC). Conviene ejecutarlo **una vez** después de levantar RethinkDB.

Desde la **raíz del repositorio**:

```bash
dotnet run --project MultiverseScout.Seed
```

Si RethinkDB no está en `localhost:28015`, puedes usar variables de entorno:

```bash
# Windows (PowerShell)
$env:RETHINKDB_HOST="localhost"; $env:RETHINKDB_PORT="28015"; dotnet run --project MultiverseScout.Seed

# Linux/macOS
RETHINKDB_HOST=localhost RETHINKDB_PORT=28015 dotnet run --project MultiverseScout.Seed
```

---

## Paso 3: API (backend)

La API expone los endpoints que usa el frontend (personajes, batallas, guion, etc.).

Desde la **raíz del repositorio**:

```bash
dotnet run --project MultiverseScout.Api
```

- **URL de la API:** http://localhost:5050  
- La configuración está en `MultiverseScout.Api/Properties/launchSettings.json` y en `appsettings.json` (RethinkDB: `localhost`, puerto `28015`, base `multiverse`).

Si RethinkDB no está levantado o no es accesible, la API fallará al iniciar con un error de conexión. Asegúrate de que el **Paso 1** esté hecho.

---

## Paso 4: Frontend (Blazor WebAssembly)

El cliente web se conecta por defecto a la API en `http://localhost:5050`. Debe ejecutarse **después** de que la API esté en marcha.

Desde la **raíz del repositorio**:

```bash
dotnet run --project MultiverseScout
```

Se abrirá el navegador o verás en consola una URL del tipo `https://localhost:7xxx` o `http://localhost:5xxx`. Abre esa URL para usar la aplicación.

---

## Resumen de puertos

| Servicio           | Puerto  | URL / Uso                          |
|--------------------|---------|------------------------------------|
| RethinkDB (driver) | 28015   | Conexión API y Seed                |
| RethinkDB (web UI) | 8080    | http://localhost:8080              |
| MultiverseScout.Api| 5050    | http://localhost:5050              |
| MultiverseScout (Blazor) | 5xxx/7xxx | La que muestre `dotnet run` |

---

## Solución de problemas

- **“No se puede establecer una conexión” (10061)**  
  RethinkDB no está en marcha o no es accesible. Ejecuta `docker compose up -d` y espera unos segundos antes de levantar la API o el Seed.

- **“Control de aplicaciones bloqueó este archivo” (0x800711C7)**  
  En Windows, desbloquear la carpeta del proyecto (Propiedades → Desbloquear) o desactivar temporalmente Control de aplicaciones inteligentes. Ver enlace de requisitos previos.

- **Frontend no carga personajes**  
  Comprueba que la API esté corriendo en http://localhost:5050 y que hayas ejecutado el Seed al menos una vez.

- **Parar Docker**  
  En la raíz del repo: `docker compose down`. Los datos en el volumen `rethinkdb_data` se conservan.

---

## Estructura de la solución

- **MultiverseScout.Contracts** — DTOs y contratos compartidos (referenciado por API y Blazor).
- **MultiverseScout.Seed** — Consola que inserta datos iniciales en RethinkDB.
- **MultiverseScout.Api** — API REST + Orleans; depende de RethinkDB.
- **MultiverseScout** — Frontend Blazor WebAssembly; consume la API en `localhost:5050`.

Orden lógico: primero RethinkDB (Docker), luego Seed (opcional primera vez), después API y por último el frontend.

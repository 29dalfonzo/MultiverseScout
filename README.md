# Multiverse Scout — Marvel vs DC

Aplicación web para enfrentar personajes de Marvel y DC, comparar atributos y votar en batallas. Estilo cómic, división diagonal de pantalla y guion narrativo generado para cada encuentro.

---

## Inicio rápido (Docker)

Con **Docker** y **Docker Compose** instalados, en la raíz del repositorio:

```bash
docker compose up -d
```

Se levantan en orden: **RethinkDB** → **Seed** (datos iniciales, solo si la base está vacía) → **API** → **Frontend**.

| Dónde              | URL                      |
|--------------------|--------------------------|
| **App (frontend)** | http://localhost:8080    |
| **API**            | http://localhost:5050    |
| **RethinkDB (UI)** | http://localhost:8081    |

Parar todo: `docker compose down` (los datos se conservan en el volumen).

---

## Stack

- **Frontend:** Blazor WebAssembly (.NET 8)
- **Backend:** ASP.NET Core + Orleans (.NET 8)
- **Base de datos:** RethinkDB
- **Contenedores:** Docker Compose (BD, seed, API y frontend)

---

## Estructura del repositorio

| Proyecto / carpeta      | Descripción                                      |
|-------------------------|--------------------------------------------------|
| `MultiverseScout`      | Frontend Blazor (página principal, votación, timer) |
| `MultiverseScout.Api`  | API REST + grains Orleans                       |
| `MultiverseScout.Seed` | Consola que crea la base y carga 10 personajes  |
| `MultiverseScout.Contracts` | DTOs compartidos (personajes, batallas, votos) |
| `docs/`                 | Documentación (p. ej. [Cómo iniciar](docs/COMO-INICIAR.md)) |

---

## Desarrollo sin Docker

Si prefieres ejecutar todo en local con .NET 8:

1. Base de datos: `docker compose up -d` (solo RethinkDB).
2. Datos iniciales: `dotnet run --project MultiverseScout.Seed`.
3. API: `dotnet run --project MultiverseScout.Api`.
4. Frontend: `dotnet run --project MultiverseScout`.

Guía detallada: [docs/COMO-INICIAR.md](docs/COMO-INICIAR.md).

---

## Licencia y autor

Proyecto de uso educativo / demostración. Detalles en el repositorio.

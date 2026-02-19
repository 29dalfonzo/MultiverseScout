# Marvel vs DC: "The Multiverse Scout" — Notas IA

**Frontend:** Blazor (WebAssembly)  
**Backend:** C# (Orleans)  
**BBDD:** RethinkDB  
**Estilo:** Modern Comic. Onomatopeyas, colores saturados y división de pantalla diagonal.  
**Objetivo:** Comparador de atributos y sistema de votación de batallas.  
**Acción:** Enfrentar dos perfiles, comparar sus stats y permitir el voto de la comunidad.  
**Reto Vibe:** Generar un guion breve de encuentro narrativo entre ambos personajes.  
**Responsable:** Daniel  

---

## 1. Prompts para descubrir dependencias

### Frontend (Blazor WebAssembly)

```
Necesito un proyecto Blazor WebAssembly (standalone) en .NET 8 o superior. 
Lista las dependencias NuGet necesarias para:
- Blazor WebAssembly con hosting mínimo
- Cliente HTTP para consumir la API del backend
- Serialización JSON (System.Text.Json o equivalente)
- Opcional: librería de UI que encaje con estilo "comic" o componentes base

Indica versión recomendada de cada paquete y el comando dotnet add package exacto.
```

### Backend (Orleans)

```
Necesito un proyecto backend en C# usando Microsoft Orleans (grains, silos). 
Lista las dependencias NuGet necesarias para:
- Orleans con hosting (ASP.NET Core)
- Cliente Orleans para que el frontend o un API Gateway pueda llamar a los grains
- Persistencia: conector/cliente para RethinkDB desde C#
- API REST o minimal APIs que expongan las operaciones de batallas y votación

Indica versión de cada paquete y si hay paquetes oficiales o de comunidad para RethinkDB en .NET.
```

### Base de datos (RethinkDB)

```
Para un proyecto C# que usa RethinkDB:
- ¿Existe driver/cliente oficial o mantenido para RethinkDB en .NET?
- Si no hay oficial, ¿cuál es el paquete NuGet recomendado (comunidad)?
- Indica cómo configurar la conexión (connection string, host, port) y dependencias adicionales.
```

---

## 2. Planificación paso a paso con prompts por fase

### Fase 0: Repositorio y solución

**Objetivo:** Solución con proyecto Blazor WASM, proyecto Orleans/API y referencias claras.

**Prompt:**

```
Crear una solución .NET con:
1) Proyecto Blazor WebAssembly (standalone) para el frontend "Multiverse Scout"
2) Proyecto host para Orleans (silo) que también exponga una API REST o Minimal API para batallas y votación
3) Proyecto de biblioteca de clases compartida (contracts/DTOs) referenciado por frontend y backend

Estructura de carpetas y nombres de proyectos siguiendo convenciones .NET. Sin implementar lógica aún, solo skeleton.
```

**Estado Fase 0 (completada):**
- **1)** `MultiverseScout/` — Blazor WebAssembly (standalone), .NET 8.
- **2)** `MultiverseScout.Api/` — Host Orleans (silo) + Minimal API para batallas y votación.
- **3)** `MultiverseScout.Contracts/` — Biblioteca de clases compartida (contracts/DTOs), referenciada por MultiverseScout y MultiverseScout.Api.
- **Solución:** `MultiverseScout.sln` en la raíz, incluye los tres proyectos y referencias cruzadas (frontend y API → Contracts).

---

### Fase 1: Modelo de datos y persistencia

**Objetivo:** Personajes, atributos (stats), batallas y votos en RethinkDB.

**Estado Fase 1 (completada):**
- **Contracts:** `Universo`, `AtributosPersonaje`, `PersonajeDto`, `BatallaDto`, `VotoDto`, `ResultadosBatallaDto`.
- **RethinkDB:** `RethinkDbSetup` (tablas: personajes, batallas, votos; índices: universo, fecha, batallaId); `RethinkDbOptions` y `RethinkDbStartupService` para asegurar DB al arranque.
- **Repositorios:** `IPersonajesRepository` / `PersonajesRepository` (GetPersonaje, ListarPersonajes); `IBatallasRepository` / `BatallasRepository` (CrearBatalla, GetBatalla, GetResultadosBatalla, RegistrarVoto con comprobación de doble voto por usuarioSesion).
- **Grains:** `IPersonajesGrain` / `PersonajesGrain` (clave fija), `IBatallaGrain` / `BatallaGrain` (clave = batallaId), `IBatallasFactoryGrain` / `BatallasFactoryGrain` (CrearBatalla). Todos usan los repositorios inyectados.
- **Program.cs:** Conexión RethinkDB singleton, registro de repos, `RethinkDbStartupService`, Orleans sin cambios.

**Prompt:**

```
Para "Multiverse Scout" (Marvel vs DC):
- Definir modelo de datos: Personaje (nombre, universo Marvel/DC, atributos: fuerza, velocidad, inteligencia, resistencia, poder especial), Batalla (dos personajes, fecha, votos por cada uno), Voto (batallaId, personajeId, usuario/sesión).
- En C#, crear entidades/DTOs y configuración de tablas/índices en RethinkDB.
- Crear un grain de Orleans (o servicio) que lea/escriba personajes y batallas en RethinkDB. Incluir métodos: GetPersonaje(id), ListarPersonajes(universo?), CrearBatalla(id1, id2), RegistrarVoto(batallaId, personajeId), GetResultadosBatalla(batallaId).
```

---

### Fase 2: API y grains

**Objetivo:** Endpoints que el frontend pueda consumir.

**Prompt:**

```
En el host de Orleans ya creado, exponer Minimal API (o controllers) que:
- GET /api/personajes?universo=Marvel|DC
- GET /api/personajes/{id}
- POST /api/batallas (body: { personajeAId, personajeBId }) → devuelve batalla creada con id
- GET /api/batallas/{id} (detalle + resultados de votación)
- POST /api/batallas/{id}/voto (body: { personajeId }) para registrar voto

Que los endpoints llamen a los grains de Orleans correspondientes. Incluir CORS para el origen del Blazor WASM.
```

**Estado Fase 2 (completada):**
- **GET /api/personajes?universo=Marvel|DC** → `IPersonajesGrain.ListarPersonajesAsync(universo)`.
- **GET /api/personajes/{id}** → `IPersonajesGrain.GetPersonajeAsync(id)`.
- **POST /api/batallas** (body: `{ personajeAId, personajeBId }`) → `IBatallasFactoryGrain.CrearBatallaAsync`; devuelve batalla creada (201 Created).
- **GET /api/batallas/{id}** → `IBatallaGrain.GetBatallaAsync` + `GetResultadosBatallaAsync`; respuesta `BatallaDetalleResponse` (batalla + resultados).
- **POST /api/batallas/{id}/voto** (body: `{ personajeId, usuarioSesion? }`) → `IBatallaGrain.RegistrarVotoAsync`; si no se envía `usuarioSesion` se usa cabecera `X-Session-Id` o un Guid nuevo. 409 si ya votó.
- CORS ya configurado en Program.cs.

---

### Fase 3: UI "Modern Comic" y comparador

**Objetivo:** Pantalla con división diagonal, stats y estilo comic.

**Prompt:**

```
En el proyecto Blazor WebAssembly:
- Una página principal con estilo "Modern Comic": colores muy saturados (primarios tipo cómic), tipografía bold/display, y división diagonal de pantalla (por ejemplo 50% izquierda / 50% derecha con una línea diagonal).
- Cada mitad muestra un personaje (foto, nombre, universo). Debajo, una tabla o cards de atributos (fuerza, velocidad, inteligencia, resistencia, poder) con barras o números.
- Añadir onomatopeyas decorativas (POW, BAM, etc.) como elementos visuales CSS o SVG, sin afectar accesibilidad.
- Selector para elegir dos personajes (dropdowns o búsqueda) y botón "Enfrentar" que llame al backend para crear/obtener una batalla.
```

**Estado Fase 3 (completada):**
- **Estilo Modern Comic:** Variables CSS (rojo, azul, amarillo, blanco, negro); tipografía Bebas Neue (Google Fonts); colores saturados y bordes gruesos.
- **División diagonal:** Grid 50% / 50% con línea central inclinada (::before); en móvil se apila verticalmente con línea horizontal.
- **Cada mitad:** Card con avatar placeholder, nombre, universo y tabla de atributos (Fuerza, Velocidad, Inteligencia, Resistencia, Poder) con barras y número; valores normalizados a 100% para la barra.
- **Onomatopeyas:** POW, BAM, VS como elementos decorativos con `aria-hidden="true"` y opacidad reducida.
- **Selectores y Enfrentar:** Dos `<select>` (Personaje A / B) cargados con GET /api/personajes; botón "Enfrentar" que llama a POST /api/batallas y luego GET batalla + GET personajes para rellenar las dos mitades.
- **Servicios:** IApiClient / ApiClient en Blazor; HttpClient con BaseAddress desde appsettings.json (ApiBaseUrl); appsettings.json con `http://localhost:5050`.

---

### Fase 4: Votación y resultados

**Objetivo:** Votar y ver quién gana.

**Prompt:**

```
En la misma página de batalla en Blazor:
- Tras mostrar los dos personajes enfrentados, botones "Votar por [Nombre A]" y "Votar por [Nombre B]" que llamen a POST /api/batallas/{id}/voto.
- Mostrar contador de votos en tiempo real o tras recargar (GET batalla).
- Evitar doble voto por usuario: usar localStorage o cookie con un identificador de sesión y validar en backend si ya votó en esa batalla.
```

**Estado Fase 4 (completada):**
- **Botones de votación:** En cada mitad de la batalla, "Votar por [Nombre A]" y "Votar por [Nombre B]" que llaman a POST /api/batallas/{id}/voto con personajeId y usuarioSesion.
- **Contador de votos:** En cada card se muestra "Votos: N" (batallaDetalle.Resultados.VotosPersonajeA / VotosPersonajeB); tras votar se hace GET batalla y se actualiza batallaDetalle para refrescar los números.
- **Evitar doble voto:** Servicio ISessionIdService / SessionIdService que usa JS (wwwroot/js/session.js) para getOrCreateSessionId en localStorage (clave MultiverseScout.SessionId). Ese id se envía en el body como usuarioSesion; el backend ya rechaza con 409 si ya votó. Mensaje "Ya has votado en esta batalla" si 409.
- **ApiClient:** RegistrarVotoAsync(batallaId, personajeId, usuarioSesion) que devuelve (bool Ok, bool YaVoto). Estilos .vote-section, .btn-vote, .comic-voto-message en app.css.

---

### Fase 5: Guion narrativo (reto "Vibe")

**Objetivo:** Pequeño guion de encuentro entre los dos personajes.

**Prompt:**

```
En la página de batalla, después de los atributos y la votación, añadir una sección "Encuentro en el Multiverso". 
Al cargar la batalla entre personaje A y personaje B, generar un guion breve (2–4 párrafos) de encuentro narrativo entre ambos: diálogo o narración en estilo cómic, usando sus nombres y atributos (ej. si uno tiene más fuerza, que se note en la escena). 
Opciones: (a) texto generado en backend con plantillas C# según atributos; (b) llamada a un API de IA (OpenAI/Azure) con prompt que incluya nombres y stats; (c) varios guiones predefinidos y elegir uno al azar. Implementar la opción (a) o (c) primero sin APIs externas.
```

**Estado Fase 5 (completada):**
- **Backend:** IGuionService / GuionService con 4 plantillas de 2–4 párrafos; placeholders {NombreA}, {NombreB}, {FuerzaFrase}, {VelocidadFrase}. Frases condicionales según quién tiene más fuerza/velocidad (GenerarFuerzaFrase, GenerarVelocidadFrase). Se elige una plantilla al azar y se rellenan los huecos.
- **Endpoint:** GET /api/batallas/{id}/guion que obtiene batalla (grain), personajes A y B (grain), llama a guionService.Generar y devuelve GuionResponse(Guion).
- **Blazor:** GetGuionAsync(batallaId) en ApiClient; tras cargar la batalla se llama CargarGuionAsync(); sección "Encuentro en el Multiverso" con título, estado "Generando encuentro…" y texto del guion convertido a párrafos HTML (HtmlEncode + &lt;p&gt;). Estilos .comic-guion, .guion-title, .guion-text en app.css.

---

## 3. Orden sugerido de ejecución


| Orden | Fase         | Entregable                                              |
| ----- | ------------ | ------------------------------------------------------- |
| 1     | Dependencias | Lista de paquetes y comandos `dotnet add package`       |
| 2     | Fase 0       | Solución con 3 proyectos (Blazor, Orleans host, Shared) |
| 3     | Fase 1       | Modelos, RethinkDB y grain(s) de persistencia           |
| 4     | Fase 2       | API REST y CORS                                         |
| 5     | Fase 3       | UI comic, diagonal y comparador de stats                |
| 6     | Fase 4       | Votación y resultados                                   |
| 7     | Fase 5       | Sección "Encuentro en el Multiverso" con guion breve    |


---

## 4. Prompt resumen para todo el stack

```
Proyecto "The Multiverse Scout": comparador Marvel vs DC con votación.

Stack: Blazor WebAssembly (frontend), C# backend con Orleans (grains), RethinkDB (persistencia).

Necesito:
1) Lista de dependencias NuGet para Blazor WASM, Orleans con host, cliente RethinkDB para C#, y API REST.
2) Plan de desarrollo en fases: (0) solución y proyectos, (1) modelos y persistencia en RethinkDB + grains, (2) API para personajes y batallas y votos, (3) UI estilo comic (división diagonal, colores saturados, onomatopeyas, comparador de stats), (4) votación y resultados, (5) sección de guion narrativo breve del encuentro entre los dos personajes.

Para cada fase, indica tareas concretas y posibles prompts que pueda usar con un asistente de código para implementarlo paso a paso.
```


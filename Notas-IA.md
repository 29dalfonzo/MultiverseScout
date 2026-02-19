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

---

### Fase 1: Modelo de datos y persistencia
**Objetivo:** Personajes, atributos (stats), batallas y votos en RethinkDB.

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

---

### Fase 5: Guion narrativo (reto "Vibe")
**Objetivo:** Pequeño guion de encuentro entre los dos personajes.

**Prompt:**
```
En la página de batalla, después de los atributos y la votación, añadir una sección "Encuentro en el Multiverso". 
Al cargar la batalla entre personaje A y personaje B, generar un guion breve (2–4 párrafos) de encuentro narrativo entre ambos: diálogo o narración en estilo cómic, usando sus nombres y atributos (ej. si uno tiene más fuerza, que se note en la escena). 
Opciones: (a) texto generado en backend con plantillas C# según atributos; (b) llamada a un API de IA (OpenAI/Azure) con prompt que incluya nombres y stats; (c) varios guiones predefinidos y elegir uno al azar. Implementar la opción (a) o (c) primero sin APIs externas.
```

---

## 3. Orden sugerido de ejecución

| Orden | Fase | Entregable |
|-------|------|------------|
| 1 | Dependencias | Lista de paquetes y comandos `dotnet add package` |
| 2 | Fase 0 | Solución con 3 proyectos (Blazor, Orleans host, Shared) |
| 3 | Fase 1 | Modelos, RethinkDB y grain(s) de persistencia |
| 4 | Fase 2 | API REST y CORS |
| 5 | Fase 3 | UI comic, diagonal y comparador de stats |
| 6 | Fase 4 | Votación y resultados |
| 7 | Fase 5 | Sección "Encuentro en el Multiverso" con guion breve |

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

# Multiverse Scout Seed — ejecuta una vez para crear DB y personajes si no hay datos.
# Build desde raíz: docker build -t multiverse-seed -f MultiverseScout.Seed/Dockerfile .

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY MultiverseScout.Seed/ MultiverseScout.Seed/

RUN dotnet restore MultiverseScout.Seed/MultiverseScout.Seed.csproj
RUN dotnet publish MultiverseScout.Seed/MultiverseScout.Seed.csproj -c Release -o /app/publish --no-restore

# aspnet incluye más dependencias que runtime; evita exit 139 (SIGSEGV) con el driver RethinkDB
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# RETHINKDB_HOST y RETHINKDB_PORT se inyectan en docker-compose (rethinkdb)
ENTRYPOINT ["dotnet", "MultiverseScout.Seed.dll"]

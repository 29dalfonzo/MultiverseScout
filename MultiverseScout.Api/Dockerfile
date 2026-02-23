# Multiverse Scout API (Orleans + RethinkDB)
# Build: docker build -t multiverse-api -f MultiverseScout.Api/Dockerfile .
# En docker-compose el contexto es la raíz del repo.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY MultiverseScout.Contracts/ MultiverseScout.Contracts/
COPY MultiverseScout.Api/ MultiverseScout.Api/

RUN dotnet restore MultiverseScout.Api/MultiverseScout.Api.csproj
RUN dotnet publish MultiverseScout.Api/MultiverseScout.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

# RethinkDB se sobreescribe por env en docker-compose (RethinkDB__Host=rethinkdb)
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "MultiverseScout.Api.dll"]

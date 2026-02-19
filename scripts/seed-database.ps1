# Seed de personajes en RethinkDB (Multiverse Scout)
# Uso: ejecutar después de "docker compose up -d" en la raíz del repo.
# Ejemplo: .\scripts\seed-database.ps1
# O con RethinkDB en otro host: $env:RETHINKDB_HOST="192.168.1.10"; .\scripts\seed-database.ps1

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$seedProject = Join-Path $root "MultiverseScout.Seed\MultiverseScout.Seed.csproj"

if (-not (Test-Path $seedProject)) {
    Write-Error "No se encuentra el proyecto Seed en $seedProject"
}

Write-Host "Rellendo base de datos con personajes (5 Marvel, 5 DC)..."
Set-Location $root
dotnet run --project $seedProject
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host "Seed completado."

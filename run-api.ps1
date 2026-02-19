# Desbloquea archivos del proyecto (evita error 0x800711C7) y ejecuta la API
$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot

Write-Host "Desbloqueando archivos en $root ..." -ForegroundColor Cyan
Get-ChildItem -Path $root -Recurse -Force -ErrorAction SilentlyContinue | Unblock-File

Write-Host "Limpiando y compilando..." -ForegroundColor Cyan
Set-Location $root
dotnet clean --verbosity quiet
dotnet build --verbosity minimal
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# Desbloquear de nuevo tras el build (MSBuild puede dejar DLLs bloqueados)
Write-Host "Desbloqueando salidas de compilación..." -ForegroundColor Cyan
Get-ChildItem -Path $root -Include "*.dll","*.exe" -Recurse -Force -ErrorAction SilentlyContinue | Where-Object { $_.FullName -match "\\bin\\" } | Unblock-File

Write-Host "Iniciando MultiverseScout.Api..." -ForegroundColor Green
dotnet run --project MultiverseScout.Api

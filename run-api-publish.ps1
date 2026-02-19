# Publica la API en una carpeta "limpia" y ejecuta (evita bloqueo 0x800711C7)
$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$publishDir = Join-Path $env:TEMP "MultiverseScout.Api-publish"

Write-Host "Publicando en $publishDir ..." -ForegroundColor Cyan
Set-Location $root
Remove-Item -Path $publishDir -Recurse -Force -ErrorAction SilentlyContinue
dotnet publish MultiverseScout.Api -c Debug -o $publishDir
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Get-ChildItem -Path $publishDir -Recurse -Force | Unblock-File
Write-Host "Iniciando API..." -ForegroundColor Green
Set-Location $publishDir
& "$env:ProgramFiles\dotnet\dotnet.exe" MultiverseScout.Api.dll

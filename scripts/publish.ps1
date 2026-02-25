# 產生單一執行檔（self-contained, win-x64）
# Usage: .\scripts\publish.ps1
# Output: src\bin\Release\net8.0-windows\win-x64\publish\CursorTestApp.exe

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptDir
$projectDir = Join-Path $repoRoot "src"

if (-not (Test-Path (Join-Path $projectDir "CursorTestApp.csproj"))) {
    Write-Error "Project not found: $projectDir\CursorTestApp.csproj"
}

Write-Host "Publishing single-file executable (win-x64, self-contained)..." -ForegroundColor Cyan
Push-Location $projectDir
try {
    dotnet publish -c Release -r win-x64 --self-contained true `
        -p:PublishSingleFile=true `
        -p:IncludeNativeLibrariesForSelfExtract=true
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed with exit code $LASTEXITCODE" }
    $exePath = Join-Path $projectDir "bin\Release\net8.0-windows\win-x64\publish\CursorTestApp.exe"
    Write-Host "Done. Output: $exePath" -ForegroundColor Green
}
finally {
    Pop-Location
}

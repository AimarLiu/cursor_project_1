# Convert PNG to ICO for application icon
# Usage: .\PngToIco.ps1
# Requires: .NET / System.Drawing

param(
    [string]$PngPath = "..\src\Resources\icons8-app-48.png",
    [string]$IcoPath = "..\src\Resources\icons8-app-48.ico"
)

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$pngFull = Join-Path $scriptDir $PngPath
$icoFull = Join-Path $scriptDir $IcoPath

if (-not (Test-Path $pngFull)) {
    Write-Error "PNG not found: $pngFull"
}

Add-Type -AssemblyName System.Drawing
$bitmap = [System.Drawing.Bitmap]::FromFile((Resolve-Path $pngFull))
try {
    $icon = [System.Drawing.Icon]::FromHandle($bitmap.GetHicon())
    try {
        $fs = [System.IO.File]::Create($icoFull)
        $icon.Save($fs)
        $fs.Close()
        Write-Host "Created: $icoFull"
    } finally {
        [System.Runtime.InteropServices.Marshal]::DestroyIcon($icon.Handle)
    }
} finally {
    $bitmap.Dispose()
}

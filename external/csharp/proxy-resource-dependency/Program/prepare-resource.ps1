param(
    [string]$Version,
    [string]$OutDir
)

$ErrorActionPreference = 'Stop'

$nupkg = Join-Path $OutDir "log4net.$Version.nupkg"
$embedDir = Join-Path $OutDir "embedded"
$dll = Join-Path $embedDir "log4net.dll"

New-Item -ItemType Directory -Force -Path $embedDir | Out-Null

if (-not (Test-Path $nupkg)) {
    Invoke-WebRequest `
        "https://www.nuget.org/api/v2/package/log4net/$Version" `
        -OutFile $nupkg
}

if (-not (Test-Path $dll)) {
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $zip = [IO.Compression.ZipFile]::OpenRead($nupkg)
    try {
        $entry = $zip.Entries |
            Where-Object { $_.FullName -eq 'lib/net462/log4net.dll' }
        if (-not $entry) {
            throw "log4net.dll not found in package"
        }
        $entry.ExtractToFile($dll, $true)
    }
    finally {
        $zip.Dispose()
    }
}

Write-Host "Embedded log4net prepared at $dll"


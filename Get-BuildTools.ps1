$buildTools = "C:\BuildTools"
$nuget = "$buildTools\nuget.exe"

if (-not (Test-Path -Path $buildTools)) {
    Write-Host "Creating directory: $buildTools"
    New-Item -ItemType Directory -Path $buildTools -Force | Out-Null
}

if (-not (Test-Path -Path $nuget)) {
    Write-Host "Downloading nuget.exe..."
    $nugetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
    Invoke-WebRequest -Uri $nugetUrl -OutFile $nuget
}

Write-Host "Installing packages for Azure code signing..."
& $nuget restore "packages.config" -PackagesDirectory $buildTools

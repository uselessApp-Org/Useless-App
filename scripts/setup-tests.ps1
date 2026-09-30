$ErrorActionPreference = 'Stop'
Set-Location (Join-Path $PSScriptRoot '..')
function Find-DotNet9 {
    if (Get-Command dotnet -ErrorAction SilentlyContinue) {
        return [bool]((dotnet --list-sdks) -match '^9\.')
    }
    return $false
}
if (-not (Find-DotNet9)) {
    if (-not (Get-Command winget -ErrorAction SilentlyContinue)) {
        throw 'Install the .NET 9 SDK from https://dotnet.microsoft.com/download/dotnet/9.0 then rerun this script.'
    }
    winget install --id Microsoft.DotNet.SDK.9 --exact --accept-package-agreements --accept-source-agreements
    if ($LASTEXITCODE -ne 0) { throw 'The SDK installation failed.' }
    $env:Path = [Environment]::GetEnvironmentVariable('Path', 'Machine') + ';' + [Environment]::GetEnvironmentVariable('Path', 'User')
    if (-not (Find-DotNet9)) { throw 'Restart PowerShell and rerun this script so it can find the SDK.' }
}
dotnet restore UselessApp.sln
if ($LASTEXITCODE -ne 0) { throw 'Package restore failed.' }
dotnet test UselessApp.sln --configuration Release
if ($LASTEXITCODE -ne 0) { throw 'Tests failed. See output above.' }
Write-Host 'Dependencies installed and all tests passed.'

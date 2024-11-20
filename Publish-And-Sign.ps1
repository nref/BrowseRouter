param (
    [string]$ExeName,
    [string]$CertPath
)

$certKey = $env:ENDURABYTE_WINDOWS_CODE_SIGN_KEY
$signTool = "signtool.exe"
$rids = @("win-x64", "win-arm64")

# dotnet does not support publishing multiple RIDs in parallel
foreach ($rid in $rids) {
    dotnet publish /p:PublishProfile=$rid --configuration Release
    . $signTool sign /f "$CertPath" /fd SHA256 /td SHA256 /tr http://timestamp.digicert.com `
        /csp "SafeNet Smart Card Key Storage Provider" /k $certKey /v "./publish/$rid/$ExeName"
}
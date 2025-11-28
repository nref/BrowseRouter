& ./Get-BuildTools.ps1

$signTool = "C:\BuildTools\Microsoft.Windows.SDK.BuildTools.10.0.26100.7175\bin\10.0.26100.0\x64\signtool.exe"
$dlib = "C:\BuildTools\Microsoft.Trusted.Signing.Client.1.0.95\bin\x64\Azure.CodeSigning.Dlib.dll"
$rids = @("win-x64", "win-arm64")

pushd "BrowseRouter"

# dotnet does not support publishing multiple RIDs in parallel
foreach ($rid in $rids) {
    dotnet publish /p:PublishProfile=$rid --configuration Release
}

popd

foreach ($rid in $rids) {
    & $signTool sign /v /debug /fd SHA256 /td SHA256 /tr http://timestamp.acs.microsoft.com /dlib $dlib /dmdf "AzureTrustedSigning.json" "./BrowseRouter/publish/$rid/BrowseRouter.exe"
}


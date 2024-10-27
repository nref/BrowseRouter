$certTmpPath = "cert.tmp.crt"
$certKey = $env:ENDURABYTE_WINDOWS_CODE_SIGN_KEY
$signTool = "C:\Program Files (x86)\Windows Kits\10\App Certification Kit\signtool.exe"

echo $certKey
echo $env:ENDURABYTE_WINDOWS_CODE_SIGN_KEY

dotnet publish /p:PublishProfile=arm64 --configuration Release
dotnet publish /p:PublishProfile=x64 --configuration Release

& ./Decode-FromBase64.ps1 $env:ENDURABYTE_WINDOWS_CODE_SIGN_CERTIFICATE $certTmpPath
$certTmpPath = "$PSScriptRoot/$certTmpPath"
echo "Created $certTmpPath..."

. $signtool sign /f "$certTmpPath" /fd SHA256 /td SHA256 /tr http://timestamp.digicert.com /csp "SafeNet Smart Card Key Storage Provider" /k $certKey /v "./publish/win-x64/BrowseRouter.exe"

. $signtool sign /f "$certTmpPath" /fd SHA256 /td SHA256 /tr http://timestamp.digicert.com /csp "SafeNet Smart Card Key Storage Provider" /k $certKey /v "./publish/win-arm64/BrowseRouter.exe"

rm $certTmpPath

md ./publish/signed/win-arm64 -ea 0
cp ./publish/win-arm64/BrowseRouter.exe  ./publish/signed/win-arm64/BrowseRouter.exe
cp ./publish/win-arm64/config.ini  ./publish/signed/win-arm64

md ./publish/signed/win-x64 -ea 0
cp ./publish/win-x64/BrowseRouter.exe  ./publish/signed/win-x64/BrowseRouter.exe
cp ./publish/win-x64/config.ini  ./publish/signed/win-x64

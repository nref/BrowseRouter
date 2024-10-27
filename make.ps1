$jobs = @()
$apps = @("BrowseRouter", "Unregister", "Register")
$rids = @("win-x64", "win-arm64")

$version = & ./Get-ProjectVersion.ps1 BrowseRouter

$certTmpPath = "cert.tmp.crt"

& ./Decode-FromBase64.ps1 $env:ENDURABYTE_WINDOWS_CODE_SIGN_CERTIFICATE $certTmpPath
$certTmpPath = "$PSScriptRoot/$certTmpPath"

foreach ($app in $apps) {
    echo "Packing $app..."
    $job = Start-ThreadJob {
        # Create thread-local copies of these variables
        $certTmpPath = $using:certTmpPath
        $app = $using:app
        $PSScriptRoot = $using:PSScriptRoot

        & {
            pushd $app
            & ../Publish-And-Sign.ps1 "$app.exe" $certTmpPath
            popd

            if ($LASTEXITCODE -ne 0) {
                exit $LASTEXITCODE
            }

        # Redirect all output (including stdout and stderr) to the main thread
        # so it appears in the console and in CI build logs
        } *>&1 | ForEach-Object { "[$app] $_" } | Out-Host

    } -StreamingHost $Host

    # Add the job to the list
    $jobs += $job
}

# Wait for all jobs to complete
Wait-Job -Job $jobs

rm $certTmpPath

# Create release zip files
foreach ($rid in $rids) {
  $outDir = "./Releases/$version/$rid"
  md $outDir -ea 0
  cp ./BrowseRouter/logo.ico $outDir
  cp ./BrowseRouter/publish/$rid/BrowseRouter.exe  $outDir
  cp ./BrowseRouter/publish/$rid/config.ini  $outDir
  cp ./Unregister/publish/$rid/Unregister.exe  $outDir
  cp ./Register/publish/$rid/Register.exe  $outDir

  $zipPath = "$outDir/BrowseRouter-$rid.zip"
  Compress-Archive -Path "$outDir/*" -DestinationPath $zipPath -Force

  echo "Listing release files ($rid):"
  ls "$outDir/*.zip"
}

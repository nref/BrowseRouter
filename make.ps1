$rids = @("win-x64", "win-arm64")
$project = "BrowseRouter"

& ./Publish-And-Sign.ps1

if ($LASTEXITCODE -ne 0) {
  exit $LASTEXITCODE
}

$version = & ./Get-ProjectVersion.ps1 $project

# Create release zip files
foreach ($rid in $rids) {
  $outDir = "./Releases/$version/$rid"
  md $outDir -ea 0
  cp ./$project/publish/$rid/BrowseRouter.exe  $outDir
  cp ./$project/publish/$rid/config.json  $outDir
  cp ./$project/publish/$rid/filters.json  $outDir

  $zipPath = "$outDir/BrowseRouter-$rid.zip"
  Compress-Archive -Path "$outDir/*" -DestinationPath $zipPath -Force
}

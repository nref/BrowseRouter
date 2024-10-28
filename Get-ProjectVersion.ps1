param (
    [string]$ProjectPath
)

$output = dotnet msbuild $ProjectPath -target:GetVersion -nologo -q
$version = $output.Trim()

return $version

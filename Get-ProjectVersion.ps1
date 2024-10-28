param (
    [string]$ProjectPath
)

$output = dotnet msbuild $ProjectPath -target:GetVersion -nologo -q
$version = $output.Trim()
echo "Project version for $ProjectPath is $version"

return $version

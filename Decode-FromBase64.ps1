$base64String = $args[0]
$bytes = [System.Convert]::FromBase64String($base64String)
$outPath = "$PSScriptRoot/$($args[1])"
[System.IO.File]::WriteAllBytes($outPath, $bytes)

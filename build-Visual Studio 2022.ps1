Get-ChildItem -Path .\ -Filter *.sln -Recurse -File | ForEach-Object {
    $solution = $_.FullName
}
cmd /c "`"C:\Program Files\Microsoft Visual Studio\2022\Preview\MSBuild\Current\Bin\MsBuild.exe`" `"$solution`" /t:Restore"
cmd /c "`"C:\Program Files\Microsoft Visual Studio\2022\Preview\MSBuild\Current\Bin\MsBuild.exe`" `"$solution`" /t:Rebuild"
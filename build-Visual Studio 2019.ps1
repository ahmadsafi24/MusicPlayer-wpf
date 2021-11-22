Get-ChildItem -Path .\ -Filter *.sln -Recurse -File | ForEach-Object {
    $solution = $_.FullName
}
cmd /c "`"C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MsBuild.exe`" `"$solution`" /t:Restore"
cmd /c "`"C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MsBuild.exe`" `"$solution`" /t:Rebuild"
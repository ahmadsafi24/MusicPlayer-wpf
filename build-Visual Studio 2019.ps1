#Place me in the same folder as the sln file

#Get the path of the sln file
Get-ChildItem -Path .\ -Filter *.sln -Recurse -File | ForEach-Object {
    $solution = $_.FullName
}
#If you don't have Visual Studio Professional update the path to use a different edition i.e Community or Enterprise. Make sure MsBuild.exe is located there
cmd /c "`"C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MsBuild.exe`" `"$solution`" /t:Restore"
cmd /c "`"C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MsBuild.exe`" `"$solution`" /t:Rebuild"
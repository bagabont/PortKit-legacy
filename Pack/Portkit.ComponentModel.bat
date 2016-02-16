@ECHO off
SET project_dir="..\Source\Portkit.ComponentModel.UWP\Portkit.ComponentModel.UWP.csproj"
CALL NuGet.Pack.bat %project_dir% "nuspec\Portkit.ComponentModel.nuspec"

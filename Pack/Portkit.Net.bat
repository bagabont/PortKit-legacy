@ECHO off
SET project_dir="..\Source\Portkit.Net.UWP\Portkit.Net.UWP.csproj"
CALL NuGet.Pack.bat %project_dir% "nuspec\Portkit.Net.nuspec"
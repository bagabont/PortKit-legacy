@ECHO off
SET project_dir="..\Source\Portkit.Authorization.UWP\Portkit.Authorization.UWP.csproj"
CALL NuGet.Pack.bat %project_dir% "nuspec\Portkit.Authorization.nuspec"
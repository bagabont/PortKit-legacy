@ECHO off
SET project_dir="..\Source\Portkit.Time.UWP\Portkit.Time.UWP.csproj"
CALL NuGet.Pack.bat %project_dir% "nuspec\Portkit.Time.nuspec"
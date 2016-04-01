@ECHO off
SET project_dir="..\Source\Portkit.sln"
CALL NuGet.Pack.bat %project_dir% "nuspec\Portkit.Net.nuspec"
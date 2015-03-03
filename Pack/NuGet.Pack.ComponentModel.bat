@ECHO off
SET project_dir="..\Source\Portkit.ComponentModel\Portkit.ComponentModel.csproj"
CALL NuGet.Pack.bat %project_dir%

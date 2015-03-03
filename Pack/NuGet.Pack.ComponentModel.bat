@echo off
set /p version= Enter package version:

.nuget\nuget.exe pack ..\Source\Portkit.ComponentModel\Portkit.ComponentModel.csproj -prop configuration=release -Version %version% -Build

PAUSE

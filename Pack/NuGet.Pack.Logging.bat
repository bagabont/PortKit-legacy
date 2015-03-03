@echo off
set /p version= Enter package version:

.nuget\nuget.exe pack ..\Source\Portkit.Logging\Portkit.Logging.csproj -prop configuration=release -Version %version% -Build

PAUSE

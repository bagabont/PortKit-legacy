@echo off
set /p version= Enter package version:

.nuget\nuget.exe pack ..\Source\Portkit.Core\Portkit.Core.csproj -prop configuration=release -Version %version% -Build
PAUSE

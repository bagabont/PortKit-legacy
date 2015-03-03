@ECHO off
SET project_dir=%1
SET /p version=Enter package version: 

ECHO Building ...
CALL %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe %project_dir% /m /clp:ErrorsOnly /t:Clean,Build /property:Configuration=Release

ECHO Packing ...
.nuget\nuget.exe pack %project_dir% -prop configuration=release -Version %version% -Build

PAUSE

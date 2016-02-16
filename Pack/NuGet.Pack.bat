@ECHO off
SET project_dir=%1
SET spec_file=%2

SET /p version=Enter package version: 

ECHO Building ...
CALL "C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" %project_dir% /m /clp:ErrorsOnly /t:Clean,Build /property:Configuration=Release

ECHO Packing ...
.nuget\nuget.exe pack %spec_file% -prop configuration=release -Version %version% -Build

PAUSE

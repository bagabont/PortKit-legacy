@ECHO OFF
ECHO Building...
CALL %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe ..\Source\Portkit.sln /m /clp:ErrorsOnly /t:Clean,Build /p:Configuration=Release /p:Platform="Any CPU" 
PAUSE

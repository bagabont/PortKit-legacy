.nuget\nuget.exe pack ..\Source\Portkit.Core\Portkit.Core.csproj -prop configuration=release
.nuget\nuget.exe pack ..\Source\Portkit.ComponentModel\Portkit.ComponentModel.csproj -prop configuration=release
.nuget\nuget.exe pack ..\Source\Portkit.Logging\Portkit.Logging.csproj -prop configuration=release

PAUSE

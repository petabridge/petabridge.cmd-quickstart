@echo off

SET DIR=%~dp0%

SETLOCAL
SET CACHED_NUGET=%LocalAppData%\NuGet\NuGet.exe

IF EXIST %CACHED_NUGET% goto copynuget
echo Downloading latest version of NuGet.exe...
IF NOT EXIST %LocalAppData%\NuGet md %LocalAppData%\NuGet
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://www.nuget.org/nuget.exe' -OutFile '%CACHED_NUGET%'"

:copynuget
IF EXIST src\.nuget\nuget.exe goto restore
md src\.nuget
copy %CACHED_NUGET% src\.nuget\nuget.exe > nul

:restore

src\.nuget\NuGet.exe update -self

call src\.nuget\NuGet.exe restore "src/Petabridge.Cmd.QuickStart.sln"

call msbuild "src/Petabridge.Cmd.QuickStart.sln" /p:Configuration=Release

call src\Petabridge.Cmd.QuickStart\bin\Release\Petabridge.Cmd.QuickStart.exe
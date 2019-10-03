@echo off

call dotnet build -c Release

call dotnet run -c Release --project src\Petabridge.Cmd.QuickStart\Petabridge.Cmd.QuickStart.csproj
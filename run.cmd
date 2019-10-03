@echo off

call dotnet build -c Release

call dotnet run -c Release src\Petabridge.Cmd.QuickStart.csproj
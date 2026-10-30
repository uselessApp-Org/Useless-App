# Useless-App

A Blazor app of deliberately unhelpful tools, Arcade games, and customizable themes. Built with .NET 9.

## Setup

Run commands from the repository folder containing UselessApp.sln. Install the .NET 9 SDK if needed:

```powershell
winget install Microsoft.DotNet.SDK.9
```

Restore and build:

```powershell
dotnet --version
dotnet restore UselessApp.sln
dotnet build UselessApp.sln --no-restore
```

The HTTP profile uses http://localhost:5238/. Keep the terminal open and press **Ctrl+C** to stop.

## Configure and run

Use the team-provided MongoDB database name and provider settings. Store credentials in user secrets rather than committed configuration files. Replace the placeholders with real values:

```powershell
dotnet user-secrets set "MongoDB:ConnectionString" "<team MongoDB connection string>" --project UselessApp
dotnet user-secrets set "Google:ClientId" "<Google client ID>" --project UselessApp
dotnet user-secrets set "Google:ClientSecret" "<Google client secret>" --project UselessApp
dotnet run --project UselessApp --launch-profile http --no-build
```

This starts the app with account features and initializes database indexes. MongoDB must be reachable. In MongoDB Atlas, add your current development IP to the project IP access list. Update that entry if your public IP changes.

## External services

AI, dictionary, and live weather need their own service configuration and network access. For Gemini:

```powershell
dotnet user-secrets set "Gemini:ApiKey" "<Gemini API key>" --project UselessApp
```

## Rebuild and test

Stop the running app with **Ctrl+C**, then run:

```powershell
dotnet clean UselessApp.sln
dotnet restore UselessApp.sln
dotnet build UselessApp.sln --no-restore
dotnet test UselessApp.sln --no-build --no-restore
```

Restart using the run command above. For automatic rebuilds:

```powershell
dotnet watch --project UselessApp --launch-profile http

```

## Common startup problems

- **Address already in use:** stop the other instance on port 5238 before starting another.
- **Executable is locked during build:** stop the running app, then rebuild.
- **Empty/invalid connection string:** configure your MongoDB user secrets and confirm your development IP is allowed in Atlas.
- **Old styling or controls:** rebuild, restart, then refresh with Ctrl+F5.
- **Missing API key:** configure the relevant external service.

## Contributing

Branch from current main using `UA-<ticket>-<description>`. Use the pull-request template and request the team's two approvals before merging.

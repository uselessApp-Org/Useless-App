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

Choose either run option below. Both use http://localhost:5238/. Keep the terminal open and press **Ctrl+C** to stop.

## Run with a database

Use the team-provided MongoDB database name and provider settings. Store credentials in user secrets rather than committed configuration files. Replace the placeholders with real values:

```powershell
dotnet user-secrets set "MongoDB:ConnectionString" "<team MongoDB connection string>" --project UselessApp
dotnet user-secrets set "Google:ClientId" "<Google client ID>" --project UselessApp
dotnet user-secrets set "Google:ClientSecret" "<Google client secret>" --project UselessApp
dotnet run --project UselessApp --launch-profile http --no-build
```

This starts the app with account features and initializes database indexes. MongoDB must be reachable.

## Run without a database

```powershell
dotnet run --project UselessApp --launch-profile preview --no-build
```

This Development-only profile runs local tools, games, and themes without MongoDB. Sign-in and signup are unavailable. Saved database credentials are not changed.

## External services

AI, dictionary, and live weather need their own service configuration and network access in either run mode. For Gemini:

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

Restart using either run command above. For automatic rebuilds, choose the matching command:

```powershell
# With a database
dotnet watch --project UselessApp --launch-profile http

# Without a database
dotnet watch --project UselessApp --launch-profile preview
```

## Common startup problems

- **Address already in use:** stop the other instance on port 5238 before starting another.
- **Executable is locked during build:** stop the running app, then rebuild.
- **Empty/invalid connection string:** configure real credentials for `http`, or use `preview`.
- **Old styling or controls:** rebuild, restart, then refresh with Ctrl+F5.
- **Missing API key:** configure the relevant external service.

## Contributing

Branch from current main using `UA-<ticket>-<description>`. Use the pull-request template and request the team's two approvals before merging.

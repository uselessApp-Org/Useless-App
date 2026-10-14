# UselessApp

A deliberately playful ASP.NET Core 9 / Blazor Server app with a calculator, unit converter, trivia, To-Don't list, timer, theme selector, and optional connected services.

**Start here:** [SETUP-AND-TESTING.md](SETUP-AND-TESTING.md) for local setup, tests, optional MongoDB and provider connections, and deployment cautions.

Quick start:

```powershell
dotnet restore UselessApp.sln
dotnet build UselessApp.sln
dotnet run --project UselessApp --launch-profile http
```

Tests: `dotnet test UselessApp.sln`. Most tools run without a database or API key. External integrations must be configured separately.

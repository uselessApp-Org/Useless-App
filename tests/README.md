# Tests

Run from the repository root with the .NET 9 SDK installed:

```powershell
dotnet test UselessApp.sln
```

Stop a running development server before rebuilding if Windows reports locked files. The tests use local logic, mocks, component rendering, and an in-memory HTTP server; they do not require real account or API credentials.

| Project | Coverage |
| --- | --- |
| `UselessApp.UnitTests` | Calculator parsing and accuracy, catalog integrity, and shuffled question decks |
| `UselessApp.ComponentsTests` | Original calculator buttons, keypads, and component interactions |
| `UselessApp.IntegrationTests` | Mocked account creation, password checks, and versioned stylesheet/script responses |

To run one project:

```powershell
dotnet test tests/UselessApp.UnitTests/UselessApp.UnitTests.csproj
```

Browser JavaScript, visual layout, and real external-service access require separate browser checks. The obsolete commented-out route, page, and experiment-provider suites have been removed.

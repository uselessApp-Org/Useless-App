# UselessApp — local setup, tests, and optional integrations

## Requirements

- .NET **9 SDK**, including ASP.NET Core. `dotnet --list-sdks` must list 9.0.x.
- Visual Studio 2022 with ASP.NET workload or VS Code + C# Dev Kit.
- MongoDB is **optional** for the main tools; external APIs are **optional**.

## Get running locally (Windows PowerShell)

From the directory containing `UselessApp.sln`:

```powershell
dotnet restore UselessApp.sln
dotnet build UselessApp.sln
dotnet run --project UselessApp --launch-profile http
```

Open the URL printed in your terminal (typically `http://localhost:5238`). If Windows Firewall asks, permit localhost access. HTTPS: `dotnet dev-certs https --trust` then `dotnet run --project UselessApp --launch-profile https`.

## Automated tests

```powershell
dotnet test UselessApp.sln --configuration Debug
```

Run individual suites:

```powershell
dotnet test tests/UselessApp.UnitTests/UselessApp.UnitTests.csproj
dotnet test tests/UselessApp.ComponentsTests/UselessApp.ComponentsTests.csproj
dotnet test tests/UselessApp.IntegrationTests/UselessApp.IntegrationTests.csproj
```

The added unit tests check calculator arithmetic, error handling, and conversions. The server integration tests use an in-memory ASP.NET Core test server and check page rendering and `/health/live`. Existing provider-contract tests use a fake HTTP handler; they **do not require an actual provider or a MongoDB server**.

For an actual hosted-server smoke test, start the application in another terminal and execute `./scripts/smoke-test.ps1 -BaseUrl http://localhost:5238`.

## Features and limitations

| Feature | Offline/local | External setup |
| --- | --- | --- |
| Calculator | Arithmetic, parentheses, scientific functions (degrees) | None |
| Unit converter | Accurate conversions in six categories | None |
| Timer | Countdown with pause/reset | None |
| Trivia | Multiple-choice questions and score | None |
| To-Don't | Add, check, remove, clear (current browser session) | Optional future persistence |
| Theme / app pins | Saved in browser localStorage | None |
| Translator | Joke translations in selected target languages | Provider for genuine translation |
| Directions, recipes, music | Clearly fictional suggestions | Provider for external operations |
| Weather | Built-in sample data | Weather provider for live readings |
| Dictionary | Key required | Merriam-Webster dictionary |
| Add user | Requires DB | MongoDB (not a complete auth flow) |

To-Don't list items and trivia score currently reset after a full page reload. Timer is tied to the server-side Blazor circuit and may stop if the page disconnects. The joke apps do not create real routes, playlists, or recipe searches until you configure adapters. **Do not use fictional driving directions for navigation.**

## Configure secrets locally

From `UselessApp/`:

```powershell
dotnet user-secrets init
dotnet user-secrets set "DictionaryApi:ApiKey" "YOUR_MERRIAM_WEBSTER_API_KEY"
dotnet user-secrets set "MongoDB:ConnectionString" "mongodb://localhost:27017"
dotnet user-secrets set "MongoDB:DatabaseName" "UselessApp"
```

For MongoDB Atlas, use its `mongodb+srv://...` connection string instead, create a database user, and restrict network access. Never commit secrets to Git. The user-record UI is a prototype and **does not provide production authentication**; credentials are BCrypt-hashed, but public account creation needs access controls, rate limiting, and a complete session/authentication implementation before deployment.

### Generic connected provider contract

For an experimental app, set e.g.:

```powershell
dotnet user-secrets set "ToolProviders:translator:Endpoint" "https://your-adapter.example/api/translator"
dotnet user-secrets set "ToolProviders:translator:ApiKey" "YOUR_ADAPTER_BEARER_TOKEN"
```

Supported IDs: `converter`, `translator`, `directions`, `timer`, `recipes`, `trivia`, `music`, `todo`. Only configure endpoints for services you intend to use. Endpoint must use HTTPS; HTTP is rejected. The server POSTs JSON:

```json
{"tool":"translator","input":"hello","options":{"sourceLanguage":"en","targetLanguage":"fr"},"instruction":"Return a clearly fictional, comically incorrect answer. Avoid actionable dangerous advice."}
```

Respond with HTTP 200 and a JSON object such as `{"result":"Your result here"}`. An API such as Google Translate **does not natively implement this JSON contract**. Create a server-side adapter that converts this request into the vendor API request and translates the response back. Do not place vendor API keys in browser JavaScript. Other tools can use separate adapter endpoints. A vendor account alone will not automatically connect the feature. For genuine translations, adjust the adapter's instruction/semantics accordingly; the existing generic experiment instruction specifically requests a fictional response.

## Deployment reminders

For a hosted server, provide user secrets through managed environment variables/secret stores (e.g. `ToolProviders__translator__Endpoint`). Configure HTTPS, reverse proxy/WebSocket support for interactive server components, logging, input validation, authentication and access controls on account creation, server-side rate limiting, and monitoring. `GET /health/live` checks that HTTP hosting is alive; **it does not confirm external provider, database, or end-to-end readiness**.

## Accessibility and visual review

Controls use theme-aware foreground/background tokens, form option colors, placeholders, and focus outlines. The CSS includes dark-theme overrides and small-screen layouts. These are stylesheet-level mitigations, **not** a verified WCAG audit. Manually inspect all ten themes on each page (especially hover, disabled buttons, cards, and scientific keypad) and run browser DevTools Accessibility/Lighthouse or axe. There is no browser-driven screenshot regression suite in the archive.

## Troubleshooting

- `dotnet` not found: install the **SDK** and restart PowerShell.
- Restore errors: make sure internet/NuGet is available for the first restore.
- Database unavailable: local tools still work; do not submit user credentials until MongoDB is configured.
- Provider unavailable: choose Demo. Confirm HTTPS adapter URL, bearer secret, and `{ "result": "..." }` JSON response.
- Stale UI: stop the app, run `dotnet clean`, rebuild, hard-refresh the browser.

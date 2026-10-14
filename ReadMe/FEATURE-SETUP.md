# Run and connect UselessApp

These are setup instructions for the team. They are not displayed in the app.

## 1. Extract the project

Extract Useless-App-restyled.zip. Open the Useless-App-main folder in VS Code or Visual Studio. It contains UselessApp.sln, the UselessApp folder, and tests.

## 2. Install .NET 9

Install the .NET 9 SDK, not just the runtime. If using Visual Studio, install the ASP.NET and web development workload with .NET 9 support.

Open a new PowerShell terminal and check:

```powershell
dotnet --list-sdks
```

There should be a 9.0.xxx entry. If dotnet is not recognized, restart your terminal after installation.

## 3. Initialize local configuration

From the extracted repository root:

```powershell
cd UselessApp
dotnet user-secrets init
```

Run all the user-secrets commands below from this UselessApp directory. User secrets are loaded in Development and keep local keys out of committed appsettings files.

## 4. Connect MongoDB

The home page uses MongoDB to add user records. Choose one option.

### Local MongoDB

Install MongoDB Community Server and ensure its service is running. Then set:

```powershell
dotnet user-secrets set "MongoDB:ConnectionString" "mongodb://localhost:27017"
dotnet user-secrets set "MongoDB:DatabaseName" "UselessApp"
```

### MongoDB Atlas

Create an Atlas cluster, create a database user, allow your computer's IP in Network Access, and copy the application's connection string. Replace the placeholders with the actual values:

```powershell
dotnet user-secrets set "MongoDB:ConnectionString" "mongodb+srv://YOUR_USER:YOUR_PASSWORD@YOUR_CLUSTER_HOST/?retryWrites=true&w=majority"
dotnet user-secrets set "MongoDB:DatabaseName" "UselessApp"
```

Use the connection string Atlas provides. Reserved characters in the username or password must be URL-encoded. The users collection is created on the first successful insert. This page creates records; it is not a finished authentication system. Use test credentials.

## 5. Connect the dictionary

Get a Merriam-Webster Collegiate Dictionary API key from its developer service, then configure:

```powershell
dotnet user-secrets set "DictionaryApi:ApiKey" "YOUR_DICTIONARY_KEY"
```

The key is now read from server configuration, rather than hardcoded in APISearch.razor. The dictionary page calls the existing Collegiate endpoint and displays its response. It does not yet transform real definitions into wrong ones.

## 6. Run the app

Return to the repository root:

```powershell
cd ..
dotnet restore UselessApp.sln
dotnet build UselessApp.sln
dotnet dev-certs https --trust
dotnet run --project UselessApp --launch-profile https
```

Open https://localhost:7067, or the URL printed by the command. Keep the terminal open while using the app. Press Ctrl+C to stop it.

If you prefer HTTP for local testing:

```powershell
dotnet run --project UselessApp --launch-profile http
```

Open http://localhost:5238. If it redirects to HTTPS, use the HTTPS profile and trust the development certificate.

## 7. Check what works without another provider

- Ten themes, with Light and Dark labels intentionally swapped.
- Search, pin/unpin, pinned-only filter, compact view, Surprise me.
- Theme and pins persist in the same browser.
- All eight Other apps pages have local joke demos.
- Converter: amount, category, matching from/to units, swap control.
- Translator: text, source/target languages, swap control.
- Existing counter and random sample weather.

The calculator is still a keypad prototype. Experiment demos return text only: the timer does not run a countdown, music does not create a playlist, and directions are fictional.

## 8. Connect an Other apps experiment

This requires a real provider endpoint or a wrapper written by your team. Supplying an API key alone does not connect a vendor. The included adapter expects a particular JSON request/response; vendor APIs with different schemas need a wrapper or a replacement service implementation.

Choose a tool ID:

| App | ID |
| --- | --- |
| Unit Confuser | converter |
| Lost in Translation | translator |
| Wrong Way | directions |
| Eventually | timer |
| Recipe for Disaster | recipes |
| Definitely Incorrect | trivia |
| Wrong Vibes | music |
| To-Don't | todo |

For example, configure the translator from the UselessApp directory:

```powershell
dotnet user-secrets set "ToolProviders:translator:Endpoint" "https://YOUR_SERVER/translate"
dotnet user-secrets set "ToolProviders:translator:ApiKey" "YOUR_PROVIDER_KEY"
```

Replace the URL and key with actual values. The adapter requires HTTPS. The key is optional for endpoints that do not require authentication. Repeat with a different ID for each tool you want to connect.

Restart the app, open that tool, and choose Connected provider in Answer source. It defaults to Demo. Configured means the endpoint has been supplied, not that reachability or credentials have been tested.

### What your endpoint receives

The server sends a POST with Content-Type: application/json. When an API key is configured it also sends Authorization: Bearer YOUR_PROVIDER_KEY.

Translator example:

```json
{
  "tool": "translator",
  "input": "Hello there",
  "options": {"sourceLanguage": "en", "targetLanguage": "es"},
  "instruction": "Return a clearly fictional, comically incorrect answer. Avoid actionable dangerous advice."
}
```

Converter example:

```json
{
  "tool": "converter",
  "input": "10 Meters to Feet",
  "options": {"category": "Length", "amount": "10", "fromUnit": "Meters", "toUnit": "Feet"},
  "instruction": "Return a clearly fictional, comically incorrect answer. Avoid actionable dangerous advice."
}
```

Other tools currently send their text input with options set to null.

### What your endpoint must return

```json
{"result":"Your deliberately wrong answer goes here."}
```

The result must be a JSON string. Requests time out after 20 seconds. Errors are shown without exposing credentials.

Language codes: en, es, fr, de, it, pt, ja, ko, zh, ar, hi, ru. Source can also be auto; target cannot. Auto-detect is a request to your provider, not a feature of the local demo.

## 9. Where to implement connections

| File | Responsibility |
| --- | --- |
| UselessApp/Services/ExperimentService.cs | Shared provider adapter, IExperimentService interface, local demo responses |
| UselessApp/Services/ToolOptions.cs | Converter units and translator languages |
| UselessApp/Services/ToolCatalog.cs | Tool metadata and routes |
| UselessApp/Services/MongodbUser.cs | MongoDB operations |
| UselessApp/Components/Pages/APISearch.razor | Existing dictionary request |
| UselessApp/Program.cs | Service registration and HttpClient timeout |

Add vendor SDKs and connection logic in a service file, then inject that service into the page. Keep credentials in server configuration. The experiment page only gathers input and displays results; its connection setup panel has been removed.

For deployment, configure environment variables instead of local user secrets:

```text
MongoDB__ConnectionString=YOUR_CONNECTION_STRING
MongoDB__DatabaseName=UselessApp
DictionaryApi__ApiKey=YOUR_KEY
ToolProviders__translator__Endpoint=https://YOUR_SERVER/translate
ToolProviders__translator__ApiKey=YOUR_KEY
```

## 10. Run tests

From the repository root:

```powershell
dotnet test tests/UselessApp.UnitTests
dotnet test tests/UselessApp.ComponentsTests
dotnet test tests/UselessApp.IntegrationTests
```

Check the app in a phone-sized browser window too. The editing environment has no .NET SDK or browser binary, so the build, C# tests, and browser checks have not been run here.

## Common problems

| Problem | Check |
| --- | --- |
| dotnet not found | Install the SDK, then restart the terminal. |
| Home page stalls or Add User fails | MongoDB service/cluster, connection string, database user permissions, and Atlas IP access. |
| Dictionary not configured | Set DictionaryApi:ApiKey in the UselessApp project and restart in Development. |
| Connected provider option missing | Endpoint must be configured under the exact tool ID and use HTTPS; restart. |
| Connection error | Endpoint URL, credentials, server logs, and the provider's expected request schema. |
| Response error | Return valid JSON with a string result property. |
| Pins or theme do not persist | Browser storage may be blocked or cleared; settings belong to that browser and origin. |

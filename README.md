# Useless-App
Welcome to Useless App we hope you find **no real information** while using our application!

## Versioning Practices
1. Fetch then Pull main into your local repo
2. Create a new branch in the following format from main(usually):
   * UA-<##>-Few-Word-Description
   * replace <##> with the number of your ticket from Jira
3. Do your work
4. Create a pull-request on Github or through VS Code or GitHub Desktop.
   * Fill out the PR Template
5. Notify team of your pull request so it can gain its two required approvals

## Setting up the Environment
1. Ensure you have .Net 9.0 installed
   * `dotnet --version`
   * If you don't have the correct version run
   * `winget install Microsoft.DotNet.SDK.9`
2. Create the solution file if you need to
   * `dotnet new sln --name UselessApp --format sln`
3. Ensure you have the required NuGet Dependencies
   * `dotnet restore UselessApp/`
4. Build the project
   * `dotnet build UselessApp/`
5. Run the project with live updates
   * `dotnet watch --project UselessApp/`

## Database Set-up (should not need this)

 'dotnet add package MongoDB.Driver'
 Create an account on MongoDB so you can access the database and see if the connection went through. 

## Database Notes

 Database may not connect with the connection string,we are currently working on that, if you need to connect to the database let Caleb know
 
 In the appsettings.json file, there is a connection string. DO NOT change this, if that gets changed you will not be able to connect to the database without the connection string changed back.
## Setting up Gemini API
1. Getting API key
   * Go to https://aistudio.google.com/api-keys
   * Click create API Key
   * pick a coneveastion to run it out of
2. Using API Key
   * Add the API key to the user secrets
3. Add service to call API Key
   * See example in Services/GeminiService
4. Adding Blazor Component
   * See example of model and Blozor component in Components/Pages/AskAI.razor

## Quick start and detailed setup

This guide runs Useless-App on your Windows computer. Start with the copyable commands, then use the detailed sections if you need help getting credentials or connecting a service.

### Part 1: Copyable commands

#### Before pasting

1. Open the project folder in VS Code.
2. Choose **Terminal > New Terminal** and select **PowerShell**.
3. Obtain the MongoDB connection string, Google OAuth client ID, and Google OAuth client secret from the team. For AI and Dictionary, obtain their API keys too.
4. Add your current public IP to the team’s Atlas project as explained in Part 2. The app requires MongoDB during startup.

Paste the contents of each code block, without the surrounding Markdown fences. Commands beginning with `#` are comments.

#### A. Install the SDK if needed

```powershell
dotnet --list-sdks
```

Look for a `9.0.xxx` entry. If it is missing, run this command, follow the installer, and reopen your terminal:

```powershell
winget install Microsoft.DotNet.SDK.9
```

#### B. First-time setup and run

You can paste this whole block. It asks for your project folder and credentials instead of requiring you to edit placeholders. Credentials are hidden while you type or paste them; press Enter after each value. For an optional key you do not have, press Enter to skip it. Do not paste JSON or `key = value` lines into a prompt; paste only the requested value.

If the secrets are already configured, use section C instead. Repeating section B replaces values you enter but keeps optional secrets you skip.

```powershell
# Enter the folder containing UselessApp.sln, without surrounding quotation marks.
$projectFolder = Read-Host 'Project folder (example: C:\Projects\Useless-App-GitHub\Useless-App)'
Set-Location -LiteralPath $projectFolder -ErrorAction Stop
if (-not (Test-Path -LiteralPath 'UselessApp.sln')) {
    throw 'Wrong folder: UselessApp.sln was not found.'
}

# This helper hides each credential while it is entered.
function Read-SetupSecret {
    param([string]$Prompt)
    $secureValue = Read-Host $Prompt -AsSecureString
    return [System.Net.NetworkCredential]::new('', $secureValue).Password
}

$mongoConnection = Read-SetupSecret 'MongoDB connection string (required)'
$googleClientId = Read-Host 'Google OAuth client ID (required)'
$googleClientSecret = Read-SetupSecret 'Google OAuth client secret (required)'
if ([string]::IsNullOrWhiteSpace($mongoConnection) -or
    [string]::IsNullOrWhiteSpace($googleClientId) -or
    [string]::IsNullOrWhiteSpace($googleClientSecret)) {
    throw 'MongoDB and Google OAuth values are required. Obtain them from the team, then rerun this block.'
}
$mongoDatabase = Read-Host 'MongoDB database name (press Enter for Users)'
if ([string]::IsNullOrWhiteSpace($mongoDatabase)) { $mongoDatabase = 'Users' }
$geminiKey = Read-SetupSecret 'Gemini API key (optional; Enter to skip)'
$dictionaryKey = Read-SetupSecret 'Collegiate Dictionary API key (optional; Enter to skip)'

$setupSecrets = @{
    'MongoDB:DatabaseName' = $mongoDatabase
    'MongoDB:ConnectionString' = $mongoConnection
    'Google:ClientId' = $googleClientId
    'Google:ClientSecret' = $googleClientSecret
}
if (-not [string]::IsNullOrWhiteSpace($geminiKey)) { $setupSecrets['Gemini:ApiKey'] = $geminiKey }
if (-not [string]::IsNullOrWhiteSpace($dictionaryKey)) { $setupSecrets['DictionaryAPI:ApiKey'] = $dictionaryKey }

# Store settings outside the repository, using piped JSON.
$setupSecrets | ConvertTo-Json -Compress | dotnet user-secrets set --project UselessApp
if ($LASTEXITCODE -ne 0) { throw 'Saving user secrets failed.' }

# Remove credential variables from this terminal session.
Remove-Variable setupSecrets,mongoConnection,googleClientId,googleClientSecret,geminiKey,dictionaryKey,secureValue -ErrorAction SilentlyContinue

dotnet restore UselessApp.sln
if ($LASTEXITCODE -ne 0) { throw 'Restore failed. Read the error above.' }
dotnet build UselessApp.sln --no-restore
if ($LASTEXITCODE -ne 0) { throw 'Build failed. Read the error above.' }
dotnet run --project UselessApp --launch-profile http --no-build
```

Open **http://localhost:5238/** after the terminal says **Now listening on**. Keep that terminal open. Press **Ctrl+C** there to stop the server.

#### C. Start again after setup

Run from the repository folder. This does not ask for secrets again:

```powershell
dotnet run --project UselessApp --launch-profile http
```

For automatic rebuilds when you save source files:

```powershell
dotnet watch --project UselessApp --launch-profile http
```

Use only one of these run commands at a time.

#### D. Full rebuild and all tests

Stop the running app with **Ctrl+C**, then paste this block from the repository folder:

```powershell
dotnet clean UselessApp.sln
if ($LASTEXITCODE -ne 0) { throw 'Clean failed.' }
dotnet restore UselessApp.sln
if ($LASTEXITCODE -ne 0) { throw 'Restore failed.' }
dotnet build UselessApp.sln --no-restore
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
dotnet test UselessApp.sln --no-build --no-restore
if ($LASTEXITCODE -ne 0) { throw 'Tests failed. Review their output before continuing.' }
dotnet run --project UselessApp --launch-profile http --no-build
```

### Part 2: What each part does

| Part | Purpose | Connection requirements |
|---|---|---|
| .NET SDK | Restores packages, compiles C#/Razor, runs the server and tests. | .NET 9 SDK; internet for uncached packages. |
| Blazor server | Renders pages and processes interactive button events. | Keep the server terminal open; the browser connects to localhost. |
| MongoDB Atlas | Hosts account records; startup creates the username index. | Reachable cluster, database user, correct database name, and allowed public IP. |
| Google sign-in | Redirects users to Google and handles the sign-in callback. | Google OAuth web client ID/secret and matching redirect URI. |
| Ask AI | Sends prompts to Gemini. | Gemini API key and access to the model configured in GeminiService.cs. |
| API Search / Dictionary | Uses Merriam-Webster or Urban Dictionary; deliberately searches for a random word rather than defining the exact input. | Collegiate Dictionary key for Merriam-Webster. Urban Dictionary has no API-key setting in this app, but the page also injects MerriamAPI, so configure the Dictionary key to avoid its missing-key constructor error. |
| Weather | Finds city coordinates with Open-Meteo and requests weather.gov data. | Internet access; no API-key setting in these services. weather.gov forecast coverage is primarily U.S. locations. |
| Counter and original Calculator | Local interactive tools; the original calculator intentionally scrambles operators. | No feature-specific API key. Normal app startup still requires the account configuration above. |
| Arcade | Local games and joke tools, including an accurate evaluator used by the Arcade calculator. | No additional external credentials for the local games. |
| Themes, pins, custom colors | Customize appearance and save browser preferences. | Browser local storage; no API key. Clearing site data removes saved preferences. |

#### What the commands mean

- `dotnet restore` downloads the dependencies listed in the project files.
- `dotnet build` compiles the solution and reports code errors.
- `dotnet test` runs the unit, component, and integration test projects. Passing these tests does not prove real external credentials work; some tests use mocks.
- `dotnet run` starts the server. The `http` launch profile sets Development mode and the localhost URL.
- `--no-restore` reuses restored dependencies; `--no-build` reuses the most recent build. Do not use `--no-build` after source changes unless you have rebuilt.
- `dotnet watch` rebuilds/reloads during development.
- `dotnet clean` removes normal build outputs; it does not remove your user secrets.

### Part 3: User secrets explained

User secrets are local development settings stored outside the repository. A key such as `MongoDB:ConnectionString` identifies a setting; the value is your actual connection string. The colon represents a configuration section.

This project already has a `UserSecretsId` in `UselessApp/UselessApp.csproj`; you do not need to run `dotnet user-secrets init`. The Development launch profiles load user secrets automatically. They are not an encrypted production vault. [Microsoft user-secrets documentation](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-9.0)

| Secret name | What to enter |
|---|---|
| `MongoDB:DatabaseName` | `Users`, unless the team specifies another database. |
| `MongoDB:ConnectionString` | The full Atlas driver connection string with database-user credentials. |
| `Google:ClientId` | The OAuth web application client ID, usually ending in `.apps.googleusercontent.com`. |
| `Google:ClientSecret` | The secret from that same OAuth web client. |
| `Gemini:ApiKey` | The Gemini API key from Google AI Studio; separate from Google OAuth credentials. |
| `DictionaryAPI:ApiKey` | A Merriam-Webster Collegiate Dictionary API key. |

To update one value, replace the placeholder before running this example:

```powershell
dotnet user-secrets set 'MongoDB:DatabaseName' 'Users' --project UselessApp
```

To view stored settings:

```powershell
dotnet user-secrets list --project UselessApp
```

That command prints actual credentials. Do not paste its output into a PR, issue, or chat. Restart the app after changing secrets.

On Windows, the file is normally `%APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json`. Do not put credentials in README, launchSettings.json, or committed appsettings files. Files in `bin/` and `obj/` are generated; edit source files rather than their generated copies.

### Part 4: MongoDB Atlas account and connection setup

#### A. Join the team project or create your own

For the shared UselessApp database, ask its owner to invite your Atlas account to the correct project. Creating a separate Atlas account or project does not automatically grant access to the team’s cluster.

If you intentionally want an independent development database, register at [MongoDB Atlas](https://www.mongodb.com/cloud/atlas/register), create a project, and create a Free cluster. Your new cluster starts empty and does not include the team’s accounts.

#### B. Create or obtain a database user

An **Atlas account** signs into the Atlas website. A **database user** is the username/password in the app’s connection string. They are different accounts. [Atlas database-user documentation](https://www.mongodb.com/docs/atlas/security-add-mongodb-users/)

1. Select the correct Atlas project and open its database access settings (under Security; the UI may group Database and Network Access together).
2. Use the database user supplied by the team, or ask an owner to create an application-specific database user.
3. Give that user read/write access to the intended database, normally `Users`. The app also needs permission to create its username index; MongoDB’s built-in `readWrite` role on that database supports index creation.
4. Save the database username and password. They are not your Atlas website login.

#### C. Allow your development IP

1. Open the project’s **Network Access / IP Access List**.
2. Choose **Add IP Address**, then **Add Current IP Address**.
3. Add a useful description and save. Wait until the entry is active.
4. If your public IP changes, you move networks, or a VPN changes your outgoing IP, update the entry. A home connection can still have a dynamic public IP.

Use your public outgoing IP, not a private address such as `192.168.x.x`. Atlas only permits connecting addresses in the project’s access list. [Atlas connection setup](https://www.mongodb.com/docs/atlas/connect-to-database-deployment/)

#### D. Get the connection string

1. On the cluster, choose **Connect > Drivers**.
2. Select **C#/.NET** and an appropriate driver version.
3. Copy Atlas’s connection string and replace its username/password placeholders with the database user’s credentials.
4. Keep the actual hostname and required query options that Atlas provides. Use the database name supplied by the team.
5. If the username or password includes reserved URI characters, encode that credential component; do not encode the entire connection string.
6. Enter the completed string into the setup prompt in Part 1. Do not commit it.

The shape is `mongodb+srv://USERNAME:PASSWORD@CLUSTER.mongodb.net/Users?appName=UselessApp`; this is an illustration, not a usable connection string. [Atlas driver guide](https://www.mongodb.com/docs/atlas/driver-connection/)

#### E. Verify access

Ensure the cluster is running and the network allows MongoDB traffic, normally TCP port 27017. Start the app and look for **Now listening on**. Startup calls `EnsureIndexes()` before opening the local server, so a database failure can prevent the website from starting.

You can also test the same connection using MongoDB Compass. If it fails there too, investigate Atlas access, network/TLS, and credentials before changing app code. A TLS handshake failure happens before database authentication and does not prove that the password is incorrect.

### Part 5: Google sign-in setup

For the team’s existing Google OAuth client, ask the owner for the client ID/secret and confirm that it supports your localhost URL. If creating your own:

1. Open [Google Cloud Console](https://console.cloud.google.com/) and choose a project.
2. Configure the OAuth consent screen / Google Auth Platform settings. If the app is in testing, add your Google account as a test user when required.
3. Create an **OAuth client ID** with application type **Web application**.
4. Add the authorized redirect URI matching the profile you use:

```text
http://localhost:5238/signin-google
https://localhost:7067/signin-google
```

The `/signin-google` callback is handled by ASP.NET Core middleware; it is not a Razor page you need to create. Scheme, hostname, port, and path must match the actual callback. Register both only if you use both profiles. [Google login setup for ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-9.0)

5. Save the values as `Google:ClientId` and `Google:ClientSecret` through the setup prompts.
6. Restart and try the Google sign-in action. The app’s challenge endpoint is `/auth/login`.

For HTTPS development, trust the local certificate, then run the HTTPS profile:

```powershell
dotnet dev-certs https --trust
dotnet run --project UselessApp --launch-profile https
```

Open **https://localhost:7067/** for that profile. A Google OAuth secret is not a Gemini API key.

### Part 6: Connect the AI and Dictionary tools

#### Gemini / Ask AI

1. Obtain the team’s approved key or create a key in [Google AI Studio](https://aistudio.google.com/api-keys).
2. Save it as `Gemini:ApiKey` using Part 1’s prompt or the JSON approach below.
3. Ensure its Google Cloud project can use the model configured in `Services/GeminiService.cs`; quotas, model availability, or billing requirements can affect requests.
4. Restart, open Ask AI, and submit a short prompt.

Gemini keys and OAuth client credentials serve different purposes. [Gemini key documentation](https://ai.google.dev/gemini-api/docs/api-key)

#### Merriam-Webster Dictionary

1. Obtain the team’s key or [register for a Merriam-Webster developer account](https://www.dictionaryapi.com/register/index.htm).
2. Request a **Collegiate Dictionary** key, which matches this app’s `/references/collegiate/json/` endpoint.
3. Save it as `DictionaryAPI:ApiKey` and restart.
4. Open API Search, choose Merriam-Webster, enter a word, and select Search.

The app intentionally looks up a randomly chosen word. A valid key does not rule out a request or JSON-handling bug. The Urban Dictionary option uses its public random endpoint and has no separate key setting in this app. [Merriam-Webster API setup](https://www.dictionaryapi.com/)

#### Weather and local tools

Weather uses Open-Meteo to find coordinates and weather.gov for forecast details. No additional secret is read by those services. Try a U.S. city because weather.gov is not a worldwide forecast service.

Counter, Calculator, Arcade, and themes need no additional keys. This does not remove the application-wide MongoDB and Google configuration requirements.

#### Optional experiment-provider adapter

The `/lab/{id}` experiments use local demos unless a supported provider is configured and Connected mode is selected. This is separate from the local Arcade routes.

For an experiment ID whose `ToolCatalog.Apps` entry has no route, the adapter reads `ToolProviders:<id>:Endpoint` and optionally `ToolProviders:<id>:ApiKey`. The endpoint must be HTTPS. It receives a JSON POST containing `tool`, `input`, `options`, and `instruction`; the API key is sent as a Bearer token. It expects a response with a `text` property. See `Services/ExperimentService.cs` before connecting an endpoint; a vendor URL will not necessarily support that schema.

### Part 7: Alternative JSON secret setup

If you prefer editing JSON, open your user-secrets file through your IDE’s Manage User Secrets option or its Windows path from Part 3. Merge these keys with existing settings. Replace every placeholder before saving; this is not a working configuration as written:

```json
{
  "MongoDB:DatabaseName": "Users",
  "MongoDB:ConnectionString": "<complete Atlas connection string>",
  "Google:ClientId": "<OAuth web client ID>",
  "Google:ClientSecret": "<OAuth web client secret>",
  "Gemini:ApiKey": "<Gemini API key>",
  "DictionaryAPI:ApiKey": "<Collegiate Dictionary API key>"
}
```

Do not replace unrelated existing secrets, add this file to Git, or edit the generated settings under `bin/`. Restart after saving.

### Part 8: Troubleshooting checklist

| Problem | What to check |
|---|---|
| `dotnet` is not recognized | Install the .NET 9 SDK and reopen the terminal. |
| Project or solution not found | Run `Get-Location` and `Test-Path .\UselessApp.sln`; move to the repository folder. |
| Empty or invalid MongoDB string | Check the secret name and remove literal placeholders; use a complete Atlas driver URI. |
| MongoDB timeout | Confirm the cluster is active, current public IP is allowed, hostname resolves, and port 27017 is reachable. |
| TLS / Local Security Authority error | Investigate the encrypted connection, local network/security software, and Atlas access. Compare with Compass; do not assume it means a bad database password. |
| Database authentication or permission error | Use the database user, not the Atlas website account; verify credentials, database scope, and index privileges. |
| Google `redirect_uri_mismatch` | Register the exact callback scheme, host, port, and `/signin-google` path being used. |
| Missing or rejected API key | Confirm the secret name, provider/product, project restrictions, quota, and response details; restart after changes. |
| Address already in use | Stop the other app instance on port 5238; do not start multiple copies on the same port. |
| Build says files are locked | Stop the running server before a full rebuild. |
| Styles or icon look old | Rebuild, restart, and press Ctrl+F5. |
| A feature fails but the page loads | Check server output and browser errors. Tests alone do not verify live provider access. |

See Versioning Practices above for the contribution workflow. This guide changes documentation only; it does not change login, database, or tool behavior.

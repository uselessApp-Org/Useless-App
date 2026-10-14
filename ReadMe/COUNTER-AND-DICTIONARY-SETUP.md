# Counter fix and dictionary feature

## Install this update

This archive contains the entire project. **Back up any local changes** and replace your extracted project folder with this version (or copy the changed files into your own project). Stop the running app with Ctrl+C, then run:

```powershell
dotnet restore UselessApp.sln
dotnet build UselessApp.sln
dotnet run --project UselessApp --launch-profile http
```

Open http://localhost:5238/counter for the new Counter page. It supports increment, decrement, and reset with per-session state. If you previously had the app open, refresh your browser.

Open http://localhost:5238/api-search-term for the dictionary. The default provider is https://dictionaryapi.dev/ (Free Dictionary API), which uses no API key. The application server needs outbound Internet access to make these requests. Enter a word such as `hello` and choose **Search**. Missing words, remote HTTP errors, timeouts, and unexpected responses show helpful messages.

### Optional Merriam-Webster key

If you have a Merriam-Webster Collegiate Dictionary API key, store it as a .NET user secret, rather than committing credentials:

```powershell
cd UselessApp
dotnet user-secrets init
dotnet user-secrets set "DictionaryApi:ApiKey" "YOUR_KEY_HERE"
cd ..
```

Restart the app. When `DictionaryApi:ApiKey` is set, the dictionary uses Merriam-Webster's API instead. To return to the no-key provider:

```powershell
cd UselessApp
dotnet user-secrets remove "DictionaryApi:ApiKey"
cd ..
```

## Tests

```powershell
dotnet test UselessApp.sln
```

The new counter component test verifies increment/decrement/reset. Integration route tests include `/counter` and `/api-search-term`. Tests do **not** make live dictionary calls, so they will pass without API credentials or network access.

## Known limits

- The Counter is per browser circuit, not persisted across browser reloads.
- A dictionary search requires Internet access from the server. Local-only use without Internet requires a downloaded dictionary dataset, which is not bundled.
- External service availability and live API responses are not checked by the automated tests.
- The .NET SDK is not installed in the artifact-building environment; compile and test with the commands above on your PC.

# UselessApp Arcade v2 – Audio crash fix

This build is based on the **Arcade Local v2 ZIP**. It embeds the tiny `absurdArcade` sound helper in `Components/App.razor` ahead of Blazor startup, and handles JavaScript audio errors without crashing the interactive server circuit. Bubble Wrap calls the same safe helper as the soundboard.

## Install (Windows PowerShell)

1. Stop the running server with **Ctrl+C**.
2. Back up your current folder (especially if you have edited its source).
3. Extract the patched ZIP into a new location; do not mix its files with those of the distinct Combined Update edition.
4. In the extracted `Useless-App-main` folder run:

```powershell
dotnet restore UselessApp.sln
dotnet build UselessApp.sln
dotnet test UselessApp.sln
dotnet run --project UselessApp --launch-profile http
```

5. Open http://localhost:5238/arcade/bubbles and press Ctrl+Shift+R to hard-refresh if you previously loaded the old script. Try a bubble and then http://localhost:5238/arcade/soundboard.

## MongoDB warning

The MongoDB Atlas TLS message in your log is **independent** of the audio fix. It reports `The Local Security Authority cannot be contacted` during TLS handshake, not a proven wrong MongoDB username/password. Check Windows time and updates, TLS inspection, VPN/proxy/firewall, and Atlas network access. Keep credentials out of logs and source control. MongoDB-dependent account features may be unavailable until the connection is repaired.

## Verification

The archive and source edits have been inspected, but the .NET SDK is unavailable in this build environment. Run the build and tests on your machine. This fix specifically targets Arcade v2's missing JavaScript global, not a full merge of the separate Combined Update build.

```powershell
# Requires the .NET 9 SDK. Run from the folder containing UselessApp.sln.
# Configure hosted Atlas and Google OAuth in user secrets before running.
# The project already has a UserSecretsId. Keep existing unrelated secret values.
# Example (replace the value before uncommenting):
# dotnet user-secrets set "MongoDB:ConnectionString" "YOUR_COMPLETE_ATLAS_CONNECTION_STRING" --project UselessApp
# dotnet user-secrets set "MongoDB:DatabaseName" "Users" --project UselessApp
# dotnet user-secrets set "Google:ClientId" "YOUR_GOOGLE_CLIENT_ID" --project UselessApp
# dotnet user-secrets set "Google:ClientSecret" "YOUR_GOOGLE_CLIENT_SECRET" --project UselessApp

dotnet restore UselessApp.sln
if ($LASTEXITCODE -ne 0) { throw 'Restore failed.' }
dotnet build UselessApp.sln --no-restore
if ($LASTEXITCODE -ne 0) { throw 'Build failed. Stop any running app before rebuilding.' }
dotnet run --project UselessApp/UselessApp.csproj --no-build --launch-profile http
# Open http://localhost:5238. Press Ctrl+C to stop.
```

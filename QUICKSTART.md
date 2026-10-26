```powershell
# Requires the .NET 9 SDK. Run from the repository root.
# Browsing apps works without MongoDB. Signup/login require valid database credentials.
# To configure accounts, uncomment these commands and replace the connection string:
# dotnet user-secrets init --project UselessApp/UselessApp.csproj
# dotnet user-secrets set "MongoDB:ConnectionString" "YOUR_MONGODB_CONNECTION_STRING" --project UselessApp/UselessApp.csproj

dotnet restore UselessApp.sln
dotnet build UselessApp.sln --no-restore
dotnet run --project UselessApp/UselessApp.csproj --no-build --launch-profile http
# Open http://localhost:5238. Press Ctrl+C to stop.
```

# App design update

See FEATURE-SETUP.md for the current feature list, provider setup, and validation instructions.

Run with the .NET 9 SDK:

```sh
dotnet restore UselessApp.sln
dotnet run --project UselessApp
```

Open the localhost URL printed by the command. User creation requires your configured MongoDB connection. The calculator remains a keypad prototype, and weather uses random sample data. Existing tool behavior is retained. The eight new experiments offer local joke demos and an optional server-side provider adapter. Light and dark theme labels are intentionally swapped.

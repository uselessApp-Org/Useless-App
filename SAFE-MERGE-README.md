# Safe merge onto known-working project (Useless-App-main(3).zip)

This archive uses your uploaded working project as the source of truth.

Changes: add two Arcade Razor pages, Arcade service and JS assets; replace only the homepage app catalog/shelf, the theme selector, browser preferences, and theme CSS; add two script tags to App.razor. Remove obsolete tests for retired experiments and add Arcade registration tests.

**Preserved byte-for-byte**: Program.cs, Routes.razor, Home.razor, Calculator, Counter, API Search, Dictionary, Weather, Login, MongoDB service, other original pages, and their original services.

This does NOT inherit routing/hosting modifications from previous diagnostic builds. `/` remains the working base home route.

Extract to a NEW folder, then run: dotnet restore UselessApp.sln ; dotnet build UselessApp.sln ; dotnet test UselessApp.sln ; dotnet run --project UselessApp --launch-profile http

NOTE: Full dotnet compilation and browser test were not available in this environment; syntax/route/static archive checks performed. If browser goes blank, inspect the *server console* for prerender exceptions, as errors may not appear in DevTools.

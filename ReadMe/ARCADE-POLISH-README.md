# UselessApp Arcade — UI & gameplay polish

This update builds on **UselessApp-Arcade-v2-Full-Requested-Update.zip**.

## What's changed
- Ridiculous Measurements starts empty, with amount placeholder and selectable unit fields. Clicking a unit opens a searchable list under that field; no browser datalist autofill. Source selection determines compatible target options.
- Calculator: instantly resets its state when changing modes; Catch Me position resets on difficulty changes; slot machine uses red/green result, lever movement, and 1-in-3 win probability; boss fight adds Easy/Normal/Hard/Nightmare and multiple-choice or typed-answer modes.
- Translator: additional local slang dictionary, with approximate reverse lookup. These are playful substitutions, not a full AI translator.
- Trip Fortune no longer produces a trip excuse; fictional weather stays separate.
- Bubble Wrap is 5 × 5.
- Home app cards now group arcade tools in Other apps; removed Classic Unit Converter, Eventually, Classic To-Don’t and Wrong Vibes cards. Sidebar only lists Home, Counter, Weather, Calculator and API Search.

## Local startup (PowerShell)
```powershell
dotnet restore UselessApp.sln
dotnet build UselessApp.sln
dotnet test UselessApp.sln
dotnet run --project UselessApp --launch-profile http
```
Open the address printed in your terminal (usually http://localhost:5238). Hard refresh with Ctrl+Shift+R if CSS seems cached.

## Verification
This environment does not contain the .NET SDK. Archive integrity and static checks were performed, but a complete build, browser interactivity check, and test run need to be executed on your Windows machine.

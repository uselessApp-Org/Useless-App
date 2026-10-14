# Useless Arcade — local implementation

## Run

```powershell
dotnet restore UselessApp.sln
dotnet build UselessApp.sln
dotnet test UselessApp.sln
dotnet run --project UselessApp --launch-profile http
```

Visit the home page and open **Useless Arcade**, or visit `/arcade/converter`, `/arcade/calculator`, `/arcade/counter`, `/arcade/todo`, `/arcade/translator`, `/arcade/travel`, `/arcade/soundboard`, `/arcade/rock`, `/arcade/bubbles`.

No API keys required; Web Audio works after a user click. The pre-existing database/account features still require your MongoDB configuration.

## Reference units

Length stored in inches; weight stored in kilograms. Conversion is source amount × source factor ÷ target factor. Banana reference is 7.5 in for medium banana; edible mass 118 g, with-peel reference 180 g. A giraffe is 18 feet tall and reference mass 1360 kg. Bald eagle weight reference 12 lb and wingspan 7 ft. Blue whale reference mass 136000 kg and adult male lion representative 190 kg. Nonstandard objects vary significantly; these are deliberately labeled approximations rather than universal constants.

Sources: USDA banana portions via https://www.ars.usda.gov/ARSUserFiles/80400530/pdf/7778/cfe_admin_rep_352.pdf ; Smithsonian giraffe https://movementoflife.si.edu/species/giraffe/ ; U.S. Fish & Wildlife bald eagle https://www.fws.gov/sites/default/files/documents/bald-eagle-fact-sheet.pdf . Other rough size references (spaghetti, refrigerator, bus, lion, whale) are demo assumptions and should be verified before claiming scientific accuracy.

## Design choices / limitations

- Translator uses local phrase substitutions and supports Gen Z -> English as a separate mode; it does not attempt full semantic translation.
- Calculator uses the existing correct arithmetic parser; slot mode reveals the correct answer with a randomized display, and dodge mode includes an accessible alternative.
- Sound effects use browser-generated oscillator tones; no external API or bundled copyrighted audio.
- Game state is in-memory per interactive server circuit and resets on full reload; this is a playable local prototype, **not persistent account storage**.
- Converter bureaucracy is intentionally optional.
- Do not use the fictional travel prophecy or excuses as real guidance.

## Tests

`tests/UselessApp.UnitTests/AbsurdFeatureTests.cs` exercises unit conversions, incompatible dimensions, two-way slang and pirate speech. Run `dotnet test` locally. Do manual browser checks in light and dark modes (including focus indicators, keyboard use, and mobile viewport).

## Optional next-step integrations

Keep all external API keys in server-side configuration (user secrets or environment variables), never in `wwwroot` or JavaScript. Add provider interfaces and an `HttpClient` registered via dependency injection. Do not send free-form user content to external APIs without clearly identifying the feature as connected. For persistence, a local-storage JS adapter or database-backed per-user service can be introduced later.

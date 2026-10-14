# UselessApp Arcade — Combined Feature Update

This archive is based on **UselessApp-Arcade-v2-Audio-Fix.zip**, incorporating the requested Arcade upgrade together with the corrected audio bootstrap. The original source and tests remain in place.

## Local Windows run

From the extracted `Useless-App-main` directory, run:

```powershell
dotnet restore UselessApp.sln
dotnet build UselessApp.sln
dotnet test UselessApp.sln
dotnet run --project UselessApp --launch-profile http
```

Open the address shown in your console (typically http://localhost:5238), then hard-refresh your browser with Ctrl+Shift+R. Stop the previous app before launching the new copy.

## Implemented

- Sentence-style searchable converter with **321 length/mass units**. Each unit has a reference value and a note, with automatic source-category detection, random compatible targets, alphabetical/size sorting, optional bureaucracy approval, and an object visualization. The figures for organic and manufactured objects are approximations; literal physical dimension and weight should not be conflated. The first-party scientific units use defined conversion factors. Some enormous values are rough illustrative scales, not surveying-grade numbers.
- Calculator with Catch Me difficulty settings, randomly moving equals button, slot-machine game with a 1/3 random win, correct arithmetic, and a multi-boss math battle.
- Counter with deeper milestones, 5-second and 10-second click tests, and rolling one-second peak tracking (sampled as clicks arrive); per-page-visit records.
- Trick trivia with 24 varied prompts, each with three category-fitting options and one subtle/absurd outlier. Option locations shuffle. Scores tracked for the visit.
- Trip Fortune page renamed and expanded with an **independent fictional weather forecast**, explicitly unrelated to real conditions.
- Distinct Web Audio synthesized effects for clap-like applause, noise-based explosions, record scratches, drum rolls, boings, and other soundboard keys. The JS file loads before Blazor.
- Extra generated excuses, Gen Z substitutions, travel prophecies, and consistent styling/navigation. The home ToolShelf displays Arcade features as first-class cards and retires the overlapping Lost in Translation, Wrong Way, and Recipe for Disaster demo cards. Classic versions of the calculator, converter, counter, and To-Don’t remain available.
- Dictionary API search remains at `/api-search-term`, with error handling for missing words, network failures, JSON issues, and a default public dictionary API with no key. Live external responses were not testable in this environment.

## Known limitations / checks

- **This environment has no .NET SDK**, so a complete compile, automated test run, and browser-level UI test have not been performed. `absurd-arcade.js` passed Node's JavaScript syntax check. Please run the commands above locally and provide any compiler/test output if a specific issue is found.
- Click speed duration is finalized when another click occurs after a test's expiration; it does not live-count down between click events. Freestyle rolling one-second peak is sampled on click events.
- Counter results, quiz scores, and pet rock state are session-state prototypes, not synced server profiles.
- Slang translation is curated phrase substitution, not an AI model or nuanced grammatical translation.
- The calculator's animations use CSS; reduced-motion preferences disable them. The escaping button is intentionally difficult to click, though accessible alternative controls are still desirable for a public deployment.
- MongoDB Atlas login failures are unrelated to the local Arcade games. Use correct server credentials and trusted TLS/network configuration; do not disable certificate validation.

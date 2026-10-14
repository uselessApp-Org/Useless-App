# Bubble Wrap and custom theme update

## Included changes
- 5×5 raised, theme-aware bubbles, including highlights, shadows, hover and popped states.
- Point economy: base 1 point per pop; five consecutive pops increase the multiplier, capped at ×3. Completing a 25-bubble sheet awards 10 bonus points and prepares a fresh sheet. Gold bubbles cost 30 points and double the base reward; double-points upgrade costs 50 points and doubles it again. Costs are deducted from available points; lifetime earned is tracked separately. A manual new sheet resets the combo.
- No extra navigation checkbox: sidebar retains its main links.
- Custom theme accent using a native color picker; selection is kept in localStorage and survives refreshes; `Reset color` restores the selected theme's original colors. Button text is selected for contrast against the custom accent.

## Run locally (PowerShell)
```powershell
dotnet restore UselessApp.sln
dotnet build UselessApp.sln
dotnet test UselessApp.sln
dotnet run --project UselessApp --launch-profile http
```

Open the address in your terminal, usually http://localhost:5238, and select Bubble Wrap under Other Apps. Hard refresh with Ctrl+Shift+R if the theme picker or styles appear stale.

## Verification
Static JavaScript syntax check passed. Archive validation passed. .NET SDK isn't installed in the packaging environment, so compile, automated tests, and browser behavior must be verified locally.

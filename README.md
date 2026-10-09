# Useless-App
Welcome to Useless App we hope you find **no real information** while using our application!

## Versioning Practices
1. Fetch then Pull main into your local repo
2. Create a new branch in the following format from main(usually):
   * UA-<##>-Few-Word-Description
   * replace <##> with the number of your ticket from Jira
3. Do your work
4. Create a pull-request on Github or through VS Code or GitHub Desktop.
   * Fill out the PR Template
5. Notify team of your pull request so it can gain its two required approvals

## Setting up the Environment
1. Ensure you have .Net 9.0 installed
   * `dotnet --version`
   * If you don't have the correct version run
   * `winget install Microsoft.DotNet.SDK.9`
2. Ensure you have the required NuGet Dependencies
   * `dotnet restore`
3. Build the project
   * `dotnet build`
4. Run the project with live updates
   * `dotnet watch --project UselessApp/`
## Setting Gemini API
1. Get API Key
   * `dotnet --version`
   * If you don't have the correct version run
   * `winget install Microsoft.DotNet.SDK.9`
2. Use API Key
   * `dotnet restore`
3. Using in Gemini in code
   * `dotnet build`
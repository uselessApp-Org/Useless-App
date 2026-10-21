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
2. Create the solution file if you need to
   * `dotnet new sln --name UselessApp --format sln`
3. Ensure you have the required NuGet Dependencies
   * `dotnet restore UselessApp/`
4. Build the project
   * `dotnet build UselessApp/`
5. Run the project with live updates
   * `dotnet watch --project UselessApp/`

## Database Set-up (should not need this)

 'dotnet add package MongoDB.Driver'
 Create an account on MongoDB so you can access the database and see if the connection went through. 

## Database Notes

 Database may not connect with the connection string,we are currently working on that, if you need to connect to the database let Caleb know
 
 In the appsettings.json file, there is a connection string. DO NOT change this, if that gets changed you will not be able to connect to the database without the connection string changed back.
## Setting up Gemini API
1. Getting API key
   * Go to https://aistudio.google.com/api-keys
   * Click create API Key
   * pick a coneveastion to run it out of
2. Using API Key
   * Add the API key to the user secrets
3. Add service to call API Key
   * See example in Services/GeminiService
4. Adding Blazor Component
   * See example of model and Blozor component in Components/Pages/AskAI.razor
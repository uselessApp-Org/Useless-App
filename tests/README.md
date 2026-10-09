# UselessApp Tests

Tests check that our C# code and Blazor pages work as expected. Keep tests in the same repository as the app, inside the `tests` folder.

## First-time setup

Open a terminal in the main folder containing `UselessApp.sln`. On Windows, run:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\setup-tests.ps1
```

This installs the .NET 9 SDK if needed, downloads the test packages, and runs all tests. You may see an installation permission prompt. The setup script is included in the updated project ZIP.

## Run the tests

From the same main folder:

```powershell
dotnet test UselessApp.sln
```

You do not need to start the website first.

- **Passed:** The result matched what the test expected.
- **Failed:** Read the test name and expected/actual results in the terminal. Check the code or update the test if the intended behavior changed.

To run just the Blazor tests:

```powershell
dotnet test tests/UselessApp.ComponentsTests
```

## What each project tests

| Folder inside `tests` | What it checks | Example |
| --- | --- | --- |
| `UselessApp.UnitTests` | C# logic using xUnit | Celsius-to-Fahrenheit conversion |
| `UselessApp.ComponentsTests` | Blazor components using bUnit and xUnit | Clicking the counter button updates the count |
| `UselessApp.IntegrationTests` | The running ASP.NET app using an in-memory test server | `/counter` returns a successful response |

The current suite has **19 test cases**. It covers the current template features. Add tests when you add new features. Component tests simulate interactions; they do not open a real browser or run browser JavaScript.

## Example: Test C# logic

Create a `.cs` file in `UselessApp.UnitTests`:

```csharp
using UselessApp.Models;
using Xunit;

public class TemperatureExampleTests
{
    [Fact]
    public void ZeroCelsius_Returns32Fahrenheit()
    {
        // Arrange: prepare the input.
        var forecast = new WeatherForecast { TemperatureC = 0 };

        // Act: run the code being tested.
        var result = forecast.TemperatureF;

        // Assert: check the result.
        Assert.Equal(32, result);
    }
}
```

This uses the app's actual `WeatherForecast` model. The existing conversion truncates decimals instead of rounding; the current tests preserve that behavior.

## Example: Test a Blazor button

Create a `.cs` file in `UselessApp.ComponentsTests`:

```csharp
using Bunit;
using UselessApp.Components.Pages;
using Xunit;

public class CounterExampleTests : BunitContext
{
    [Fact]
    public void ClickingCounter_ShowsOne()
    {
        // Arrange: render the real Counter component.
        var component = Render<Counter>();

        // Act: click its button.
        component.Find("button").Click();

        // Assert: check what the user sees.
        Assert.Equal(
            "Current count: 1",
            component.Find("[role=status]").TextContent);
    }
}
```

`BunitContext` provides the component testing tools. `Render` creates the component, `Find` selects an HTML element, and `Click` simulates a click.

## Example: Test a web page

Create a `.cs` file in `UselessApp.IntegrationTests`:

```csharp
using System.Net;
using UselessApp.IntegrationTests;
using Xunit;

public class PageExampleTests : IClassFixture<AppFactory>
{
    private readonly AppFactory factory;

    public PageExampleTests(AppFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task CounterPage_ReturnsSuccess()
    {
        using var client = factory.CreateClient(new()
        {
            BaseAddress = new Uri("https://localhost")
        });

        using var response = await client.GetAsync("/counter");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

`AppFactory` is already defined in `RouteTests.cs`. It starts the app inside the test process, so no separate server is needed.

## Adding your own tests

1. Put the test in the project that matches what you are testing.
2. Give the class and test clear names describing the behavior.
3. Prepare the inputs, perform an action, and check the result.
4. Run `dotnet test UselessApp.sln` before submitting your pull request.

Use `[Fact]` for one case. Use `[Theory]` and `[InlineData]` to repeat a test with different inputs:

```csharp
[Theory]
[InlineData(0, 32)]
[InlineData(10, 49)]
public void Temperature_ConvertsCelsius(int celsius, int expected)
{
    var forecast = new WeatherForecast { TemperatureC = celsius };
    Assert.Equal(expected, forecast.TemperatureF);
}
```

Put this method inside a test class with the same imports as the C# example. Test meaningful behavior, such as validation, calculations, button actions, and error handling. Avoid expecting exact random weather values.

## GitHub checks

The included `tests.yml` workflow builds and runs all tests on pull requests and pushes to `main`. Look for **Build and Tests** in the GitHub checks. A repository maintainer can make this a required check before merging.

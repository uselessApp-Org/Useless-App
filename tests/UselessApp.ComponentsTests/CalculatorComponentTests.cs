using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using UselessApp.Components.Calculator;
using UselessApp.Components.Calculator.Components;
using UselessApp.Services;

namespace UselessApp.ComponentsTests;

public class CalculatorComponentTests : BunitContext
{
    #region CalcButton Tests

    [Fact]
    public void CalcButton_RendersValueAsText_WhenNoDisplayTextOrChildContent()
    {
        var cut = Render<CalcButton>(parameters => parameters
            .Add(p => p.Value, "7"));

        var button = cut.Find("button");
        Assert.Equal("7", button.TextContent.Trim());
        Assert.Equal("CalcButton", button.ClassName);
    }

    [Fact]
    public void CalcButton_RendersDisplayText_WhenProvided()
    {
        var cut = Render<CalcButton>(parameters => parameters
            .Add(p => p.Value, "pi")
            .Add(p => p.DisplayText, "\u03c0"));

        var button = cut.Find("button");
        Assert.Equal("\u03c0", button.TextContent.Trim());
    }

    [Fact]
    public void CalcButton_RendersChildContent_WhenProvided()
    {
        var cut = Render<CalcButton>(parameters => parameters
            .Add(p => p.Value, "sin(")
            .AddChildContent("<span>Custom Sin</span>"));

        var button = cut.Find("button");
        Assert.Contains("Custom Sin", button.InnerHtml);
    }

    [Fact]
    public void CalcButton_AppliesCustomStyleClass()
    {
        var cut = Render<CalcButton>(parameters => parameters
            .Add(p => p.Value, "0")
            .Add(p => p.Style, "Zero"));

        var button = cut.Find("button");
        Assert.Contains("CalcButton", button.ClassList);
        Assert.Contains("Zero", button.ClassList);
    }

    [Fact]
    public void CalcButton_InvokesClickedCallback_WithCorrectValue()
    {
        string? clickedValue = null;
        var cut = Render<CalcButton>(parameters => parameters
            .Add(p => p.Value, "5")
            .Add(p => p.Clicked, EventCallback.Factory.Create<string>(this, val => clickedValue = val)));

        cut.Find("button").Click();

        Assert.Equal("5", clickedValue);
    }

    #endregion

    #region OperatorKeypad Tests

    [Fact]
    public void OperatorKeypad_RendersHeaderAndFourOperatorButtons()
    {
        var cut = Render<OperatorKeypad>();

        Assert.NotNull(cut.Find(".OperatorPanel"));
        Assert.Equal("Operators", cut.Find(".PanelHeader span").TextContent.Trim());

        var buttons = cut.FindAll("button");
        Assert.Equal(4, buttons.Count);

        var buttonTexts = buttons.Select(b => b.TextContent.Trim()).ToList();
        Assert.Contains("/", buttonTexts);
        Assert.Contains("*", buttonTexts);
        Assert.Contains("-", buttonTexts);
        Assert.Contains("+", buttonTexts);
    }

    [Theory]
    [InlineData("/", "/")]
    [InlineData("*", "*")]
    [InlineData("-", "-")]
    [InlineData("+", "+")]
    public void OperatorKeypad_ClickingOperatorButton_TriggersButtonClickedCallback(string buttonText, string expectedValue)
    {
        string? receivedValue = null;
        var cut = Render<OperatorKeypad>(parameters => parameters
            .Add(p => p.ButtonClicked, EventCallback.Factory.Create<string>(this, val => receivedValue = val)));

        var button = cut.FindAll("button").First(b => b.TextContent.Trim() == buttonText);
        button.Click();

        Assert.Equal(expectedValue, receivedValue);
    }

    #endregion

    #region ScientificKeypad Tests

    [Fact]
    public void ScientificKeypad_RendersHeaderAndTwelveButtons()
    {
        var cut = Render<ScientificKeypad>();

        Assert.NotNull(cut.Find(".ScientificPanel"));
        Assert.Equal("Scientific", cut.Find(".PanelHeader span").TextContent.Trim());

        var buttons = cut.FindAll("button");
        Assert.Equal(12, buttons.Count);
    }

    [Fact]
    public void ScientificKeypad_RendersSuperscriptElements()
    {
        var cut = Render<ScientificKeypad>();

        // Superscript should be rendered for asin, acos, atan (power -1) and x^y
        var superscripts = cut.FindAll("sup");
        Assert.NotEmpty(superscripts);
        var supTexts = superscripts.Select(s => s.TextContent.Trim()).ToList();
        Assert.Contains("-1", supTexts);
        Assert.Contains("y", supTexts);
    }

    [Fact]
    public void ScientificKeypad_ClickingButton_TriggersButtonClickedCallback()
    {
        string? clickedValue = null;
        var cut = Render<ScientificKeypad>(parameters => parameters
            .Add(p => p.ButtonClicked, EventCallback.Factory.Create<string>(this, val => clickedValue = val)));

        var firstButton = cut.Find("button");
        firstButton.Click();

        Assert.NotNull(clickedValue);
        Assert.NotEmpty(clickedValue);
    }

    #endregion

    #region Calculator Page Integration Tests

    [Fact]
    public void CalculatorPage_RendersDisplayAndSubcomponents()
    {
        Services.AddSingleton<CalculatorEvaluatorService>();

        var cut = Render<Calculator>();

        var display = cut.Find(".Display");
        Assert.Equal("0", display.TextContent.Trim());

        Assert.NotNull(cut.FindComponent<ScientificKeypad>());
        Assert.NotNull(cut.FindComponent<OperatorKeypad>());
        Assert.NotEmpty(cut.FindComponents<CalcButton>());
    }

    [Fact]
    public void CalculatorPage_ClickingDigits_UpdatesDisplay()
    {
        Services.AddSingleton<CalculatorEvaluatorService>();

        var cut = Render<Calculator>();

        // Find a button with text "0"
        var zeroBtn = cut.FindAll("button").First(b => b.TextContent.Trim() == "0");
        zeroBtn.Click();

        var display = cut.Find(".Display");
        Assert.Equal("0", display.TextContent.Trim());
    }

    [Fact]
    public void CalculatorPage_ClearButton_ResetsDisplay()
    {
        Services.AddSingleton<CalculatorEvaluatorService>();

        var cut = Render<Calculator>();

        // Find Clear button
        var clearBtn = cut.FindAll("button").First(b => b.TextContent.Trim() == "CLEAR");
        clearBtn.Click();

        var display = cut.Find(".Display");
        Assert.Equal("0", display.TextContent.Trim());
    }

    #endregion
}


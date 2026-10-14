using Bunit;
using UselessApp.Components.Pages;

namespace UselessApp.ComponentsTests;

public class CounterPageTests : BunitContext
{
    [Fact]
    public void Counter_Increments_Decrements_And_Resets()
    {
        var page = Render<Counter>();
        Assert.Equal("Current count: 0", page.Find("[role=status]").TextContent.Trim());
        var buttons = page.FindAll("button");
        buttons[0].Click();
        buttons[0].Click();
        Assert.Equal("Current count: 2", page.Find("[role=status]").TextContent.Trim());
        buttons[1].Click();
        Assert.Equal("Current count: 1", page.Find("[role=status]").TextContent.Trim());
        buttons[2].Click();
        Assert.Equal("Current count: 0", page.Find("[role=status]").TextContent.Trim());
    }
}

/*using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using UselessApp.Components.Pages;
using UselessApp.Components.Layout;
namespace UselessApp.ComponentsTests;

public class PageTests : BunitContext
{
    [Fact]
    public void Counter_StartsAtZero()
    {
        var component = Render<Counter>();
        Assert.Equal("Current count: 0", component.Find("[role=status]").TextContent);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(10)]
    public void Counter_ClicksIncreaseCount(int clicks)
    {
        var component = Render<Counter>();
        for (var i = 0; i < clicks; i++) component.Find("button").Click();
        Assert.Equal($"Current count: {clicks}", component.Find("[role=status]").TextContent);
    }

    [Fact]
    public void Counter_NewInstanceStartsFresh()
    {
        var first = Render<Counter>();
        first.Find("button").Click();
        var second = Render<Counter>();
        Assert.Equal("Current count: 0", second.Find("[role=status]").TextContent);
    }

    [Fact]
    public void Weather_LoadsFiveForecasts()
    {
        var component = Render<Weather>();
        component.WaitForAssertion(() =>
        {
            Assert.Equal(5, component.FindAll("tbody tr").Count);
            foreach (var row in component.FindAll("tbody tr"))
            {
                var cells = row.QuerySelectorAll("td");
                Assert.Equal(4, cells.Length);
                Assert.InRange(int.Parse(cells[1].TextContent), -20, 54);
                Assert.False(string.IsNullOrWhiteSpace(cells[3].TextContent));
            }
        }, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void NavMenu_CounterLinkIsActiveOnCounterRoute()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/counter");
        var component = Render<NavMenu>();
        Assert.Contains("active", component.Find("a[href='counter']").ClassList);
        Assert.DoesNotContain("active", component.Find("nav a[href='']").ClassList);
        Assert.Equal(5, component.FindAll("nav a").Count);
    }
}
*/
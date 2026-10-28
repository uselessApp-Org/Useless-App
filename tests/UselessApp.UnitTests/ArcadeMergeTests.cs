using UselessApp.Services;

public class ArcadeMergeTests
{
    [Theory]
    [InlineData("arcade-gym", "arcade-extra/gym")]
    [InlineData("arcade-captcha", "arcade-extra/captcha")]
    [InlineData("arcade-rps", "arcade-extra/rps")]
    [InlineData("arcade-platformer", "arcade-extra/platformer")]
    [InlineData("arcade-converter", "arcade/converter")]
    [InlineData("arcade-bubbles", "arcade/bubbles")]
    public void ArcadeApps_AreRegistered(string id, string route)
    {
        Assert.Contains(ToolCatalog.Apps, app => app.Id == id && app.Route == route);
    }

    [Fact]
    public void RemovedCoinFlip_IsNotInCatalog() =>
        Assert.DoesNotContain(ToolCatalog.Apps, app => app.Id == "arcade-coin");

    [Fact]
    public void AppIdsAndRoutes_AreUnique()
    {
        Assert.Equal(ToolCatalog.Apps.Length, ToolCatalog.Apps.Select(a => a.Id).Distinct().Count());
        var routes = ToolCatalog.Apps.Where(a => a.Route is not null).Select(a => a.Route).ToArray();
        Assert.Equal(routes.Length, routes.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    [Fact]
    public void ExistingMainApps_RemainRegistered()
    {
        foreach (var id in new[] { "counter", "calculator", "dictionary", "weather" })
            Assert.Contains(ToolCatalog.Apps, app => app.Id == id);
    }

    [Fact]
    public void Converter_HasManyDistinctUnits()
    {
        Assert.True(AbsurdConversions.Units.Length >= 200);
        Assert.Equal(AbsurdConversions.Units.Length,
            AbsurdConversions.Units.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    [Fact]
    public void SlangDictionary_PreservesUserRequestedTerms() =>
        Assert.Equal("skibidi", PersonalityTranslations.GenZ["cool"]);
}

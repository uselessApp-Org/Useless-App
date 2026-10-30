using UselessApp.Services;

public class CatalogIntegrityTests
{
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
    public void Converter_UnitNamesAreUnique()
    {
        Assert.NotEmpty(AbsurdConversions.Units);
        Assert.Equal(AbsurdConversions.Units.Length,
            AbsurdConversions.Units.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }
}

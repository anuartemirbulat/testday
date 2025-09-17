using RegionModule;

namespace Tests;

public class RegionLayerTests
{
    [Fact]
    public void RegionLayer_GeneratesCorrectly()
    {
        var layer = new RegionLayer(100, 100, 10, 10);
        var regionId = layer.GetRegionId(15, 25);
        var region = layer.GetRegionMeta(regionId);

        Assert.NotNull(region);
        Assert.Equal(regionId, region.Id);
    }

    [Fact]
    public void TileBelongsToRegion_ReturnsTrue()
    {
        var layer = new RegionLayer(50, 50, 10, 10);
        var id = layer.GetRegionId(20, 20);
        Assert.True(layer.TileBelongsToRegion(20, 20, id));
    }

    [Fact]
    public void GetRegionsInArea_ReturnsCorrectSet()
    {
        var layer = new RegionLayer(100, 100, 20, 20);
        var regions = layer.GetRegionsInArea(10, 10, 50, 50).ToList();

        Assert.True(regions.Count == 9);
    }
}
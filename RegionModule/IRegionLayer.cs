namespace RegionModule;

public interface IRegionLayer
{
    ushort GetRegionId(int x, int y);
    Region GetRegionMeta(ushort id);
    bool TileBelongsToRegion(int x, int y, ushort regionId);
    IEnumerable<Region> GetRegionsInArea(int x1, int y1, int x2, int y2);
}
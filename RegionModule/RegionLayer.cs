namespace RegionModule;

public class RegionLayer : IRegionLayer
{
    private readonly Region[] _regions;
    private readonly ushort[,] _regionMap;

    private readonly int _mapWidth;
    private readonly int _mapHeight;
    private readonly int _regionWidth;
    private readonly int _regionHeight;
    public RegionLayer():this(100,100,20,20)
    {
    }

    public RegionLayer(int mapWidth, int mapHeight, int regionWidth, int regionHeight)
    {
        _mapWidth = mapWidth;
        _mapHeight = mapHeight;
        _regionWidth = regionWidth;
        _regionHeight = regionHeight;

        int regionsX = (int)Math.Floor((double)mapWidth / regionWidth);
        int regionsY = (int)Math.Floor((double)mapHeight / regionHeight);
        int totalRegions = regionsX * regionsY;

        _regions = new Region[totalRegions];
        _regionMap = new ushort[mapWidth, mapHeight];

        GenerateRegions(regionsX, regionsY);
    }

    private void GenerateRegions(int regionsX, int regionsY)
    {
        ushort id = 0;
        for (int ry = 0; ry < regionsY; ry++)
        {
            for (int rx = 0; rx < regionsX; rx++)
            {
                string name = $"Регион {id}";
                _regions[id] = new Region { Id = id, Name = name };

                for (int y = ry * _regionHeight; y < Math.Min((ry + 1) * _regionHeight, _mapHeight); y++)
                {
                    for (int x = rx * _regionWidth; x < Math.Min((rx + 1) * _regionWidth, _mapWidth); x++)
                    {
                        _regionMap[x, y] = id;
                    }
                }

                id++;
            }
        }
    }

    public ushort GetRegionId(int x, int y) => _regionMap[x, y];

    public Region GetRegionMeta(ushort id) => _regions[id];

    public bool TileBelongsToRegion(int x, int y, ushort regionId) => _regionMap[x, y] == regionId;

    public IEnumerable<Region> GetRegionsInArea(int x1, int y1, int x2, int y2)
    {
        var result = new HashSet<ushort>();
        int startX = Math.Min(x1, x2);
        int endX = Math.Max(x1, x2);
        int startY = Math.Min(y1, y2);
        int endY = Math.Max(y1, y2);

        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                result.Add(_regionMap[x, y]);
            }
        }

        return result.Select(id => _regions[id]);
    }
}
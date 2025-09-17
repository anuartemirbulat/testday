namespace ObjectLayerModule;

public interface IObjectLayer
{
    Task<long> AddAsync(MapObject mapObject);
    Task<MapObject> GetByID(int id);
    Task<bool> Remove(int id);
    Task<MapObject> GetByPosition(int x, int y);
    bool IsObjectInArea(MapObject obj, int areaX1, int areaY1, int areaX2, int areaY2);
    IEnumerable<MapObject> GetObjectsByArea(int x1, int x2, int y1, int y2);

    public event Action<MapObject>? ObjectAdded;
    public event Action<int>? ObjectRemoved;
}
using Common.Extensions;
using DAL.Core.Redis.Repositories;
using StackExchange.Redis;

namespace ObjectLayerModule;

public class ObjectLayer : IObjectLayer
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private const string GeoKey = "objects_geo";

    public ObjectLayer(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<long> AddAsync(MapObject mapObject)
    {
        var rep = new RedisRepository<MapObject>(_connectionMultiplexer);
        mapObject.Longitude = mapObject.X.ConverToGraduce();
        mapObject.Latitude = mapObject.Y.ConverToGraduce();
        var res = await rep.InsertAsync(mapObject, GeoKey);
        return res;
    }

    public async Task<MapObject> GetByID(int id)
    {
        var rep = new RedisRepository<MapObject>(_connectionMultiplexer);
        var mapObj = await rep.GetByIdAsync(id);
        return mapObj;
    }

    public async Task<bool> Remove(int id)
    {
        var rep = new RedisRepository<MapObject>(_connectionMultiplexer);
        var res = await rep.RemoveAsync(id, GeoKey);
        return res;
    }

    public async Task<MapObject> GetByPosition(int x, int y)
    {
        var rep = new RedisRepository<MapObject>(_connectionMultiplexer);
        var results = await rep.GeoRadiusAsync(GeoKey, x.ConverToGraduce(), y.ConverToGraduce(),
            NumberExtension.ScaleFactor);

        foreach (var redisValue in results)
        {
            if (int.TryParse(redisValue.ToString(), out int objectId))
            {
                var obj = await rep.GetByIdAsync(objectId);
                if (obj == null)
                    continue;

                bool inside = x >= obj.X && x < obj.X + obj.Width &&
                              y >= obj.Y && y < obj.Y + obj.Height;

                if (inside)
                    return obj;
            }
        }

        return null;
    }

    public bool IsObjectInArea(MapObject obj, int areaX1, int areaY1, int areaX2, int areaY2)
    {
        int startX = Math.Min(areaX1, areaX2);
        int endX = Math.Max(areaX1, areaX2);
        int startY = Math.Min(areaY1, areaY2);
        int endY = Math.Max(areaY1, areaY2);

        int objStartX = obj.X;
        int objEndX = obj.X + obj.Width;
        int objStartY = obj.Y;
        int objEndY = obj.Y + obj.Height;

        bool intersects = !(objEndX <= startX || objStartX >= endX ||
                            objEndY <= startY || objStartY >= endY);

        return intersects;
    }


    public async Task<IEnumerable<MapObject>> GetObjectsByArea(int x1, int x2, int y1, int y2)
    {
        var rep = new RedisRepository<MapObject>(_connectionMultiplexer);

        int startX = Math.Min(x1, x2);
        int endX = Math.Max(x1, x2);
        int startY = Math.Min(y1, y2);
        int endY = Math.Max(y1, y2);

        double lon1 = startX.ConverToGraduce();
        double lat1 = startY.ConverToGraduce();
        double lon2 = endX.ConverToGraduce();
        double lat2 = endY.ConverToGraduce();

        double centerLon = (lon1 + lon2) / 2;
        double centerLat = (lat1 + lat2) / 2;

        double radius = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)) / 2.0 * NumberExtension.ScaleFactor;

        var nearbyIds = await rep.GeoRadiusAsync(GeoKey, centerLon, centerLat, radius);

            
        var result = new List<MapObject>();

        foreach (var redisValue in nearbyIds)
        {
            if (!int.TryParse(redisValue.ToString(), out int objectId))
                continue;

            var obj = await rep.GetByIdAsync(objectId);
            if (obj == null)
                continue;

            if (IsObjectInArea(obj, x1, x2, y1, y2))
                result.Add(obj);
        }

        return result;
    }
}
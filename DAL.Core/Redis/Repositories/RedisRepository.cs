using DAL.Core.Redis.BaseEntities;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace DAL.Core.Redis.Repositories;

public class RedisRepository<T> : IRedisRepository<T> where T : IDocument
{
    private readonly IConnectionMultiplexer _redis;

    public RedisRepository(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<long> InsertAsync(T document,string geoKey)
    {
        var span = TimeSpan.FromMinutes(15);
        var db = _redis.GetDatabase();
        long uniqueIntId = await db.StringIncrementAsync("unique_int_id_counter");
        document.Id = uniqueIntId;
        await db.StringSetAsync(uniqueIntId.ToString(), JsonConvert.SerializeObject(document), span);
        await db.GeoAddAsync(geoKey, document.Longitude, document.Latitude, uniqueIntId.ToString());
        return uniqueIntId;
    }

    public async Task<T> GetByIdAsync(long uniqueId)
    {
        var db = _redis.GetDatabase();
        var data = await db.StringGetAsync(uniqueId.ToString());
        return data.HasValue ? JsonConvert.DeserializeObject<T>(data) : default;
    }

    public async Task<bool> RemoveAsync(long id,string geoKey)
    {
        var db = _redis.GetDatabase();
        var res = await db.KeyDeleteAsync(id.ToString());
        await db.GeoRemoveAsync(geoKey, id.ToString());
        return res;
    }

    public async Task<GeoRadiusResult[]> GeoRadiusAsync(string geoKey, double lon, double lat ,double radius)
    {
        var db = _redis.GetDatabase();
        var results = await db.GeoRadiusAsync(geoKey, lon, lat, radius, GeoUnit.Kilometers);
        return results;
    }
}
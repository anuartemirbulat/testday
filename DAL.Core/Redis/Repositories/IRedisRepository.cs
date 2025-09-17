using DAL.Core.Redis.BaseEntities;
using StackExchange.Redis;

namespace DAL.Core.Redis.Repositories;

public interface IRedisRepository<TDocument> where TDocument : IDocument
{
    long Insert(TDocument document, string geoKey);
    Task<long> InsertAsync(TDocument document, string geoKey);
    Task<TDocument> GetByIdAsync(long uniqueId);
    Task<bool> RemoveAsync(long id, string geoKey);


    GeoRadiusResult[] GeoRadius(string geoKey, double lon, double lat, double radius);
    Task<GeoRadiusResult[]> GeoRadiusAsync(string geoKey, double lon, double lat, double radius);

    Task<RedisResult> GeoSearchAsync(string geoKey, double lon, double lat, double radius);

    RedisResult GeoSearch(string geoKey, double lon, double lat, double radius);
}
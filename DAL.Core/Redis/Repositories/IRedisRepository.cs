using DAL.Core.Redis.BaseEntities;
using StackExchange.Redis;

namespace DAL.Core.Redis.Repositories;

public interface IRedisRepository<TDocument> where TDocument : IDocument
{
    Task<long> InsertAsync(TDocument document,string geoKey);
    Task<TDocument> GetByIdAsync(long uniqueId);
    Task<bool> RemoveAsync(long id,string geoKey);
    Task<GeoRadiusResult[]> GeoRadiusAsync(string geoKey, double lon, double lat, double radius);
}
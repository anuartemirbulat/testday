using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using StackExchange.Redis;
using Xunit;
using ObjectLayerModule;
using DAL.Core.Redis.Repositories;
using Common.Extensions;

public class ObjectLayerTests
{
    private readonly Mock<IConnectionMultiplexer> _redisMock;
    private readonly Mock<IDatabase> _dbMock;
    private readonly ObjectLayer _objectLayer;

    public ObjectLayerTests()
    {
        _redisMock = new Mock<IConnectionMultiplexer>();
        _dbMock = new Mock<IDatabase>();
        _redisMock.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_dbMock.Object);
        _objectLayer = new ObjectLayer(_redisMock.Object);
    }

    [Fact]
    public async Task AddAsync_SavesObjectAndGeoPosition()
    {
        var obj = new MapObject { X = 10, Y = 20, Width = 5, Height = 5 };

        _dbMock.Setup(db => db.StringIncrementAsync(
                new RedisKey("unique_int_id_counter"),
                1,
                CommandFlags.None))
            .ReturnsAsync(1);


        _dbMock.Setup(db =>
                db.StringSetAsync("1", It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(), When.Always, CommandFlags.None))
            .ReturnsAsync(true);

        _dbMock.Setup(db =>
                db.GeoAddAsync("objects_geo", obj.X.ConverToGraduce(), obj.Y.ConverToGraduce(), "1", CommandFlags.None))
            .ReturnsAsync(true);

        var result = await _objectLayer.AddAsync(obj);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task GetByID_ReturnsObject()
    {
        var obj = new MapObject { Id = 1, X = 10, Y = 20, Width = 5, Height = 5 };
        var serialized = JsonConvert.SerializeObject(obj);

        _dbMock.Setup(db => db.StringGetAsync("1", CommandFlags.None))
            .ReturnsAsync(serialized);

        var result = await _objectLayer.GetByID(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task Remove_DeletesObjectAndGeoEntry()
    {
        _dbMock.Setup(db => db.KeyDeleteAsync("1", CommandFlags.None))
            .ReturnsAsync(true);

        _dbMock.Setup(db => db.GeoRemoveAsync("objects_geo", "1", CommandFlags.None))
            .ReturnsAsync(true);

        var result = await _objectLayer.Remove(1);

        Assert.True(result);
    }

    [Theory]
    [InlineData(10, 20, 15, 25, true)]
    [InlineData(0, 0, 5, 5, false)]
    [InlineData(12, 22, 18, 28, true)]
    public void IsObjectInArea_ChecksCorrectly(int x1, int y1, int x2, int y2, bool expected)
    {
        var obj = new MapObject { X = 10, Y = 20, Width = 5, Height = 5 };
        var result = _objectLayer.IsObjectInArea(obj, x1, y1, x2, y2);
        Assert.Equal(expected, result);
    }
}
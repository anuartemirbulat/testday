using Constants.Enums;
using TileMapCore;
using TileMapCore.Implementations;

namespace Tests;

public class TileMapTests
{
    private const short TestWidth = 5;
    private const short TestHeight = 5;
    private readonly TileMap _tileMap;

    public TileMapTests()
    {
        _tileMap = new TileMap();
    }

    private IEnumerable<Tile> CreateDefaultTiles()
    {
        var tiles = new List<Tile>();
        for (int y = 0; y < TestHeight; y++)
        {
            for (int x = 0; x < TestWidth; x++)
            {
                tiles.Add(new Tile(TileType.Plain, x, y));
            }
        }

        return tiles;
    }

    [Fact]
    public void Create_ValidTiles_InitializesCorrectly()
    {
        var tiles = CreateDefaultTiles();

        _tileMap.Create(tiles, TestWidth, TestHeight);

        Assert.Equal(TestWidth, _tileMap.Width);
        Assert.Equal(TestHeight, _tileMap.Height);
        Assert.Equal(TileType.Plain, _tileMap.GetTileTypeByCoords(0, 0));
        Assert.Equal(TileType.Plain, _tileMap.GetTileTypeByCoords(4, 4));
    }

    [Fact]
    public void Create_TilesWithMixedTypes_CorrectlyPlacesTiles()
    {
        var tiles = new List<Tile>
        {
            new Tile(TileType.Mountain, 1, 1),
            new Tile(TileType.Plain, 2, 2),
            new Tile(TileType.Mountain, 3, 3)
        };

        _tileMap.Create(tiles, TestWidth, TestHeight);

        Assert.Equal(TileType.Mountain, _tileMap.GetTileTypeByCoords(1, 1));
        Assert.Equal(TileType.Plain, _tileMap.GetTileTypeByCoords(2, 2));
        Assert.Equal(TileType.Mountain, _tileMap.GetTileTypeByCoords(3, 3));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(2, 2)]
    [InlineData(4, 4)]
    public void GetSetTileTypeByCoords_ValidCoords_WorksCorrectly(int x, int y)
    {
        _tileMap.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        _tileMap.SetTileTypeByCoords(x, y, TileType.Mountain);
        var result = _tileMap.GetTileTypeByCoords(x, y);

        Assert.Equal(TileType.Mountain, result);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(5, 0)]
    [InlineData(0, 5)]
    public void GetSetTileTypeByCoords_InvalidCoords_ThrowsException(int x, int y)
    {
        _tileMap.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        Assert.Throws<IndexOutOfRangeException>(() => _tileMap.GetTileTypeByCoords(x, y));
        Assert.Throws<IndexOutOfRangeException>(() => _tileMap.SetTileTypeByCoords(x, y, TileType.Mountain));
    }

    [Fact]
    public void FillArea_ValidArea_FillsCorrectly()
    {
        _tileMap.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        _tileMap.FillArea(1, 1, 3, 3, TileType.Mountain);

        for (int y = 1; y <= 3; y++)
        {
            for (int x = 1; x <= 3; x++)
            {
                Assert.Equal(TileType.Mountain, _tileMap.GetTileTypeByCoords(x, y));
            }
        }

        Assert.Equal(TileType.Plain, _tileMap.GetTileTypeByCoords(0, 0));
        Assert.Equal(TileType.Plain, _tileMap.GetTileTypeByCoords(4, 4));
    }

    [Fact]
    public void FillArea_AreaOutsideBounds_ClippedCorrectly()
    {
        _tileMap.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        _tileMap.FillArea(-1, -1, 6, 6, TileType.Mountain);

        for (int y = 0; y < TestHeight; y++)
        {
            for (int x = 0; x < TestWidth; x++)
            {
                Assert.Equal(TileType.Mountain, _tileMap.GetTileTypeByCoords(x, y));
            }
        }
    }

    [Fact]
    public void FillArea_ReverseCoordinates_NormalizesCorrectly()
    {
        _tileMap.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        _tileMap.FillArea(3, 3, 1, 1, TileType.Mountain);

        for (int y = 1; y <= 3; y++)
        {
            for (int x = 1; x <= 3; x++)
            {
                Assert.Equal(TileType.Mountain, _tileMap.GetTileTypeByCoords(x, y));
            }
        }
    }

    [Fact]
    public void CanPlaceObjectInArea_OnPlains_ReturnsTrue()
    {
        _tileMap.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        Assert.True(_tileMap.CanPlaceObjectInArea(1, 1, 2, 2));
    }

    [Fact]
    public void CanPlaceObjectInArea_OnMountain_ReturnsFalse()
    {
        _tileMap.Create(CreateDefaultTiles(), TestWidth, TestHeight);
        _tileMap.SetTileTypeByCoords(2, 2, TileType.Mountain);

        Assert.False(_tileMap.CanPlaceObjectInArea(1, 1, 3, 3));
    }

    [Fact]
    public void CanPlaceObjectInArea_PartialMountain_ReturnsFalse()
    {
        _tileMap.Create(CreateDefaultTiles(), TestWidth, TestHeight);
        _tileMap.SetTileTypeByCoords(3, 3, TileType.Mountain);

        Assert.False(_tileMap.CanPlaceObjectInArea(2, 2, 2, 2));
    }

    [Theory]
    [InlineData(-1, 0, 2, 2)]
    [InlineData(0, -1, 2, 2)]
    [InlineData(4, 4, 2, 2)]
    [InlineData(0, 0, 6, 6)]
    public void CanPlaceObjectInArea_OutsideBounds_ReturnsFalse(int startX, int startY, int width, int height)
    {
        _tileMap.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        Assert.False(_tileMap.CanPlaceObjectInArea(startX, startY, width, height));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    public void CanPlaceObjectInArea_ZeroSize_ReturnsFalse(int width, int height)
    {
        _tileMap.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        Assert.False(_tileMap.CanPlaceObjectInArea(0, 0, width, height));
    }

    [Fact]
    public void CanPlaceObjectInArea_SingleTilePlain_ReturnsTrue()
    {
        _tileMap.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        Assert.True(_tileMap.CanPlaceObjectInArea(2, 2, 1, 1));
    }

    [Fact]
    public void CanPlaceObjectInArea_SingleTileMountain_ReturnsFalse()
    {
        _tileMap.Create(CreateDefaultTiles(), TestWidth, TestHeight);
        _tileMap.SetTileTypeByCoords(2, 2, TileType.Mountain);

        Assert.False(_tileMap.CanPlaceObjectInArea(2, 2, 1, 1));
    }

    [Fact]
    public void Tile_CanPlaceObject_ReturnsCorrectValues()
    {
        var plainTile = new Tile(TileType.Plain, 0, 0);
        var mountainTile = new Tile(TileType.Mountain, 0, 0);

        Assert.True(plainTile.CanPlaceObject);
        Assert.False(mountainTile.CanPlaceObject);
    }

    [Fact]
    public void Tile_Properties_AreCorrect()
    {
        var tile = new Tile(TileType.Mountain, 10, 20);

        Assert.Equal(TileType.Mountain, tile.Type);
        Assert.Equal(10, tile.X);
        Assert.Equal(20, tile.Y);
        Assert.False(tile.CanPlaceObject);
    }
}
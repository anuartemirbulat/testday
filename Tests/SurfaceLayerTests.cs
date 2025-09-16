using Constants.Enums;
using TileMapCore;
using TileMapCore.Abstractions;
using TileMapCore.Implementations;

namespace Tests;

public class SurfaceLayerTests
{
    private const short TestWidth = 5;
    private const short TestHeight = 5;
    private readonly IMapLayer _surfaceLayer;

    public SurfaceLayerTests()
    {
        _surfaceLayer = new SurfaceLayer();
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
    public void Create_WithEmptyTileList_ShouldNotThrow()
    {
        var tiles = new List<Tile>();
        _surfaceLayer.Create(tiles, TestWidth, TestHeight);

        Assert.Equal(TestWidth, _surfaceLayer.Width);
        Assert.Equal(TestHeight, _surfaceLayer.Height);
    }


    [Fact]
    public void Create_ValidTiles_InitializesCorrectly()
    {
        var tiles = CreateDefaultTiles();

        _surfaceLayer.Create(tiles, TestWidth, TestHeight);

        Assert.Equal(TestWidth, _surfaceLayer.Width);
        Assert.Equal(TestHeight, _surfaceLayer.Height);
        Assert.Equal(TileType.Plain, _surfaceLayer.GetTileTypeByCoords(0, 0));
        Assert.Equal(TileType.Plain, _surfaceLayer.GetTileTypeByCoords(4, 4));
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

        _surfaceLayer.Create(tiles, TestWidth, TestHeight);

        Assert.Equal(TileType.Mountain, _surfaceLayer.GetTileTypeByCoords(1, 1));
        Assert.Equal(TileType.Plain, _surfaceLayer.GetTileTypeByCoords(2, 2));
        Assert.Equal(TileType.Mountain, _surfaceLayer.GetTileTypeByCoords(3, 3));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(2, 2)]
    [InlineData(4, 4)]
    public void GetSetTileTypeByCoords_ValidCoords_WorksCorrectly(int x, int y)
    {
        _surfaceLayer.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        _surfaceLayer.SetTileTypeByCoords(x, y, TileType.Mountain);
        var result = _surfaceLayer.GetTileTypeByCoords(x, y);

        Assert.Equal(TileType.Mountain, result);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(5, 0)]
    [InlineData(0, 5)]
    public void GetSetTileTypeByCoords_InvalidCoords_ThrowsException(int x, int y)
    {
        _surfaceLayer.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        Assert.Throws<IndexOutOfRangeException>(() => _surfaceLayer.GetTileTypeByCoords(x, y));
        Assert.Throws<IndexOutOfRangeException>(() => _surfaceLayer.SetTileTypeByCoords(x, y, TileType.Mountain));
    }

    [Fact]
    public void FillArea_ValidArea_FillsCorrectly()
    {
        _surfaceLayer.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        _surfaceLayer.FillArea(1, 1, 3, 3, TileType.Mountain);

        for (int y = 1; y <= 3; y++)
        {
            for (int x = 1; x <= 3; x++)
            {
                Assert.Equal(TileType.Mountain, _surfaceLayer.GetTileTypeByCoords(x, y));
            }
        }

        Assert.Equal(TileType.Plain, _surfaceLayer.GetTileTypeByCoords(0, 0));
        Assert.Equal(TileType.Plain, _surfaceLayer.GetTileTypeByCoords(4, 4));
    }

    [Fact]
    public void FillArea_AreaOutsideBounds_ClippedCorrectly()
    {
        _surfaceLayer.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        _surfaceLayer.FillArea(-1, -1, 6, 6, TileType.Mountain);

        for (int y = 0; y < TestHeight; y++)
        {
            for (int x = 0; x < TestWidth; x++)
            {
                Assert.Equal(TileType.Mountain, _surfaceLayer.GetTileTypeByCoords(x, y));
            }
        }
    }

    [Fact]
    public void FillArea_ReverseCoordinates_NormalizesCorrectly()
    {
        _surfaceLayer.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        _surfaceLayer.FillArea(3, 3, 1, 1, TileType.Mountain);

        for (int y = 1; y <= 3; y++)
        {
            for (int x = 1; x <= 3; x++)
            {
                Assert.Equal(TileType.Mountain, _surfaceLayer.GetTileTypeByCoords(x, y));
            }
        }
    }

    [Fact]
    public void CanPlaceObjectInArea_OnPlains_ReturnsTrue()
    {
        _surfaceLayer.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        Assert.True(_surfaceLayer.CanPlaceObjectInArea(1, 1, 2, 2));
    }

    [Fact]
    public void CanPlaceObjectInArea_OnMountain_ReturnsFalse()
    {
        _surfaceLayer.Create(CreateDefaultTiles(), TestWidth, TestHeight);
        _surfaceLayer.SetTileTypeByCoords(2, 2, TileType.Mountain);

        Assert.False(_surfaceLayer.CanPlaceObjectInArea(1, 1, 3, 3));
    }

    [Fact]
    public void CanPlaceObjectInArea_PartialMountain_ReturnsFalse()
    {
        _surfaceLayer.Create(CreateDefaultTiles(), TestWidth, TestHeight);
        _surfaceLayer.SetTileTypeByCoords(3, 3, TileType.Mountain);

        Assert.False(_surfaceLayer.CanPlaceObjectInArea(2, 2, 2, 2));
    }

    [Theory]
    [InlineData(-1, 0, 2, 2)]
    [InlineData(0, -1, 2, 2)]
    [InlineData(4, 4, 2, 2)]
    [InlineData(0, 0, 6, 6)]
    public void CanPlaceObjectInArea_OutsideBounds_ReturnsFalse(int startX, int startY, int width, int height)
    {
        _surfaceLayer.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        Assert.False(_surfaceLayer.CanPlaceObjectInArea(startX, startY, width, height));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    public void CanPlaceObjectInArea_ZeroSize_ReturnsFalse(int width, int height)
    {
        _surfaceLayer.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        Assert.False(_surfaceLayer.CanPlaceObjectInArea(0, 0, width, height));
    }

    [Fact]
    public void CanPlaceObjectInArea_SingleTilePlain_ReturnsTrue()
    {
        _surfaceLayer.Create(CreateDefaultTiles(), TestWidth, TestHeight);

        Assert.True(_surfaceLayer.CanPlaceObjectInArea(2, 2, 1, 1));
    }

    [Fact]
    public void CanPlaceObjectInArea_SingleTileMountain_ReturnsFalse()
    {
        _surfaceLayer.Create(CreateDefaultTiles(), TestWidth, TestHeight);
        _surfaceLayer.SetTileTypeByCoords(2, 2, TileType.Mountain);

        Assert.False(_surfaceLayer.CanPlaceObjectInArea(2, 2, 1, 1));
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
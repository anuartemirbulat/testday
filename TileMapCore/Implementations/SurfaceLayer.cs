using Constants.Enums;
using TileMapCore.Abstractions;

namespace TileMapCore.Implementations;

public class SurfaceLayer : IMapLayer
{
    private Tile[] _items;

    public short Width { get; set; }
    public short Height { get; set; }

    private int GetIndex(int x, int y)
    {
        return y * Width + x;
    }

    public TileType GetTileTypeByCoords(int x, int y)
    {
        if (!IsCoordinateValid(x, y))
        {
            throw new IndexOutOfRangeException($"Coordinates ({x}, {y}) are out of bounds");
        }

        var index = GetIndex(x, y);
        return _items[index].Type;
    }

    public void SetTileTypeByCoords(int x, int y, TileType type)
    {
        if (!IsCoordinateValid(x, y))
        {
            throw new IndexOutOfRangeException($"Coordinates ({x}, {y}) are out of bounds");
        }

        var index = GetIndex(x, y);
        _items[index].Type = type;
    }

    private bool IsCoordinateValid(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public void Create(IEnumerable<Tile> tiles, short width, short height)
    {
        if (_items == null || width != Width || height != Height)
        {
            Width = width;
            Height = height;
            _items = new Tile[width * height];
        }

        foreach (var tile in tiles)
        {
            if (!IsCoordinateValid(tile.X, tile.Y))
                continue;
            int index = GetIndex(tile.X, tile.Y);
            _items[index] = tile;
        }
    }

    public void FillArea(int startX, int startY, int endX, int endY, TileType tileType)
    {
        var actualStartX = Math.Min(startX, endX);
        var actualStartY = Math.Min(startY, endY);
        var actualEndX = Math.Max(startX, endX);
        var actualEndY = Math.Max(startY, endY);

        actualStartX = Math.Max(0, actualStartX);
        actualStartY = Math.Max(0, actualStartY);
        actualEndX = Math.Min(Width - 1, actualEndX);
        actualEndY = Math.Min(Height - 1, actualEndY);

        for (var y = actualStartY; y <= actualEndY; y++)
        {
            for (int x = actualStartX; x <= actualEndX; x++)
            {
                _items[GetIndex(x, y)].Type = tileType;
            }
        }
    }

    public bool CanPlaceObjectInArea(int startX, int startY, int width, int height)
    {
        if (width <= 0 || height <= 0)
            return false;

        int endX = startX + width - 1;
        int endY = startY + height - 1;

        if (startX < 0 || startY < 0 || endX >= Width || endY >= Height)
            return false;

        for (int y = startY; y <= endY; y++)
        {
            int rowStartIndex = GetIndex(startX, y);
            for (int x = 0; x < width; x++)
            {
                if (!_items[rowStartIndex + x].CanPlaceObject)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
using Constants.Enums;

namespace TileMapCore;

public struct Tile
{
    public Tile(TileType type, int x, int y)
    {
        Type = type;
        X = x;
        Y = y;
    }

    public TileType Type { get; set; }
    public int X { get; }
    public int Y { get; }
    public bool CanPlaceObject => Type == TileType.Plain;
}
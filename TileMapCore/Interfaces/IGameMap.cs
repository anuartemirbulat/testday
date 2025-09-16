using Constants.Enums;

namespace TileMapCore.Interfaces;

public interface IGameMap
{
    TileType GetTileTypeByCoords(int x, int y);
    void SetTileTypeByCoords(int x, int y, TileType type);
    void Create(IEnumerable<Tile> tiles, short width, short height);
    void FillArea(int startX, int startY, int endX, int endY, TileType tileType);
}
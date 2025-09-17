using MemoryPack;

namespace DataContracts;

[MemoryPackable]
public partial class MapObjectDto
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

using DAL.Core.Redis.BaseEntities;

namespace ObjectLayerModule;

public class MapObject:Document
{
    public int X { get; set; }
    public int Y { get; set; }
    public short Width { get; set; }
    public short Height { get; set; }
}
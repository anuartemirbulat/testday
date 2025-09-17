using MemoryPack;

namespace DataContracts;

[MemoryPackable]
public partial class ObjectEvent
{
    public string Type { get; set; } // "Added", "Updated", "Deleted"
    public MapObjectDto Object { get; set; }
}

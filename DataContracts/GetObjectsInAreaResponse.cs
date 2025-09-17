using MemoryPack;

namespace DataContracts;
[MemoryPackable]
public partial class GetObjectsInAreaResponse
{
    public List<MapObjectDto> Objects { get; set; }
}

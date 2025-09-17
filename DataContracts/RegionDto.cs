using MemoryPack;

namespace DataContracts;

[MemoryPackable]
public partial class RegionDto
{
    public ushort Id { get; set; }
    public string Name { get; set; }
}

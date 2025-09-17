using MemoryPack;

namespace DataContracts;

[MemoryPackable]
public partial class GetRegionsInAreaResponse
{
    public List<RegionDto> Regions { get; set; }
}

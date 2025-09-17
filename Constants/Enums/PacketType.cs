namespace Constants.Enums;

public enum PacketType : byte
{
    GetObjectsInArea = 1,
    GetObjectsInAreaResponse = 2,
    GetRegionsInArea = 3,
    GetRegionsInAreaResponse = 4,
    ObjectEvent = 5,
    AddObject=6
}

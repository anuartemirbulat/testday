using System.Net;
using System.Net.Sockets;
using Constants;
using Constants.Enums;
using DataContracts;
using LiteNetLib;
using MemoryPack;
using ObjectLayerModule;
using RegionModule;

namespace MapServer;

public class GameMapServer : INetEventListener
{
    private readonly NetManager _server;
    private readonly IObjectLayer _objectLayer;
    private readonly IRegionLayer _regionLayer;
    private readonly List<NetPeer> _clients = new();

    public GameMapServer(IObjectLayer objectLayer, IRegionLayer regionLayer)
    {
        _objectLayer = objectLayer;
        _regionLayer = regionLayer;
        _server = new NetManager(this);
        _server.Start(9050);
    }

    public void PollEvents() => _server.PollEvents();

    public void OnConnectionRequest(ConnectionRequest request)
    {
        request.AcceptIfKey(Key.MapKey);
    }

    public void OnPeerConnected(NetPeer peer)
    {
        lock (_clients) _clients.Add(peer);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo info)
    {
        lock (_clients) _clients.Remove(peer);
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        throw new NotImplementedException();
    }
    

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
        UnconnectedMessageType messageType)
    {
        throw new NotImplementedException();
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber,
        DeliveryMethod deliveryMethod)
    {
        var data = reader.GetRemainingBytes();
        reader.Recycle();

        var type = (PacketType)data[0];
        var payload = data.Skip(1).ToArray();

        switch (type)
        {
            case PacketType.GetObjectsInArea:
                var reqObj = MemoryPackSerializer.Deserialize<GetObjectsInAreaRequest>(payload);
                var objects = _objectLayer.GetObjectsByArea(reqObj.X1, reqObj.Y1, reqObj.X2, reqObj.Y2);
                var objResponse = new GetObjectsInAreaResponse
                {
                    Objects = objects.Select(o => new MapObjectDto
                        { Id = o.Id, X = o.X, Y = o.Y, Width = o.Width, Height = o.Height }).ToList()
                };
                SendResponse(peer, PacketType.GetObjectsInAreaResponse, objResponse);
                break;

            case PacketType.GetRegionsInArea:
                var reqReg = MemoryPackSerializer.Deserialize<GetRegionsInAreaRequest>(payload);
                var regions = _regionLayer.GetRegionsInArea(reqReg.X1, reqReg.Y1, reqReg.X2, reqReg.Y2);
                var regResponse = new GetRegionsInAreaResponse
                    { Regions = regions.Select(r => new RegionDto { Id = r.Id, Name = r.Name }).ToList() };
                SendResponse(peer, PacketType.GetRegionsInAreaResponse, regResponse);
                break;
        }
    }

    private void SendResponse<T>(NetPeer peer, PacketType type, T payload)
    {
        var data = MemoryPackSerializer.Serialize(payload);
        var packet = new byte[data.Length + 1];
        packet[0] = (byte)type;
        Buffer.BlockCopy(data, 0, packet, 1, data.Length);
        peer.Send(packet, DeliveryMethod.ReliableOrdered);
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }
}
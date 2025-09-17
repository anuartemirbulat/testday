// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using Constants;
using Constants.Enums;
using DataContracts;
using LiteNetLib;
using MemoryPack;

class Program : INetEventListener
{
    private NetManager _client;
    private NetPeer? _server;

    static async Task Main(string[] args)
    {
        var listener = new Program();
        listener._client = new NetManager(listener);
        listener._client.Start();
        listener._client.Connect("mapserver", 9050, Key.MapKey);

        Console.WriteLine("Connecting to server...");
        while (listener._server == null)
        {
            listener._client.PollEvents();
            await Task.Delay(100);
        }

        Console.WriteLine("Запрос объектов по области...");
        var request = new GetObjectsInAreaRequest { X1 = 10, Y1 = 10, X2 = 50, Y2 = 50 };
        var data = MemoryPackSerializer.Serialize(request);
        var packet = new byte[data.Length + 1];
        packet[0] = (byte)PacketType.GetObjectsInArea;
        Buffer.BlockCopy(data, 0, packet, 1, data.Length);
        listener._server.Send(packet, DeliveryMethod.ReliableOrdered);
        
        Console.WriteLine("Запрос регионов по области...");
        var regionRequest = new GetRegionsInAreaRequest { X1 = 10, Y1 = 10, X2 = 50, Y2 = 50 };
        var regionData = MemoryPackSerializer.Serialize(regionRequest);
        var regionPacket = new byte[regionData.Length + 1];
        regionPacket[0] = (byte)PacketType.GetRegionsInArea;
        Buffer.BlockCopy(regionData, 0, regionPacket, 1, regionData.Length);
        listener._server.Send(regionPacket, DeliveryMethod.ReliableOrdered);

        

        while (true)
        {
            listener._client.PollEvents();
            await Task.Delay(10);
        }
    }

    public void OnPeerConnected(NetPeer peer)
    {
        _server = peer;
        Console.WriteLine("Connected to server.");
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        var data = reader.GetRemainingBytes();
        reader.Recycle();

        var type = (PacketType)data[0];
        var payload = data.Skip(1).ToArray();

        switch (type)
        {
            case PacketType.GetObjectsInAreaResponse:
                var response = MemoryPackSerializer.Deserialize<GetObjectsInAreaResponse>(payload);
                Console.WriteLine($"В этой области находится {response.Objects.Count} объектов:");
                foreach (var obj in response.Objects)
                {
                    Console.WriteLine($"ID: {obj.Id}, Size: {obj.Width}x{obj.Height} , x:{obj.X} , y:{obj.Y}");
                }
                break;
            
            case PacketType.GetRegionsInAreaResponse:
            {
                var responseRegion = MemoryPackSerializer.Deserialize<GetRegionsInAreaResponse>(payload);
                Console.WriteLine($"В этой области находится {responseRegion.Regions.Count} регионов:");
                foreach (var region in responseRegion.Regions)
                {
                    Console.WriteLine($"Region: {region.Name} , Id={region.Id}");
                }
                break;
            }

        }
    }

    public void OnConnectionRequest(ConnectionRequest request) => request.AcceptIfKey(Key.MapKey);
    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo info) => Console.WriteLine("Disconnected.");
    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        throw new NotImplementedException();
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        throw new NotImplementedException();
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }
}

using LiteNetLib;
using LiteNetLib.Utils;
using Tetris.GUI;

namespace Tetris.Multiplayer.Network.Packets;

public class PlayerConnectPacket : Packet
{
    public PlayerConnectPacket(int id) : base(id)
    {
    }

    protected override void RunPacket(NetPacketReader packetReader)
    {
        Gui.Instance.AddDebugMessage("Player has successfully connected!");
        NetworkManager.Instance.Connected = true;
        base.RunPacket(packetReader);
    }

    protected override void SendPacket()
    {
        dataWriter = new NetDataWriter();
        dataWriter.Put(PacketID);
        base.SendPacket();
    }
}
using LiteNetLib;
using LiteNetLib.Utils;
using Tetris.Game.Events;
using Tetris.Game.Managers;

namespace Tetris.Multiplayer.Network.Packets;

public class PausePacket : Packet
{
    public PausePacket(int id) : base(id)
    {
    }

    protected override void RunPacket(NetPacketReader packetReader)
    { 
        EventManager.Instance.GetEvent("pause").Call();
        base.RunPacket(packetReader);
    }

    protected override void SendPacket()
    {
        dataWriter = new NetDataWriter();
        dataWriter.Put(PacketID);
        base.SendPacket();
    }
}
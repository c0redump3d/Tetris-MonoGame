using LiteNetLib;
using LiteNetLib.Utils;
using Tetris.Game.InGame;

namespace Tetris.Multiplayer.Network.Packets;

public class SendGarbagePacket : Packet
{
    public SendGarbagePacket(int id) : base(id)
    {
    }

    protected override void RunPacket(NetPacketReader packetReader)
    {
        TetrisBoard.Instance.RandomBlock(packetReader.GetInt());
        base.RunPacket(packetReader);
    }

    protected override void SendPacket()
    {
        if (TetrisBoard.Instance.GetActualLines() - 1 < 1)
            return;

        dataWriter = new NetDataWriter();
        dataWriter.Put(PacketID);
        dataWriter.Put(TetrisBoard.Instance.GetActualLines() - 1);
        base.SendPacket();
    }
}
using System.Linq;
using LiteNetLib;
using LiteNetLib.Utils;
using Tetris.Game.InGame;
using Tetris.Util;

namespace Tetris.Multiplayer.Network.Packets;

public class BoardPacket : Packet
{
    public BoardPacket(int id) : base(id)
    {
    }

    protected override void RunPacket(NetPacketReader packetReader)
    {
        for (var i = 0; i < TetrisBoard.Instance.Board.GetLength(0); i++)
        {
            var row = packetReader.GetIntArray();
            for (var f = 0; f < TetrisBoard.Instance.Board.GetLength(1); f++)
                PlayerMP.Instance.MultiplayerBoard[i, f] = row[f];
        }

        base.RunPacket(packetReader);
    }

    protected override void SendPacket()
    {
        dataWriter = new NetDataWriter();
        dataWriter.Put(PacketID);
        for (var i = 0; i < TetrisBoard.Instance.Board.GetLength(0); i++)
            dataWriter.PutArray(TetrisBoard.Instance.Board.SliceRow(i).ToArray());
        base.SendPacket();
    }
}
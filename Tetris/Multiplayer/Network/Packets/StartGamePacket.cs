using LiteNetLib;
using LiteNetLib.Utils;
using Tetris.Game;
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.Game.Mode;
using Tetris.GUI;
using Tetris.GUI.Screens;

namespace Tetris.Multiplayer.Network.Packets
{
    public class StartGamePacket : Packet
    {
        public StartGamePacket(int id) : base(id){}

        protected override void SendPacket()
        {
            dataWriter = new NetDataWriter();
            dataWriter.Put(PacketID);
            dataWriter.Put(ScoreHandler.Instance.SelectedLevel);
            dataWriter.Put(ModeManager.Instance.GetCurrentMode().Name);
            base.SendPacket();
        }

        protected override void RunPacket(NetPacketReader packetReader)
        {
            PlayerMP.Instance.MultiplayerBoard = new int[22, 10];
            ScoreHandler.Instance.SelectedLevel = packetReader.GetInt();
            ModeManager.Instance.SetCurrentMode(packetReader.GetString());
            Gui.Instance.SetCurrentScreen(new GuiInGame());
            base.RunPacket(packetReader);
        }
    }
}
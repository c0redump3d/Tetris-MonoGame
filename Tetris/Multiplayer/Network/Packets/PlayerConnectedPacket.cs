using Tetris.Other;

namespace Tetris.Multiplayer.Network.Packets
{
    class PlayerConnectedPacket : Packet
    {

        public PlayerConnectedPacket(string name) : base(name) {}

        protected override void RunPacket()
        {
            if(!Instance.GetGame().Stopped)
                Instance.GetGame().EndGame(); // make sure there is no running game
            Instance.AllPlayersConnected = true;
            Instance.InMultiplayer = true;
            if (!IsServer())
            {
                Instance.GetGui().AddMenuButtons();
                Instance.GetScoreHandler().SelectedLevel = 1;
            }
            Instance.GetGuiDebug().DebugMessage("Successfully connected to player!");
            base.RunPacket();
        }
    }
}

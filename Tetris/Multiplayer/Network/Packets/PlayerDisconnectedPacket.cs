using Tetris.Other;

namespace Tetris.Multiplayer.Network.Packets
{
    class PlayerDisconnectedPacket : Packet
    {
        
        public PlayerDisconnectedPacket(string name) : base(name) {}

        protected override void RunPacket()
        {
            Instance.AllPlayersConnected = false;
            Instance.GetMultiplayerHandler().HideMultiplayer();
            
            Instance.GetGuiDebug().DebugMessage(@"Disconnected from player.");
            
            if(IsServer())
                Server.CloseConnection();
            else
            {
                if (Instance.GetGame().CurrentScreen == 0)
                {
                    Instance.GetGui().AddMenuButtons();
                }

                Client.CloseConnection();
            }
            
            if(!Instance.GetGame().Stopped)
                Instance.GetGame().EndGame(); // end any running games
            
            base.RunPacket();
        }

        protected override void SendPacket()
        {
            base.SendPacket();
        }
    }
}

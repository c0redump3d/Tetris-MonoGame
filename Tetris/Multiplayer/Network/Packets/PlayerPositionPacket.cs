using Tetris.Main.Player;
using Tetris.Other;

namespace Tetris.Multiplayer.Network.Packets
{
    class PlayerPositionPacket : Packet
    {

        public PlayerPositionPacket(string name) : base(name) {}

        protected override void RunPacket(string data)
        {
            string[] pos = data.Split(',');
            var mHandler = Instance.GetMultiplayerHandler();
         
            //with the given data, read and parse each position.
            
            mHandler.MultiX = int.Parse(pos[1]);
            mHandler.MultiY = int.Parse(pos[2]);
            mHandler.Mr1 = int.Parse(pos[3]);
            mHandler.Mr2 = int.Parse(pos[4]);
            mHandler.Ml1 = int.Parse(pos[5]);
            mHandler.Ml2 = int.Parse(pos[6]);
            mHandler.Mt1 = int.Parse(pos[7]);
            mHandler.Mt2 = int.Parse(pos[8]);
            base.RunPacket(data);
        }

        protected override void SendPacket()
        {
            if (!InMultiplayer())
                return;
            string data = "";
            TetrisPlayer ply = Instance.GetPlayer();
            //sends all important positions of the player(x,y,location of ply rectangles).
            data = $",{ply.PlyX},{ply.PlyY},{ply.PlayerPos[0]},{ply.PlayerPos[1]},{ply.PlayerPos[2]},{ply.PlayerPos[3]},{ply.PlayerPos[4]},{ply.PlayerPos[5]}";
            base.SendPacket(data);
        }
    }
}

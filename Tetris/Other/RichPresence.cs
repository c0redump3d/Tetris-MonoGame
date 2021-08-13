using DiscordRPC;
using DiscordRPC.Logging;

namespace Tetris.Other
{
    public class RichPresence
    {
        private readonly DiscordRpcClient client;
        private int currentScreen = -1;

        public RichPresence()
        {
            client = new DiscordRpcClient("873719171211485224");
            client.Logger = new ConsoleLogger() {Level = LogLevel.Warning};
            client.Initialize();
        }

        /// <summary>
        /// Updates rich presence to show current scores from in-game
        /// </summary>
        public void UpdatePresence()
        {
            string status = Instance.GetGame().GameOver ? "Game Over" : Instance.GetGame().Paused ? "Paused" : $"{Instance.GetGame().GameModes[Instance.GetGame().CurrentMode]}";
            client.UpdateDetails($"{status} | Level {Instance.GetScoreHandler().Level}");
            client.UpdateState($"Score - {Instance.GetScoreHandler().Score:n0} | Lines - {Instance.GetScoreHandler().TotalLines}");
        }

        public void Shutdown()
        {
            client.Deinitialize();
            client.Dispose();
        }
        
        public void SetPresence(int screen)
        {
            if (currentScreen == screen)
                return;
            
            if (screen == 0)
            {
                client.SetPresence(new DiscordRPC.RichPresence()
                {
                    Details = "Main Menu",
                    State = "Idle",
                    Timestamps = Timestamps.Now,
                    Assets = new Assets()
                    {
                        LargeImageKey = $"tetrisicon640"
                    }
                });
            }

            if (screen == 1)
            {
                client.SetPresence(new DiscordRPC.RichPresence()
                {
                    Details = $"{Instance.GetGame().GameModes[Instance.GetGame().CurrentMode]} | Level {Instance.GetScoreHandler().Level}",
                    State = $"Score - {Instance.GetScoreHandler().Score:n0} | Lines - {Instance.GetScoreHandler().TotalLines}",
                    Timestamps = Timestamps.Now,
                    Assets = new Assets()
                    {
                        LargeImageKey = $"tetrisicon640"
                    }
                });
            }
            
            Instance.GetGuiDebug().DebugMessage($"Successfully set discord presence to menu {screen}.");
            currentScreen = screen;
        }
        
    }
}
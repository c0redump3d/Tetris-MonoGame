
#if !__IOS__
using DiscordRPC;
using DiscordRPC.Logging;
#endif
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.Game.Mode;
using Tetris.GUI.DebugMenu;

namespace Tetris.Util
{
    public class RichPresence
    {
        
#if !__IOS__
        private readonly DiscordRpcClient client;
        private int currentScreen = -1;
#endif

        private RichPresence()
        {
            #if !__IOS__
            client = new DiscordRpcClient("873719171211485224") {Logger = new ConsoleLogger {Level = LogLevel.Warning}};
            client.Initialize();
            #endif
        }

        /// <summary>
        ///     Updates rich presence to show current scores from in-game
        /// </summary>
        public void UpdatePresence()
        {
#if !__IOS__
            var status = InGameManager.Instance.GameOver ? "Game Over" :
                InGameManager.Instance.Paused ? "Paused" :
                $"{ModeManager.Instance.GetCurrentMode().Name}";
            client.UpdateDetails($"{status} | Level {ScoreHandler.Instance.Level}");
            client.UpdateState(
                $"Score - {ScoreHandler.Instance.Score:n0} | Lines - {ScoreHandler.Instance.TotalLines}");
#endif
        }

        public void Shutdown()
        {
#if !__IOS__
            client.Deinitialize();
            client.Dispose();
#endif
        }

        public void SetPresence(int screen)
        {
#if !__IOS__
            if (currentScreen == screen)
                return;

            if (screen == 0)
                client.SetPresence(new DiscordRPC.RichPresence
                {
                    Details = "Main Menu",
                    State = "Idle",
                    Timestamps = Timestamps.Now,
                    Assets = new Assets
                    {
                        LargeImageKey = "tetrisicon640"
                    }
                });

            if (screen == 1)
                client.SetPresence(new DiscordRPC.RichPresence
                {
                    Details =
                        $"{ModeManager.Instance.GetCurrentMode().Name} | Level {ScoreHandler.Instance.Level}",
                    State =
                        $"Score - {ScoreHandler.Instance.Score:n0} | Lines - {ScoreHandler.Instance.TotalLines}",
                    Timestamps = Timestamps.Now,
                    Assets = new Assets
                    {
                        LargeImageKey = "tetrisicon640"
                    }
                });

            DebugConsole.Instance.AddMessage($"Successfully set discord presence to menu {screen}.");
            currentScreen = screen;
#endif
        }

        private static RichPresence _instance;
        public static RichPresence Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new RichPresence();
                }

                return result;
            }
        }
    }
}
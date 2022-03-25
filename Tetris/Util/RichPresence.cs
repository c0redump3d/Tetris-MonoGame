using System;
using DiscordRPC;
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.Game.Mode;
using Tetris.GUI;

namespace Tetris.Util;

public class RichPresence
{
    private static RichPresence _instance;

    private readonly DiscordRpcClient client;
    private int currentScreen = -1;

    private RichPresence()
    {
        try
        {
            client = new DiscordRpcClient("873719171211485224");
            client.Initialize();
        }
        catch (Exception)
        {
            Gui.Instance.AddDebugMessage("Failed to start discord rpc.");
        }
    }

    public static RichPresence Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new RichPresence();

            return result;
        }
    }

    /// <summary>
    ///     Updates rich presence to show current scores from in-game
    /// </summary>
    public void UpdatePresence()
    {
        var status = InGameManager.Instance.GameOver ? "Game Over" :
            InGameManager.Instance.Paused ? "Paused" :
            $"{ModeManager.Instance.GetCurrentMode().Name}";
        client.UpdateDetails($"{status} | Level {ScoreHandler.Instance.Level}");
        client.UpdateState(
            $"Score - {ScoreHandler.Instance.Score:n0} | Lines - {ScoreHandler.Instance.TotalLines}");
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

        Gui.Instance.AddDebugMessage($"Successfully set discord presence to menu {screen}.");
        currentScreen = screen;
    }
}
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Game.Events;
using Tetris.GUI;
using Tetris.Multiplayer.Network;
using Tetris.Settings;
using Tetris.Util;

namespace Tetris.Game.Managers;

public class GameManager
{
    private static GameManager _instance;

    //This class is used to draw stuff to the screen when the player is not in game(Any type of menu other than the pause menu)

    public readonly Random Random = new();
    private bool isFullscreen;

    private KeyboardState keyState;
    private KeyboardState oldKeyState;
    private int oldWidth, oldHeight;

    public static GameManager Instance
    {
        get
        {
            //need to instantiate keyboard input before this class.
            var keyboard = KeyboardInput.Instance;
            var result = _instance;
            if (result == null) result = _instance ??= new GameManager();

            return result;
        }
    }

    public void UpdateAll(GameTime gameTime)
    {
        Globals.GameTime = gameTime;
        OnKeyPress();
        NetworkManager.Instance.UpdateNetwork(gameTime);
        InGameManager.Instance.UpdateGame(gameTime);
        Gui.Instance.Update(gameTime);
        TimerUtil.Instance.UpdateTimers(gameTime);
    }

    /// <summary>
    ///     Draws the game to the window. Note: everything drawn in this field will be rendered to a render target of 1280x720
    /// </summary>
    public void DrawAll(SpriteBatch gameBatch, SpriteBatch screenBatch, GameTime gameTime)
    {
        screenBatch.Begin();
        screenBatch.Draw(Globals.GradientBackground, new Vector2(0, 0), Color.White);
        screenBatch.End();
        Gui.Instance.DrawGui(gameBatch, gameTime);
        InGameManager.Instance.DrawGame(gameBatch, screenBatch);
        Gui.Instance.DrawPanels(screenBatch, gameTime);
    }

    private void OnKeyPress()
    {
        keyState = Keyboard.GetState();

        EventManager.Instance.GetEvent("keyboard").Call();

        if (oldKeyState.IsKeyDown(Keys.LeftAlt) && keyState.IsKeyDown(Keys.Enter) &&
            !oldKeyState.IsKeyDown(Keys.Enter) ||
            IsFullscreen() != (bool) GameSettings.Instance.GetOptionValue("Fullscreen"))
        {
            GameSettings.Instance.ChangeToggle("Fullscreen", !IsFullscreen());
            GameSettings.Instance.Save();
            isFullscreen = (bool) GameSettings.Instance.GetOptionValue("Fullscreen");
            Fullscreen();
        }

        oldKeyState = keyState;
    }

    /// <summary>
    ///     Starts the application.
    /// </summary>
    public void RunGame()
    {
        using var game = TetrisGame.Instance;
        game.Run();
    }

    public bool IsFullscreen()
    {
        return isFullscreen;
    }

    private void Fullscreen()
    {
        var graphics = TetrisGame.Instance.Graphics;
        var window = TetrisGame.Instance.Window;
        if (!isFullscreen)
        {
            graphics.ToggleFullScreen();
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Globals.ScreenWidth = 1280;
            Globals.ScreenHeight = 720;
        }
        else
        {
            window.BeginScreenDeviceChange(isFullscreen);
            window.EndScreenDeviceChange(window.ScreenDeviceName, graphics.GraphicsDevice.DisplayMode.Width,
                graphics.GraphicsDevice.DisplayMode.Height);
            graphics.IsFullScreen = true;
        }

        TetrisGame.Instance.Window.IsBorderless = isFullscreen;
        graphics.ApplyChanges();
    }
}
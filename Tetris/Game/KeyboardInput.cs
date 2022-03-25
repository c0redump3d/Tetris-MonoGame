using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Tetris.Game.Events;
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.Game.Player;
using Tetris.GUI;
using Tetris.GUI.DebugMenu;
using Tetris.GUI.Screens;
using Tetris.Multiplayer.Network;
using Tetris.Settings;

namespace Tetris.Game;

public class KeyboardInput
{
    private static KeyboardInput _instance;
    //TODO: Add gamepad support(directly supported by MonoGame)

    private readonly double[] elapsedTime = new double[3];
    private readonly double[] holdTime = new double[3];
    private KeyboardState keyState;
    private KeyboardState oldKeyState;

    private KeyboardInput()
    {
        EventManager.Instance.CreateEvent("keyboard", HandleKeyPress);
        TetrisGame.Instance.Window.TextInput += KeyTyped;
    }

    public static KeyboardInput Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new KeyboardInput();

            return result;
        }
    }

    public void HandleKeyPress()
    {
        keyState = Keyboard.GetState();
        var gameTime = Globals.GameTime;

        if (Gui.Instance.CurrentScreen is GuiInGame &&
            InGameManager.Instance.CanMove)
            if (IsKeyPress(Keys.Escape))
                if (NetworkManager.Instance.IsServer() && NetworkManager.Instance.Connected ||
                    !NetworkManager.Instance.Connected)
                {
                    EventManager.Instance.GetEvent("pause").Call();
                    NetworkManager.Instance.SendPacket(8);
                }

        if (IsKeyPress(Keys.F4)) DebugMenu.Instance.ShowMenu();

        if (Gui.Instance.CurrentScreen is GuiSettings)
        {
            var screen = (GuiSettings) Gui.Instance.CurrentScreen;
            if (keyState.GetPressedKeys().Length > 0)
                if (screen.CurrentlyFocused)
                    screen.KeyPressed(keyState.GetPressedKeys()[0]);
        }

        if (InGameManager.Instance.Stopped || InGameManager.Instance.Paused || !InGameManager.Instance.CanMove)
        {
            oldKeyState = keyState;
            return;
        }

        for (var i = 0; i < elapsedTime.Length; i++)
            elapsedTime[i] -= gameTime.ElapsedGameTime.TotalMilliseconds;

        if (IsKeyPress((Keys) GameSettings.Instance.GetOptionValue("HardDrop")) && !InGameManager.Instance.Waiting)
            EventManager.Instance.GetEvent("forcedrop").Call();

        if (IsKeyPress((Keys) GameSettings.Instance.GetOptionValue("Hold")))
            EventManager.Instance.GetEvent("hold").Call();

        if (keyState.IsKeyDown((Keys) GameSettings.Instance.GetOptionValue("Left")) &&
            !oldKeyState.IsKeyDown((Keys) GameSettings.Instance.GetOptionValue("Right")))
        {
            if (elapsedTime[0] <= 0)
            {
                Movement.Instance.MoveLeft();
                if (holdTime[0] > 400)
                    elapsedTime[0] = 50;
                else
                    elapsedTime[0] = 150;
            }

            holdTime[0] += gameTime.ElapsedGameTime.TotalMilliseconds;
        }
        else
        {
            elapsedTime[0] = 0;
            holdTime[0] = 0;
        }

        if (keyState.IsKeyDown((Keys) GameSettings.Instance.GetOptionValue("Right")) &&
            !oldKeyState.IsKeyDown((Keys) GameSettings.Instance.GetOptionValue("Left")))
        {
            if (elapsedTime[1] <= 0)
            {
                Movement.Instance.MoveRight();

                if (holdTime[1] > 400)
                    elapsedTime[1] = 50;
                else
                    elapsedTime[1] = 150;
            }

            holdTime[1] += gameTime.ElapsedGameTime.TotalMilliseconds;
        }
        else
        {
            elapsedTime[1] = 0;
            holdTime[1] = 0;
        }

        if (keyState.IsKeyDown((Keys) GameSettings.Instance.GetOptionValue("Down")) && !InGameManager.Instance.Waiting)
        {
            if (elapsedTime[2] <= 0)
            {
                Movement.Instance.MoveDown();
                if (holdTime[2] > 400)
                    elapsedTime[2] = 50;
                else
                    elapsedTime[2] = 150;
            }

            holdTime[2] += gameTime.ElapsedGameTime.TotalMilliseconds;
        }
        else
        {
            elapsedTime[2] = 0;
            holdTime[2] = 0;
        }

        if (IsKeyPress(Keys.F1) && DebugMenu.Instance.IsOptionEnabled(2))
        {
            var shape = Rotate.Instance.GetCurShape();
            if (shape < 8)
                shape++;
            if (shape == 8)
                shape = 1;

            Rotate.Instance.SetCurShape(shape);
            Rotate.Instance.ResetRot();
            PlayerController.Instance.SetShape();
        }

        if (IsKeyPress((Keys) GameSettings.Instance.GetOptionValue("RotateRight")) ||
            IsKeyPress((Keys) GameSettings.Instance.GetOptionValue("RotateLeft")))
            Rotate.Instance.RotateTetris(
                !keyState.IsKeyDown((Keys) GameSettings.Instance.GetOptionValue("RotateLeft")));

        oldKeyState = keyState;
    }

    private void KeyTyped(object sender, TextInputEventArgs args)
    {
        if (Gui.Instance.CurrentTextBox != null)
            Gui.Instance.CurrentTextBox.UpdateText(args.Character);
    }

    private bool IsKeyPress(Keys key)
    {
        if (keyState.IsKeyDown(key) && !oldKeyState.IsKeyDown(key))
            return true;

        return false;
    }
}
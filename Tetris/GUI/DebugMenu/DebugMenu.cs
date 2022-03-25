using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Game;
using Tetris.Game.Player;
using Tetris.GUI.Control.Controls;
using Tetris.GUI.Control.Controls.DebugControls;

namespace Tetris.GUI.DebugMenu;

/// <summary>
///     An extremely basic menu that allows for enabling/disabling of certain debugging features in-game.
/// </summary>
public class DebugMenu
{
    private static DebugMenu _instance;

    private readonly List<bool> debugEnabled;
    private readonly List<string> debugOptions;
    private readonly string menuText = "Tetris Debug Menu";
    public DebugConsole Console;
    private int cursorPos;
    private KeyboardState keyState;
    private float longestTextX;
    private float longestTextY;
    private KeyboardState oldKeyState;
    private bool showDebugMenu;

    private DebugMenu()
    {
        debugOptions = new List<string>();
        debugEnabled = new List<bool>();
        AddOption("Debug Console");
        AddOption("Tetrimino Names");
        AddOption("Tetrimino Info");
        AddOption("Draw Rotation Tetriminos");
        AddOption("Button Debug");
    }

    public static DebugMenu Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new DebugMenu();

            return result;
        }
    }

    public void Update(GameTime gameTime)
    {
        if (showDebugMenu) // listen for arrow keypresses to update debug menu
        {
            keyState = Keyboard.GetState();

            if (IsKeyPress(Keys.Down))
            {
                if (cursorPos < debugOptions.Count - 1)
                    cursorPos++;
                else
                    cursorPos = 0;

                CheckLength();
            }

            if (IsKeyPress(Keys.Up))
            {
                if (cursorPos > 0)
                    cursorPos--;
                else
                    cursorPos = debugOptions.Count - 1;
                CheckLength();
            }

            if (IsKeyPress(Keys.Enter))
            {
                debugEnabled[cursorPos] = !debugEnabled[cursorPos];
                if (debugEnabled[cursorPos])
                    HandleEnable(cursorPos);
                else
                    HandleDisable(cursorPos);
                CheckLength();
            }

            oldKeyState = keyState;
        }
    }

    private void HandleEnable(int id)
    {
        var CurrentScreen = Gui.Instance.CurrentScreen;
        switch (id)
        {
            case 0:
                if (Console == null && CurrentScreen != null)
                {
                    var debugPan = new Panel("Debug Console", 50, 50, true, 1);
                    var performancePan = new Panel("Performance", 50, 100
                        , true, 1);
                    CurrentScreen.AddControl(debugPan);
                    debugPan.AddControl(Console = new DebugConsole());
                    CurrentScreen.AddControl(performancePan);
                    performancePan.AddControl(new FpsGraph(0, 0));
                    //performancePan.AddControl(new ParticleList());
                }
                break;
            case 4:
                if (CurrentScreen != null)
                {
                    var controlPan = new Panel("Control List", 50, 200, true, 1);
                    CurrentScreen.AddControl(controlPan);
                    controlPan.AddControl(new ControlList());
                }
                break;
        }
    }

    private void HandleDisable(int id)
    {
        var CurrentScreen = Gui.Instance.CurrentScreen;
        switch (id)
        {
            case 0:
                if (Console != null && CurrentScreen != null)
                {
                    foreach (var con in CurrentScreen.Controls.ToList())
                        if (con.GetType() == typeof(Panel))
                            if (((Panel) con).Text.Contains("Debug") || con.Text.Contains("Performance"))
                            {
                                if (Gui.Instance.CurrentPanel == con)
                                    Gui.Instance.CurrentPanel = null;
                                CurrentScreen.Controls.Remove(con);
                            }

                    Console = null;
                }

                break;
            case 4:
                if (CurrentScreen != null)
                    foreach (var con in CurrentScreen.Controls.ToList())
                        if (con.GetType() == typeof(Panel))
                            if (((Panel) con).Text.Contains("Control"))
                            {
                                if (Gui.Instance.CurrentPanel == con)
                                    Gui.Instance.CurrentPanel = null;
                                CurrentScreen.Controls.Remove(con);
                            }
                break;
        }
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (showDebugMenu)
        {
            var lastY = 0;
            spriteBatch.Draw(Globals.TexBox,
                new Rectangle(0, 0, (int) longestTextX + 2, (debugOptions.Count + 2) * 12 + 20),
                Color.Black * 0.5f);
            spriteBatch.DrawString(Fonts.ConsoleFont, $"{menuText}", new Vector2(0, 0),
                Color.White);
            for (var i = 0; i < debugOptions.Count; i++)
            {
                var cursor = cursorPos == i ? ">" : "";
                var enabled = debugEnabled[i] ? "ON" : "OFF";
                spriteBatch.DrawString(Fonts.ConsoleFont, $"{cursor}{debugOptions[i]}:{enabled}",
                    new Vector2(10, (i + 1) * 12), Color.White);
                lastY = (i + 1) * 12;
            }

            //Not a super great way of getting a super accurate value of frames, but works well enough for its purpose.
            var extraInfo =
                $"{Globals.Version} fps: {1 / gameTime.ElapsedGameTime.TotalSeconds:n2} os: {Globals.CurrentOS}";

            spriteBatch.DrawString(Fonts.ConsoleFont, $"{extraInfo}", new Vector2(0, lastY += 18), Color.White);
            spriteBatch.DrawString(Fonts.ConsoleFont, $"resolution: {Globals.ScreenWidth}x{Globals.ScreenHeight}",
                new Vector2(0, lastY + 12), Color.White);
        }
    }

    private void AddOption(string name)
    {
        debugOptions.Add(name);
        debugEnabled.Add(false);
    }

    /// <summary>
    ///     Draws helpful player specific debug information to screen.
    /// </summary>
    public void DrawPlayerDebug(SpriteBatch spriteBatch)
    {
        var ply = PlayerController.Instance;

        //Draws blocks name on top of block(ex: ply[1] would draw text 'R1' at its current position)
        if (IsOptionEnabled(1))
        {
            spriteBatch.DrawString(Fonts.ConsoleFont, "ply",
                new Vector2(ply.PlayerBlocks[0].X + 3, ply.PlayerBlocks[0].Y + 5),
                Color.Black);
            spriteBatch.DrawString(Fonts.ConsoleFont, "rgt",
                new Vector2(ply.PlayerBlocks[1].X + 4, ply.PlayerBlocks[1].Y + 5),
                Color.Black);
            spriteBatch.DrawString(Fonts.ConsoleFont, "lft",
                new Vector2(ply.PlayerBlocks[2].X + 5, ply.PlayerBlocks[2].Y + 5),
                Color.Black);
            spriteBatch.DrawString(Fonts.ConsoleFont, "top",
                new Vector2(ply.PlayerBlocks[3].X + 3, ply.PlayerBlocks[3].Y + 5),
                Color.Black);
        }

        if (IsOptionEnabled(2))
        {
            Rectangle[] positions =
                {ply.PlayerBlocks[0], ply.PlayerBlocks[1], ply.PlayerBlocks[2], ply.PlayerBlocks[3]};
            var lastY = 0;
            //Player block offset positions from main block(ply[0]).
            for (var i = 0; i < positions.Length; i++)
            {
                var blPos = "";
                switch (i)
                {
                    case 1:
                        blPos = $"R1: {ply.PlayerPos[0]} R2: {ply.PlayerPos[1]}";
                        break;
                    case 2:
                        blPos = $"L1: {ply.PlayerPos[2]} L2: {ply.PlayerPos[3]}";
                        break;
                    case 3:
                        blPos = $"T1: {ply.PlayerPos[4]} T2: {ply.PlayerPos[5]}";
                        break;
                }

                spriteBatch.DrawString(Fonts.ConsoleFont,
                    $"({i})X: {positions[i].X} Y: {positions[i].Y} {blPos}",
                    new Vector2(0, i * 14), Color.White * 0.5F);
                lastY = i + 1;
            }

            //Draws the current shape and angle of the block.
            spriteBatch.DrawString(Fonts.ConsoleFont,
                $"shape/ang: {Rotate.Instance.GetCurShape()},{Rotate.Instance.GetCurAngle()}",
                new Vector2(0, lastY * 14), Color.White * 0.5F);
            spriteBatch.DrawString(Fonts.ConsoleFont, $"gravity: {ply.GetGravityTime():n2}ms",
                new Vector2(0, ++lastY * 14),
                Color.White * 0.5F);
            spriteBatch.DrawString(Fonts.ConsoleFont, $"grounded: {ply.Grounded}", new Vector2(0, ++lastY * 14),
                Color.White * 0.5F);
        }

        //(Rotation blocks)
        if (IsOptionEnabled(3))
            for (var i = 0; i < 4; i++)
                spriteBatch.Draw(Globals.BlockTexture[Rotate.Instance.GetCurShape() - 1],
                    RotateCheck.Instance.GetRotationBlocks()[i], Color.White * 0.2F);
    }

    /// <summary>
    ///     Check if a debug option is currently enabled or disabled.
    /// </summary>
    /// <param name="mod">Debug Option Index</param>
    /// <returns>True if debug option is enabled</returns>
    public bool IsOptionEnabled(int mod)
    {
        if (mod < 0 || mod > debugOptions.Count - 1)
            return false;

        return debugEnabled[mod];
    }

    private bool IsKeyPress(Keys key)
    {
        if (keyState.IsKeyDown(key) && !oldKeyState.IsKeyDown(key))
            return true;

        return false;
    }

    public void ShowMenu()
    {
        showDebugMenu = !showDebugMenu;
        if (showDebugMenu) CheckLength();
    }

    /// <summary>
    ///     This checks to see which debug option has the longest text so that the bordered rect encases everything
    /// </summary>
    private void CheckLength()
    {
        longestTextX = Fonts.ConsoleFont.MeasureString(menuText).X;
        longestTextY = Fonts.ConsoleFont.MeasureString(menuText).Y;

        for (var i = 0; i < debugOptions.Count; i++)
        {
            var cursor = cursorPos == i ? ">" : "";
            var enabled = debugEnabled[i] ? "ON" : "OFF";
            var text = $"{cursor}{debugOptions[i]}:{enabled}";
            if (Fonts.ConsoleFont.MeasureString(text).X + 10 > longestTextX)
                longestTextX = Fonts.ConsoleFont.MeasureString(text).X + 10;
            if (Fonts.ConsoleFont.MeasureString(text).Y > longestTextY)
                longestTextY = Fonts.ConsoleFont.MeasureString(text).Y;
        }
    }
}
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Game;
using Tetris.Game.Player;

namespace Tetris.GUI.DebugMenu
{
    /// <summary>
    /// An extremely basic menu that allows for enabling/disabling of certain debugging features in-game.
    /// </summary>
    public class DebugMenu
    {
        
        private readonly List<bool> debugEnabled;
        private readonly List<string> debugOptions;
        private KeyboardState keyState;
        private KeyboardState oldKeyState;
        private bool showDebugMenu;
        private int cursorPos;
        private float longestTextX;
        private float longestTextY;
        private readonly string menuText = "Tetris Debug Menu";

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
                    CheckLength();
                }

                oldKeyState = keyState;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (showDebugMenu)
            {
                var lastY = 0;
                spriteBatch.Begin();
                spriteBatch.Draw(Globals.TexBox,
                    new Rectangle(0, 0, (int) longestTextX + 2, (debugOptions.Count + 2) * 12 + 20),
                    Color.Black * 0.5f);
                spriteBatch.DrawString(Globals.ConsoleFont, $"{menuText}", new Vector2(0, 0),
                    Color.White);
                for (var i = 0; i < debugOptions.Count; i++)
                {
                    var cursor = cursorPos == i ? ">" : "";
                    var enabled = debugEnabled[i] ? "ON" : "OFF";
                    spriteBatch.DrawString(Globals.ConsoleFont, $"{cursor}{debugOptions[i]}:{enabled}",
                        new Vector2(10, (i + 1) * 12), Color.White);
                    lastY = (i + 1) * 12;
                }

                //Not a super great way of getting a super accurate value of frames, but works well enough for its purpose.
                var extraInfo =
                    $"{Globals.Version} fps: {1 / gameTime.ElapsedGameTime.TotalSeconds:n2} os: {Globals.CurrentOS}";

                spriteBatch.DrawString(Globals.ConsoleFont, $"{extraInfo}", new Vector2(0, lastY += 18), Color.White);
                spriteBatch.DrawString(Globals.ConsoleFont, $"resolution: {Globals.ScreenWidth}x{Globals.ScreenHeight}", new Vector2(0, lastY+12), Color.White);
                spriteBatch.End();
            }
        }
        
        private void AddOption(string name)
        {
            debugOptions.Add(name);
            debugEnabled.Add(false);
        }
        
        /// <summary>
        /// Draws helpful player specific debug information to screen.
        /// </summary>
        public void DrawPlayerDebug(SpriteBatch spriteBatch)
        {
            var ply = PlayerController.Instance;
            
            //Draws blocks name on top of block(ex: ply[1] would draw text 'R1' at its current position)
            if (IsOptionEnabled(1))
            {
                spriteBatch.DrawString(Globals.ConsoleFont, "ply", new Vector2(ply.PlayerBlocks[0].X + 3, ply.PlayerBlocks[0].Y + 5),
                    Color.Black);
                spriteBatch.DrawString(Globals.ConsoleFont, "rgt", new Vector2(ply.PlayerBlocks[1].X + 4, ply.PlayerBlocks[1].Y + 5),
                    Color.Black);
                spriteBatch.DrawString(Globals.ConsoleFont, "lft", new Vector2(ply.PlayerBlocks[2].X + 5, ply.PlayerBlocks[2].Y + 5),
                    Color.Black);
                spriteBatch.DrawString(Globals.ConsoleFont, "top", new Vector2(ply.PlayerBlocks[3].X + 3, ply.PlayerBlocks[3].Y + 5),
                    Color.Black);
            }

            if (IsOptionEnabled(2))
            {
                Rectangle[] positions = {ply.PlayerBlocks[0], ply.PlayerBlocks[1], ply.PlayerBlocks[2], ply.PlayerBlocks[3]};
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

                    spriteBatch.DrawString(Globals.ConsoleFont,
                        $"({i})X: {positions[i].X} Y: {positions[i].Y} {blPos}",
                        new Vector2(0, i * 14), Color.White * 0.5F);
                    lastY = i + 1;
                }

                //Draws the current shape and angle of the block.
                spriteBatch.DrawString(Globals.ConsoleFont,
                    $"shape/ang: {Rotate.Instance.GetCurShape()},{Rotate.Instance.GetCurAngle()}",
                    new Vector2(0, lastY * 14), Color.White * 0.5F);
                spriteBatch.DrawString(Globals.ConsoleFont, $"gravity: {ply.GetGravityTime():n2}ms",
                    new Vector2(0, ++lastY * 14),
                    Color.White * 0.5F);
                spriteBatch.DrawString(Globals.ConsoleFont, $"grounded: {ply.Grounded}", new Vector2(0, ++lastY * 14),
                    Color.White * 0.5F);
            }

            //(Rotation blocks)
            if (IsOptionEnabled(3))
                for (var i = 0; i < 4; i++)
                    spriteBatch.Draw(Globals.BlockTexture[Rotate.Instance.GetCurShape()-1],
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
            longestTextX = Globals.ConsoleFont.MeasureString(menuText).X;
            longestTextY = Globals.ConsoleFont.MeasureString(menuText).Y;

            for (var i = 0; i < debugOptions.Count; i++)
            {
                var cursor = cursorPos == i ? ">" : "";
                var enabled = debugEnabled[i] ? "ON" : "OFF";
                var text = $"{cursor}{debugOptions[i]}:{enabled}";
                if (Globals.ConsoleFont.MeasureString(text).X + 10 > longestTextX)
                    longestTextX = Globals.ConsoleFont.MeasureString(text).X + 10;
                if (Globals.ConsoleFont.MeasureString(text).Y > longestTextY)
                    longestTextY = Globals.ConsoleFont.MeasureString(text).Y;
            }
        }

        private static DebugMenu _instance;
        public static DebugMenu Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new DebugMenu();
                }

                return result;
            }
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.GUI.UiColor;
using Tetris.Multiplayer.Network;
using Tetris.Sound;
using Tetris.Util;

namespace Tetris.GUI.Screens
{
    public class GuiInGame : GuiScreen
    {
        public override void SetUp()
        {
            Rain = false;
            Sfx.Instance.PlayBackground();
            InGameManager.Instance.StartCountdown();
            base.SetUp();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void DrawScreen(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.DrawScreen(spriteBatch, gameTime);

            int multX = NetworkManager.Instance.Connected ? 160 : 360;
            
            spriteBatch.Begin();
            
            spriteBatch.Draw(Globals.GuiSPLayers[0], Vector2.Zero, ColorManager.Instance.GuiColor["Game Box Grid"] * 0.3f);
            spriteBatch.Draw(Globals.GuiSPLayers[1], Vector2.Zero, ColorManager.Instance.GuiColor["Game Box Background"]);
            spriteBatch.Draw(Globals.GuiSPLayers[2], Vector2.Zero, ColorManager.Instance.GuiColor["Game Box Border"]);
            spriteBatch.Draw(Globals.GuiSPLayers[4], Vector2.Zero, ColorManager.Instance.GuiColor["Score Background"]);
            spriteBatch.Draw(Globals.GuiSPLayers[3], Vector2.Zero, ColorManager.Instance.GuiColor["Score Border"]);
            spriteBatch.Draw(Globals.GuiSPLayers[5], Vector2.Zero, ColorManager.Instance.GuiColor["Score Text"]);
            
            spriteBatch.DrawCenteredString(Globals.Hoog28, $"{ScoreHandler.Instance.LevelText}",
                new Vector2(multX, 320), Color.White);
            spriteBatch.DrawCenteredString(Globals.Hoog28, $"{ScoreHandler.Instance.LineText}",
                new Vector2(multX, 450), Color.White);
            spriteBatch.DrawCenteredString(ScoreHandler.Instance.ScoreFont,
                $"{ScoreHandler.Instance.ScoreText}", new Vector2(multX, 580), Color.White);
            spriteBatch.End();
        }
    }
}
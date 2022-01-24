using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.Game.Mode;
using Tetris.Game.Mode.Modes;
using Tetris.Game.Player;
using Tetris.GUI.Elements;
using Tetris.GUI.Screens.ScreenAnimations;
using Tetris.Sound;
using Tetris.Util;

namespace Tetris.GUI.Screens
{
    public class GuiGameOver : GuiScreen
    {
        public override void SetUp()
        {
            base.SetUp();
            Sfx.Instance.PlayBackground();
            TetrisRain.Instance.FadeTime = 0f;
            Buttons.Add(new Button(1, new Vector2(640, 550), "Main Menu", Globals.Hoog48));
            Buttons[0].OnClick += o => Gui.SetCurrentScreen(new GuiMainMenu());
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void DrawScreen(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.DrawScreen(spriteBatch, gameTime);
            var mult = Opacity > 0.5f ? 0.5f : Opacity;
            spriteBatch.Begin();
            spriteBatch.DrawBorderedRect(new Rectangle(320, 30, 640, 450), PanelBackgroundCol * mult,
                PanelBorderCol * Opacity);
            spriteBatch.DrawStringWithShadow(Globals.Hoog48, @"Game Over!", new Vector2(440, 30), Color.White);
            var secondsText = PlayerController.Instance.TimeElapsed[1] < 10
                ? $"0{PlayerController.Instance.TimeElapsed[1]}"
                : $"{PlayerController.Instance.TimeElapsed[1]}";
            spriteBatch.DrawCenteredString(Globals.Hoog24,
                $@"Time Elapsed: {PlayerController.Instance.TimeElapsed[0]}:{secondsText}",
                new Vector2(640, 120), Color.White);

            spriteBatch.DrawStringWithShadow(Globals.Hoog36, @"Game Stats:", new Vector2(480, 175), Color.White);
            spriteBatch.DrawCenteredString(Globals.Hoog24, $@"Score: {ScoreHandler.Instance.Score:n0}",
                new Vector2(640, 250), Color.White);
            spriteBatch.DrawCenteredString(Globals.Hoog24, $@"Total Lines: {ScoreHandler.Instance.TotalLines}",
                new Vector2(640, 290), Color.White);
            spriteBatch.DrawCenteredString(Globals.Hoog24, $@"T-Spin Singles: {ScoreHandler.Instance.Bonuses[0]}",
                new Vector2(640, 330), Color.White);
            spriteBatch.DrawCenteredString(Globals.Hoog24, $@"T-Spin Doubles: {ScoreHandler.Instance.Bonuses[1]}",
                new Vector2(640, 370), Color.White);
            spriteBatch.DrawCenteredString(Globals.Hoog24, $@"T-Spin Triples: {ScoreHandler.Instance.Bonuses[2]}",
                new Vector2(640, 410), Color.White);
            spriteBatch.DrawCenteredString(Globals.Hoog24, $@"Tetrises: {ScoreHandler.Instance.Bonuses[3]}",
                new Vector2(640, 450), Color.White);
            spriteBatch.End();
        }
    }
}
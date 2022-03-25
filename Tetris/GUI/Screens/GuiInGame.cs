using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.GUI.UiColor;
using Tetris.Multiplayer.Network;
using Tetris.Sound;
using Tetris.Util;

namespace Tetris.GUI.Screens;

public class GuiInGame : GuiScreen
{
    public override void SetUp()
    {
        Rain = false;
        Sfx.Instance.PlayBackground();
        base.SetUp();
        InGameManager.Instance.StartCountdown();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void DrawScreen(SpriteBatch spriteBatch, GameTime gameTime)
    {
        base.DrawScreen(spriteBatch, gameTime);

        var multX = NetworkManager.Instance.Connected ? 162 : 362;

        var location = NetworkManager.Instance.Connected ? new Vector2(-201, 0) : Vector2.Zero;
        var locationMult = NetworkManager.Instance.Connected ? new Vector2(377, 0) : Vector2.Zero;

        //Draws each of the in game ui layers.
        spriteBatch.Draw(Globals.GuiSPLayers[0], location,
            ColorManager.Instance.GuiColor["Game Box Grid"] * (Opacity < 0.5f ? Opacity : 0.5f));
        spriteBatch.Draw(Globals.GuiSPLayers[1], location,
            ColorManager.Instance.GuiColor["Game Box Background"] * Opacity);
        spriteBatch.Draw(Globals.GuiSPLayers[2], location, ColorManager.Instance.GuiColor["Game Box Border"] * Opacity);
        spriteBatch.Draw(Globals.GuiSPLayers[4], location,
            ColorManager.Instance.GuiColor["Score Background"] * Opacity);
        spriteBatch.Draw(Globals.GuiSPLayers[3], location, ColorManager.Instance.GuiColor["Score Border"] * Opacity);
        spriteBatch.Draw(Globals.GuiSPLayers[5], location, ColorManager.Instance.GuiColor["Score Text"] * Opacity);

        if (NetworkManager.Instance.Connected)
        {
            spriteBatch.Draw(Globals.GuiSPLayers[0], locationMult,
                ColorManager.Instance.GuiColor["Game Box Grid"] * (Opacity < 0.5f ? Opacity : 0.5f));
            spriteBatch.Draw(Globals.GuiSPLayers[1], locationMult,
                ColorManager.Instance.GuiColor["Game Box Background"] * Opacity);
            spriteBatch.Draw(Globals.GuiSPLayers[2], locationMult,
                ColorManager.Instance.GuiColor["Game Box Border"] * Opacity);
        }

        spriteBatch.DrawCenteredString(Fonts.Hoog28, $"{ScoreHandler.Instance.LevelText}",
            new Vector2(multX, 315), Color.White * Opacity);
        spriteBatch.DrawCenteredString(Fonts.Hoog28, $"{ScoreHandler.Instance.LineText}",
            new Vector2(multX, 445), Color.White * Opacity);
        spriteBatch.DrawCenteredString(ScoreHandler.Instance.ScoreFont,
            $"{ScoreHandler.Instance.ScoreText}", new Vector2(multX, 575), Color.White * Opacity);
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.Game.InGame;
using Tetris.Game.Mode;
using Tetris.GUI.Control.Controls;
using Tetris.Multiplayer.Network;
using Tetris.Util;

namespace Tetris.GUI.Screens;

public class GuiGamePreferences : GuiScreen
{
    public override void SetUp()
    {
        base.SetUp();
        AddControl(new Button(new Vector2(840, 265), ">", Fonts.Hoog48, false));
        AddControl(new Button(new Vector2(440, 265), "<", Fonts.Hoog48, false));
        AddControl(new Button(new Vector2(840, 365), ">", Fonts.Hoog48, false));
        AddControl(new Button(new Vector2(440, 365), "<", Fonts.Hoog48, false));
        AddControl(new Button(new Vector2(640, 510), "Start", Fonts.Hoog48));
        AddControl(new Button(new Vector2(640, 610), "Back", Fonts.Hoog48));
        AddControl(new Paragraph(ModeManager.Instance.GetCurrentMode().Objective, Fonts.Hoog24, new Vector2(640, 160), 600));
        ((Button) GetControlFromType(typeof(Button), 0)).OnClick += LevelRight;
        ((Button) GetControlFromType(typeof(Button), 1)).OnClick += LevelLeft;
        ((Button) GetControlFromType(typeof(Button), 2)).OnClick += ModeRight;
        ((Button) GetControlFromType(typeof(Button), 3)).OnClick += ModeLeft;
        ((Button) GetControlFromType(typeof(Button), 4)).OnClick += Start;
        ((Button) GetControlFromType(typeof(Button), 5)).OnClick += s => Gui.SetCurrentScreen(new GuiMainMenu());
        ButtonsDrawn = true;
    }

    public override void DrawScreen(SpriteBatch spriteBatch, GameTime gameTime)
    {
        base.DrawScreen(spriteBatch, gameTime);
        var mult = Opacity > 0.5f ? 0.5f : Opacity;
        spriteBatch.DrawBorderedRect(new Rectangle(320, 30, 640, 450), PanelBackgroundCol * mult,
            PanelBorderCol * Opacity);

        spriteBatch.DrawCenteredString(Fonts.Hoog48,
            $"Level {ScoreHandler.Instance.SelectedLevel}", new Vector2(640, 300),
            Color.White * Opacity);
        spriteBatch.DrawCenteredString(Fonts.Hoog48,
            $"{ModeManager.Instance.GetCurrentMode().Name}", new Vector2(640, 400),
            Color.White * Opacity);
        spriteBatch.DrawCenteredString(Fonts.Hoog24,
            "Objective:", new Vector2(640, 50),
            Color.White * Opacity);

        foreach (var con in Controls)
            if (con.GetType() == typeof(Button))
                ((Button) con).Draw(spriteBatch, Color.White);
        
        ((Paragraph) GetControlFromType(typeof(Paragraph), 0)).Draw(spriteBatch);
    }

    private void Start(object sender)
    {
        Gui.Instance.SetCurrentScreen(new GuiInGame());
        NetworkManager.Instance.SendPacket(6);
    }

    private void ModeLeft(object sender)
    {
        var modes = ModeManager.Instance.GameModes;
        for (var i = 0; i < modes.Count; i++)
            if (modes[i].Name == ModeManager.Instance.GetCurrentMode().Name)
            {
                var newMode = i - 1;
                if (newMode == -1)
                    newMode = modes.Count - 1;
                ModeManager.Instance.SetCurrentMode(modes[newMode].Name);
                ((Paragraph) GetControlFromType(typeof(Paragraph), 0)).Text =
                    ModeManager.Instance.GetCurrentMode().Objective;
                break;
            }
    }

    private void ModeRight(object sender)
    {
        var modes = ModeManager.Instance.GameModes;
        for (var i = 0; i < modes.Count; i++)
            if (modes[i].Name == ModeManager.Instance.GetCurrentMode().Name)
            {
                var newMode = i + 1;
                if (newMode == modes.Count)
                    newMode = 0;
                ModeManager.Instance.SetCurrentMode(modes[newMode].Name);
                ((Paragraph) GetControlFromType(typeof(Paragraph), 0)).Text =
                    ModeManager.Instance.GetCurrentMode().Objective;
                break;
            }
    }

    private void LevelLeft(object sender)
    {
        var level = ScoreHandler.Instance.SelectedLevel;
        if (level == 1)
            level = 20;
        else
            level--;
        ScoreHandler.Instance.SelectedLevel = level;
    }

    private void LevelRight(object sender)
    {
        var level = ScoreHandler.Instance.SelectedLevel;
        if (level == 20)
            level = 1;
        else
            level++;
        ScoreHandler.Instance.SelectedLevel = level;
    }
}
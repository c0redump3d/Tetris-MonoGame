using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.GUI.Control.Controls;
using Tetris.GUI.UiColor;
using Tetris.Util;

namespace Tetris.GUI.Screens;

public class GuiEditColor : GuiScreen
{
    private List<UiColorOption> colorOptions;
    private bool hideMockUp;

    public override void SetUp()
    {
        base.SetUp();
        AddControl(new Button(new Vector2(300, 620), "Back", Fonts.Hoog48));
        ((Button) GetControlFromType(typeof(Button), 0)).OnClick +=
            o => Gui.Instance.SetCurrentScreen(new GuiSettings());
        AddControl(new Button(new Vector2(1210, 330), "Hide", Fonts.Hoog24));
        ((Button) GetControlFromType(typeof(Button), 1)).OnClick += ShowHide;
        AddControl(new Button(new Vector2(300, 550), "Example button", Fonts.Hoog24));
        AddControl(new TextBox(150, 500, "Example textbox", 15, "", 300, 30));
        colorOptions = new List<UiColorOption>();
        int x = 10, y = 10;
        List<string> addedCat = new();
        foreach (var control in ColorManager.Instance.GuiColor)
        {
            var cat = control.Key.Split(' ')[0];
            if (!addedCat.Contains(cat))
            {
                var option = new UiColorOption(cat, new Vector2(x, y), 160);
                var offscreen = option.Position.X + option.PanelWidth > 1250;
                if (offscreen)
                {
                    x = 10;
                    y += option.PanelHeight + 10;
                }

                option = new UiColorOption(cat, new Vector2(x, y), 160);
                colorOptions.Add(option);
                x += option.PanelWidth + 10;

                addedCat.Add(cat);
            }
        }

        ButtonsDrawn = true;
    }

    private void ShowHide(object sender)
    {
        var but = (Button) sender;
        if (but.Text.Equals("Hide"))
        {
            hideMockUp = true;
            but.Text = "Show";
        }
        else
        {
            but.Text = "Hide";
            hideMockUp = false;
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (Gui.Instance.CurrentColorPanel != null)
        {
            var tempArr = colorOptions;
            if (tempArr.IndexOf(Gui.Instance.CurrentColorPanel) != 0)
            {
                tempArr.Remove(Gui.Instance.CurrentColorPanel);
                tempArr.Insert(0, Gui.Instance.CurrentColorPanel);
                colorOptions = tempArr;
            }
        }

        foreach (var option in colorOptions) option.Update(gameTime);

        base.Update(gameTime);
    }

    public override void DrawScreen(SpriteBatch spriteBatch, GameTime gameTime)
    {
        base.DrawScreen(spriteBatch, gameTime);
        foreach (var option in colorOptions.Reverse<UiColorOption>()) option.Draw(spriteBatch);

        foreach (var uiControl in Controls)
            if (uiControl.GetType() == typeof(TextBox))
            {
                var textBox = (TextBox) uiControl;
                textBox.Draw(spriteBatch, gameTime);
            }

        //1265
        var mult = Opacity > 0.5f ? 0.5f : Opacity;
        if (!hideMockUp)
        {
            spriteBatch.DrawBorderedRect(new Rectangle(625, 320, 640, 390), PanelBackgroundCol * mult,
                PanelBorderCol * Opacity);
            spriteBatch.DrawCenteredString(Fonts.Hoog24, "In-Game Mock-Up", new Vector2(945, 340), Color.White);
            spriteBatch.Draw(Globals.GuiSPLayers[0], new Vector2(625, 355), null,
                ColorManager.Instance.GuiColor["Game Box Grid"] * 0.3f, 0f, Vector2.Zero, 0.5f, SpriteEffects.None,
                0f);
            spriteBatch.Draw(Globals.GuiSPLayers[1], new Vector2(625, 355), null,
                ColorManager.Instance.GuiColor["Game Box Background"], 0f, Vector2.Zero, 0.5f, SpriteEffects.None,
                0f);
            spriteBatch.Draw(Globals.GuiSPLayers[2], new Vector2(625, 355), null,
                ColorManager.Instance.GuiColor["Game Box Border"], 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(Globals.GuiSPLayers[4], new Vector2(625, 355), null,
                ColorManager.Instance.GuiColor["Score Background"], 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(Globals.GuiSPLayers[3], new Vector2(625, 355), null,
                ColorManager.Instance.GuiColor["Score Border"], 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(Globals.GuiSPLayers[5], new Vector2(625, 355), null,
                ColorManager.Instance.GuiColor["Score Text"], 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }

        foreach (var uiControl in Controls)
            if (uiControl.GetType() == typeof(Button))
            {
                var but = (Button) uiControl;
                but.Draw(spriteBatch, Color.White);
            }
    }
}
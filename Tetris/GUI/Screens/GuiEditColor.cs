using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.Game.Managers;
using Tetris.GUI.DebugMenu;
using Tetris.GUI.Elements;
using Tetris.GUI.UiColor;
using Tetris.Util;

namespace Tetris.GUI.Screens
{
    public class GuiEditColor : GuiScreen
    {

        private List<UiColorOption> colorOptions;
        private bool hideMockUp = false;

        public override void SetUp()
        {
            base.SetUp();
            Buttons.Add(new Button(0, new Vector2(300, 620), "Back", Globals.Hoog48));
            Buttons[0].OnClick += o => Gui.Instance.SetCurrentScreen(new GuiSettings());
            Buttons.Add(new Button(1, new Vector2(1210,330), "Hide", Globals.Hoog24));
            Buttons[1].OnClick += ShowHide;
            TextBoxes.Add(new TextBox(150, 500, "Example textbox", 15, "", 300, 30));
            Buttons.Add(new Button(1, new Vector2(300, 550), "Example button", Globals.Hoog24));
            colorOptions = new List<UiColorOption>();
            int x = 10, y = 10;
            List<string> addedCat = new();
            foreach (KeyValuePair<string, Color> control in ColorManager.Instance.GuiColor)
            {
                string cat = control.Key.Split(' ')[0];
                if (!addedCat.Contains(cat))
                {
                    UiColorOption option = new UiColorOption(cat, new Vector2(x, y), 160);
                    bool offscreen = option.Position.X + option.PanelWidth > 1250;
                    if (offscreen)
                    {
                        x = 10;
                        y += 155;
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
            Button but = (Button) sender;
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
            foreach (UiColorOption option in colorOptions)
            {
                option.Update(gameTime);
            }

            foreach (TextBox tBox in TextBoxes)
            {
                tBox.Update();
            }
            base.Update(gameTime);
        }
        
        public override void DrawScreen(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.DrawScreen(spriteBatch, gameTime);
            spriteBatch.Begin();
            foreach (UiColorOption option in colorOptions)
            {
                option.Draw(spriteBatch);
            }

            foreach (var textBox in TextBoxes)
            {
                textBox.Draw(spriteBatch, gameTime);
            }
            //1265
            var mult = Opacity > 0.5f ? 0.5f : Opacity;
            if (!hideMockUp)
            {
                spriteBatch.DrawBorderedRect(new Rectangle(625, 320, 640, 390), PanelBackgroundCol * mult,
                    PanelBorderCol * Opacity);
                spriteBatch.DrawCenteredString(Globals.Hoog24, "In-Game Mock-Up", new Vector2(945, 340), Color.White);
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

            foreach (Button but in Buttons)
            {
                but.Draw(spriteBatch, Color.White);
            }
            spriteBatch.End();
        }
    }
}
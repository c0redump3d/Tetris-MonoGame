using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.GUI.DebugMenu;
using Tetris.GUI.Elements;
using Tetris.Multiplayer.Network;
using Tetris.Util;

namespace Tetris.GUI.Screens
{
    public class GuiMultiplayer : GuiScreen
    {

        public override void SetUp()
        {
            base.SetUp();
            Gui.Instance.MultiplayerMessage =
                NetworkManager.Instance.Connected ? $"Connected to {(NetworkManager.Instance.IsServer() ? "client" : "host")}" : "Not connected";
            Buttons.Add(new Button(0, new Vector2(640, 420), "Connect", Globals.Hoog28));
            Buttons.Add(new Button(0, new Vector2(640, 510), "Create Server", Globals.Hoog48));
            Buttons.Add(new Button(3, new Vector2(640, 610), "Back", Globals.Hoog48));
            Buttons[0].OnClick += TryConnect;
            Buttons[1].OnClick += s => Gui.Instance.SetCurrentScreen(new GuiCreateServer());
            Buttons[2].OnClick += MenuClick;
            TextBoxes.Add(new(400, 250, "192.168.1.1:9050", 21, @"^[0-9.:]*$"));
            TextBoxes.Add(new(400, 350, "Password", 15));
            ButtonsDrawn = true;
            if (NetworkManager.Instance.Connected && !NetworkManager.Instance.IsServer())
            {
                Buttons[0].Text = "Disconnect";
                Buttons[1].Enabled = false;
            }
            else if(NetworkManager.Instance.IsServer())
            {
                Buttons[0].Enabled = false;
            }
        }

        private void TryConnect(object sender)
        {
            try
            {
                if (!NetworkManager.Instance.Connected)
                {
                    NetworkManager.Instance.Connect(TextBoxes[0].Text.Split(':')[0],
                        int.Parse(TextBoxes[0].Text.Split(':')[1]),
                        TextBoxes[1].Text);
                    Gui.Instance.MultiplayerMessage = "Attempting connection...";
                }
                else
                {
                    NetworkManager.Instance.Disconnect();
                    Buttons[0].Text = "Connect";
                }
            }
            catch (Exception)
            {
                Gui.Instance.MultiplayerMessage = $"Invalid IP Address!";
                DebugConsole.Instance.AddMessage("Invalid ip address.");
            }
        }
        
        private void MenuClick(object sender)
        {
            Gui.SetCurrentScreen(new GuiMainMenu());
        }

        public override void Update(GameTime gameTime)
        {
            if (NetworkManager.Instance.Connected && Buttons[1].Enabled && !NetworkManager.Instance.IsServer())
            {
                Buttons[1].Enabled = false;
                Buttons[0].Text = "Disconnect";
            }else if (!NetworkManager.Instance.Connected && !Buttons[1].Enabled && !NetworkManager.Instance.IsServer())
            {
                Buttons[1].Enabled = true;
                Buttons[0].Text = "Connect";
            }
            
            TextBoxes[0].Update();
            TextBoxes[1].Update();
            foreach (var but in Buttons) but.Update();
            base.Update(gameTime);
        }

        public override void DrawScreen(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.DrawScreen(spriteBatch, gameTime);

            spriteBatch.Begin();
            var mult = Opacity > 0.5f ? 0.5f : Opacity;
            spriteBatch.DrawBorderedRect(new Rectangle(320, 30, 640, 450), PanelBackgroundCol * mult,
                PanelBorderCol * Opacity);
            spriteBatch.DrawCenteredString(Globals.Hoog36, "Multiplayer", new Vector2(640, 50), Color.White);
            spriteBatch.DrawStringWithShadow(Globals.Hoog24, "IP Address and Port:", new Vector2(400, 215), Color.White);
            spriteBatch.DrawStringWithShadow(Globals.Hoog24, "Password:", new Vector2(400, 315), Color.White);
            spriteBatch.DrawCenteredString(Globals.Hoog12, Gui.Instance.MultiplayerMessage, new Vector2(640, 135), NetworkManager.Instance.Connected ? Color.Green : Color.Gray);
            TextBoxes[0].Draw(spriteBatch, gameTime);
            TextBoxes[1].Draw(spriteBatch, gameTime);
            foreach (var but in Buttons) but.Draw(spriteBatch, Color.White);
            spriteBatch.End();
        }
    }
}
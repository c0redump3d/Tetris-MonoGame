using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.GUI.Control.Controls;
using Tetris.GUI.DebugMenu;
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
            AddControl(new Button(new Vector2(640, 420), "Connect", Globals.Hoog28));
            AddControl(new Button(new Vector2(640, 510), "Create Server", Globals.Hoog48));
            AddControl(new Button(new Vector2(640, 610), "Back", Globals.Hoog48));
            ((Button)GetControlFromType(typeof(Button), 0)).OnClick += TryConnect;
            ((Button)GetControlFromType(typeof(Button), 1)).OnClick += s => Gui.Instance.SetCurrentScreen(new GuiCreateServer());
            ((Button)GetControlFromType(typeof(Button), 2)).OnClick += MenuClick;
            AddControl(new TextBox(400, 250, "192.168.1.1:9050", 21, @"^[0-9.:]*$"));
            AddControl(new TextBox(400, 350, "Password", 15));
            ButtonsDrawn = true;
            if (NetworkManager.Instance.Connected && !NetworkManager.Instance.IsServer())
            {
                ((Button)GetControlFromType(typeof(Button), 0)).Text = "Disconnect";
                ((Button)GetControlFromType(typeof(Button), 1)).Enabled = false;
            }
            else if(NetworkManager.Instance.IsServer())
            {
                ((Button)GetControlFromType(typeof(Button), 0)).Enabled = false;
            }
        }

        private void TryConnect(object sender)
        {
            try
            {
                if (!NetworkManager.Instance.Connected)
                {
                    NetworkManager.Instance.Connect(((TextBox)GetControlFromType(typeof(TextBox), 0)).Text.Split(':')[0],
                        int.Parse(((TextBox)GetControlFromType(typeof(TextBox), 0)).Text.Split(':')[1]),
                        ((TextBox)GetControlFromType(typeof(TextBox), 1)).Text);
                    Gui.Instance.MultiplayerMessage = "Attempting connection...";
                }
                else
                {
                    NetworkManager.Instance.Disconnect();
                    ((Button)GetControlFromType(typeof(Button), 0)).Text = "Connect";
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
            if (NetworkManager.Instance.Connected && ((Button)GetControlFromType(typeof(Button), 1)).Enabled && !NetworkManager.Instance.IsServer())
            {
                ((Button)GetControlFromType(typeof(Button), 1)).Enabled = false;
                ((Button)GetControlFromType(typeof(Button), 0)).Text = "Disconnect";
            }else if (!NetworkManager.Instance.Connected && !((Button)GetControlFromType(typeof(Button), 1)).Enabled && !NetworkManager.Instance.IsServer())
            {
                ((Button)GetControlFromType(typeof(Button), 1)).Enabled = true;
                ((Button)GetControlFromType(typeof(Button), 0)).Text = "Connect";
            }
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
            ((TextBox)GetControlFromType(typeof(TextBox), 0)).Draw(spriteBatch, gameTime);
            ((TextBox)GetControlFromType(typeof(TextBox), 1)).Draw(spriteBatch, gameTime);
            foreach (var but in Controls) if(but.GetType() == typeof(Button)) ((Button)but).Draw(spriteBatch, Color.White);
            spriteBatch.End();
        }
    }
}
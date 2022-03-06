using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.GUI.Control.Controls;
using Tetris.Multiplayer.Network;
using Tetris.Util;

namespace Tetris.GUI.Screens
{
    public class GuiCreateServer : GuiScreen
    {

        private string Message = "";

        public override void SetUp()
        {
            base.SetUp();
            bool running = NetworkManager.Instance.IsServer();
            AddControl(new Button(new Vector2(640, 510), running ? "Stop Server" : "Start Server", Globals.Hoog48));
            AddControl(new Button(new Vector2(640, 610), "Back", Globals.Hoog48));
            ((Button)GetControlFromType(typeof(Button),0)).OnClick += TryConnect;
            ((Button)GetControlFromType(typeof(Button),1)).OnClick += MenuClick;
            AddControl(new TextBox(400, 250, "9050", 5, @"^[0-9]*$"));
            AddControl(new TextBox(400, 350, "Password", 15));
            ButtonsDrawn = true;
            if (running)
            {
                Message = "Server is running";
            }
            else
            {
                Message = "Server is not running";
            }
        }

        private void TryConnect(object sender)
        {
            bool running = NetworkManager.Instance.IsServer();
            try
            {
                if (!running)
                {
                    NetworkManager.Instance.StartServer(int.Parse(((TextBox)GetControlFromType(typeof(TextBox),0)).Text), ((TextBox)GetControlFromType(typeof(TextBox),1)).Text);
                    ((Button)GetControlFromType(typeof(Button),0)).Text = "Stop Server";
                    Message = "Server is running";
                }
                else
                {
                    NetworkManager.Instance.StopServer();
                    ((Button)GetControlFromType(typeof(Button),0)).Text = "Start Server";
                    Message = "Server is not running";
                }
            }
            catch (Exception)
            {
                Message = "Invalid port!";
            }
        }
        
        private void MenuClick(object sender)
        {
            Gui.SetCurrentScreen(new GuiMultiplayer());
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void DrawScreen(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.DrawScreen(spriteBatch, gameTime);

            bool running = NetworkManager.Instance.IsServer();
            
            if (running && ((Button)GetControlFromType(typeof(Button),0)).Text != "Stop Server")
            {
                ((Button)GetControlFromType(typeof(Button),0)).Text = "Stop Server";
            }
            else if(!running && ((Button)GetControlFromType(typeof(Button),0)).Text != "Start Server")
            {
                if(((Button)GetControlFromType(typeof(Button),0)).Text == "Stop Server")
                    ((Button)GetControlFromType(typeof(Button),0)).Text = "Start Server";
            }
            
            spriteBatch.Begin();
            var mult = Opacity > 0.5f ? 0.5f : Opacity;
            spriteBatch.DrawBorderedRect(new Rectangle(320, 30, 640, 450), PanelBackgroundCol * mult,
                PanelBorderCol * Opacity);
            spriteBatch.DrawCenteredString(Globals.Hoog36, "Create Server", new Vector2(640, 50), Color.White);
            spriteBatch.DrawCenteredString(Globals.Hoog12, Message, new Vector2(640, 135), running ? Color.Green : Color.Gray);
            spriteBatch.DrawStringWithShadow(Globals.Hoog24, "Port:", new Vector2(400, 215), Color.White);
            spriteBatch.DrawStringWithShadow(Globals.Hoog24, "Password:", new Vector2(400, 315), Color.White);
            ((TextBox)GetControlFromType(typeof(TextBox),0)).Draw(spriteBatch, gameTime);
            ((TextBox)GetControlFromType(typeof(TextBox),1)).Draw(spriteBatch, gameTime);
            ((Button)GetControlFromType(typeof(Button),0)).Draw(spriteBatch, Color.White);
            ((Button)GetControlFromType(typeof(Button),1)).Draw(spriteBatch, Color.White);
            spriteBatch.End();
        }
    }
}
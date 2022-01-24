using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.GUI.Elements;
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
            Buttons.Add(new Button(0, new Vector2(640, 510), running ? "Stop Server" : "Start Server", Globals.Hoog48));
            Buttons.Add(new Button(1, new Vector2(640, 610), "Back", Globals.Hoog48));
            Buttons[0].OnClick += TryConnect;
            Buttons[1].OnClick += MenuClick;
            TextBoxes.Add(new(400, 250, "9050", 5, @"^[0-9]*$"));
            TextBoxes.Add(new(400, 350, "Password", 15));
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
                    NetworkManager.Instance.StartServer(int.Parse(TextBoxes[0].Text), TextBoxes[1].Text);
                    Buttons[0].Text = "Stop Server";
                    Message = "Server is running";
                }
                else
                {
                    NetworkManager.Instance.StopServer();
                    Buttons[0].Text = "Start Server";
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
            TextBoxes[0].Update();
            TextBoxes[1].Update();
            foreach (var but in Buttons) but.Update();
            base.Update(gameTime);
        }

        public override void DrawScreen(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.DrawScreen(spriteBatch, gameTime);

            bool running = NetworkManager.Instance.IsServer();
            
            if (running && Buttons[0].Text != "Stop Server")
            {
                Buttons[0].Text = "Stop Server";
            }
            else if(!running && Buttons[0].Text != "Start Server")
            {
                if(Buttons[0].Text == "Stop Server")
                    Buttons[0].Text = "Start Server";
            }
            
            spriteBatch.Begin();
            var mult = Opacity > 0.5f ? 0.5f : Opacity;
            spriteBatch.DrawBorderedRect(new Rectangle(320, 30, 640, 450), PanelBackgroundCol * mult,
                PanelBorderCol * Opacity);
            spriteBatch.DrawCenteredString(Globals.Hoog36, "Create Server", new Vector2(640, 50), Color.White);
            spriteBatch.DrawCenteredString(Globals.Hoog12, Message, new Vector2(640, 135), running ? Color.Green : Color.Gray);
            spriteBatch.DrawStringWithShadow(Globals.Hoog24, "Port:", new Vector2(400, 215), Color.White);
            spriteBatch.DrawStringWithShadow(Globals.Hoog24, "Password:", new Vector2(400, 315), Color.White);
            TextBoxes[0].Draw(spriteBatch, gameTime);
            TextBoxes[1].Draw(spriteBatch, gameTime);
            foreach (var but in Buttons) but.Draw(spriteBatch, Color.White);
            spriteBatch.End();
        }
    }
}
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Multiplayer.Network;
using Tetris.Other;

namespace Tetris.GUI
{
    public class GuiMultiplayer
    {
        private TextBox ipTextBox;
        public bool IsConnecting = false;
        public bool FailedConnect = false;
        private List<Button> buttons;

        public void ShowMultiplayer()
        {
            Instance.GetGui().ClearButtons();
            buttons = new();
            Instance.GetGame().CurrentScreen = 4;//255, 330
            buttons.Add(new Button(0, new Rectangle(327, 360, 0,0), "Connect", Globals.hoog_24));
            if(Server.ServerRunning())
                buttons.Add(new Button(1, new Rectangle(295, 400, 0,0), "Stop Server", Globals.hoog_24));
            else
                buttons.Add(new Button(1, new Rectangle(285, 400, 0,0), "Start Server", Globals.hoog_24));
            buttons.Add(new Button(2, new Rectangle(302, 440, 0,0), "Disconnect", Globals.hoog_24));
            buttons.Add(new Button(3,new Rectangle(293,480,0,0), "MAIN MENU", Globals.hoog_24));
            buttons[0].OnClick += AttemptConnect;
            buttons[1].OnClick += RunStopServer;
            buttons[2].OnClick += Disconnect;
            buttons[3].OnClick += MenuClick;
            buttons[2].Enabled = false;
            if (Server.ServerRunning() || Client.IsConnected())
            {
                buttons[0].Enabled = false;
                if (Client.IsConnected())
                {
                    buttons[1].Enabled = false;
                    buttons[2].Enabled = true;
                }
            }

            ipTextBox = new TextBox(255,310);
        }

        private void MenuClick(object sender)
        {
            Instance.GetGui().ClearButtons();
            Instance.GetGui().AddMenuButtons();
        }

        private void Disconnect(object sender)
        {
            Instance.GetPacket().SendPacketFromName("dis"); // let the server/client know we are disconnecting
            Instance.GetMultiplayerHandler().HideMultiplayer(); // hide the multiplayer screen
            Instance.GetPacket().RunPacketFromName("dis"); // run the disconnect packet on our side.
            
            if(Server.ServerRunning())
                Server.CloseConnection();
            else
                Client.CloseConnection();
            buttons[0].Enabled = true;
            buttons[2].Enabled = false;
            EnableServerButton();
        }

        public void Connected()
        {
            IsConnecting = false;
            buttons[1].Enabled = false;
            buttons[2].Enabled = true;
        }
        
        private void RunStopServer(object sender)
        {
            if (!Server.ServerRunning())
            {
                new Server();
                IsConnecting = false;
                FailedConnect = false;
                buttons[1].Text = "Stop Server";
                buttons[1].Rec = new Rectangle(buttons[1].Rec.X + 10, buttons[1].Rec.Y, buttons[1].Rec.Width,
                    buttons[1].Rec.Height);
                buttons[0].Enabled = false;
            }
            else
            {
                Disconnect(this);
                buttons[1].Text = "Start Server";
                buttons[1].Rec = new Rectangle(buttons[1].Rec.X - 10, buttons[1].Rec.Y, buttons[1].Rec.Width,
                    buttons[1].Rec.Height);
                buttons[0].Enabled = true;
            }
        }

        private void AttemptConnect(object sender)
        {
            if (IsConnecting || Instance.InMultiplayer || Server.ServerRunning())
                return;
            buttons[0].Enabled = false;
            buttons[1].Enabled = false;
            new Client(ipTextBox.Text);
            IsConnecting = true;
            FailedConnect = false;
        }

        public void EnableServerButton()
        {
            buttons[0].Enabled = true;
            buttons[1].Enabled = true;
        }
        
        public void Update()
        {
            ipTextBox.Update();
            foreach (var but in buttons)
            {
                but.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(Globals.hoog_28, "MULTIPLAYER", new Vector2(250, 20), Color.White);
            if(Client.IsConnected())
                spriteBatch.DrawString(Globals.hoog_18, "Connected to server!", new Vector2(257, 100), Color.Green);
            if(FailedConnect)
                spriteBatch.DrawString(Globals.hoog_18, "Failed to connect.", new Vector2(280, 100), Color.Red);
            if(IsConnecting)
                spriteBatch.DrawString(Globals.hoog_18, "Attempting to connect...", new Vector2(242, 100), Color.White);
            if(Server.ServerRunning())
                spriteBatch.DrawString(Globals.hoog_18, "Server is running.", new Vector2(285, 100), Color.Green);
            spriteBatch.DrawString(Globals.hoog_18, "IP Address:", new Vector2(255, 285), Color.White);
            ipTextBox.Draw(spriteBatch, gameTime);
            foreach (var but in buttons)
            {
                but.Draw(spriteBatch, Color.White);
            }
        }
    }
}
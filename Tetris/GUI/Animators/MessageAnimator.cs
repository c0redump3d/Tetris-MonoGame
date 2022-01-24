using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.Game.Managers;
using Tetris.Multiplayer.Network;
using Tetris.Util;

namespace Tetris.GUI.Animators
{
    public class MessageAnimator
    {
        private List<Message> animList;
        
        private MessageAnimator()
        {
            animList = new();
        }
        
        public void Update(GameTime gameTime)
        {
            foreach (Message msg in animList)
            {
                if (msg.Destroy())
                {
                    animList.Remove(msg);
                    break;
                }

                msg.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Message msg in animList)
            {
                msg.Draw(spriteBatch);
            }
        }

        public void AddMessage(Message msg)
        {
            animList.Add(msg);
        }

        private static MessageAnimator _instance;
        public static MessageAnimator Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new MessageAnimator();
                }

                return result;
            }
        }
    }

    public class Message
    {
        public Color Color;
        public SpriteFont Font;
        private readonly float StartTime;
        public float CurTime;
        public float Size = 0f;
        public float Opacity = 0f;

        private bool fade = false;
        
        private List<string> messageList;
        
        public Message(SpriteFont font, string text, Color color, float time = 1000f)
        {
            messageList = new();
            Font = font;
            Color = color;
            CurTime = time;
            StartTime = time;
            BreakMessage(text);
        }

        private void BreakMessage(string text)
        {
            int i;
            var spacing = 320;
            for (;
                 Font.MeasureString(text).X > spacing;
                 text = text.Substring(i)) //split any text that exceeds out spacing
            {
                for (i = 1; i < text.Length && Font.MeasureString(text.Substring(0, i + 1)).X <= spacing; i++)
                {
                }

                BreakMessage(text.Substring(0, i));
            }

            messageList.Insert(0, text);
        }
        
        public bool Destroy() => Size > 2f && Opacity < 0f;

        public void Update(GameTime gameTime)
        {
            CurTime -= gameTime.ElapsedGameTime.Milliseconds;
            if (Size < 2.0f)
            {
                Size += (15f / StartTime);
            }

            if (Opacity < 1.0f && !fade)
            {
                Opacity += (15f / StartTime);
            }
            else
            {
                fade = true;
            }

            if (fade && Opacity > 0f)
            {
                Opacity -= (15f / StartTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int count = 0;
            int startY = 328 - (messageList.Count - 1) * 40;
            spriteBatch.GraphicsDevice.Viewport = !NetworkManager.Instance.Connected ? InGameManager.Instance.NormalViewport : InGameManager.Instance.MultiViewport;
            spriteBatch.Begin(transformMatrix:Matrix.CreateScale(Size));
            foreach (string msg in messageList)
            {
                spriteBatch.DrawCenteredString(Font, msg, new Vector2(0, (startY) + (count*40)), Color, 0.3f * Opacity, Opacity);
                count++;
            }
            spriteBatch.End();
        }
    }
    
}
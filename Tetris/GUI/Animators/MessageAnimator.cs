using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game.Managers;
using Tetris.Multiplayer.Network;
using Tetris.Util;

namespace Tetris.GUI.Animators;

/// <summary>
///     This class is currently not in use, but is maybe planned to replace the ImageAnimator.
/// </summary>
public class MessageAnimator
{
    private static MessageAnimator _instance;

    private readonly List<Message> animList;

    private MessageAnimator()
    {
        animList = new List<Message>();
    }

    public static MessageAnimator Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new MessageAnimator();

            return result;
        }
    }

    public void Update(GameTime gameTime)
    {
        foreach (var msg in animList)
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
        foreach (var msg in animList) msg.Draw(spriteBatch);
    }

    public void AddMessage(Message msg)
    {
        animList.Add(msg);
    }
}

public class Message
{
    private readonly List<string> messageList;
    private readonly float StartTime;
    public Color Color;
    public float CurTime;

    private bool fade;
    public SpriteFont Font;
    public float Opacity;
    public float Size;

    public Message(SpriteFont font, string text, Color color, float time = 1000f)
    {
        messageList = new List<string>();
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

    public bool Destroy()
    {
        return Size > 2f && Opacity < 0f;
    }

    public void Update(GameTime gameTime)
    {
        CurTime -= gameTime.ElapsedGameTime.Milliseconds;
        if (Size < 2.0f) Size += 15f / StartTime;

        if (Opacity < 1.0f && !fade)
            Opacity += 15f / StartTime;
        else
            fade = true;

        if (fade && Opacity > 0f) Opacity -= 15f / StartTime;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var count = 0;
        var startY = 328 - (messageList.Count - 1) * 40;
        spriteBatch.GraphicsDevice.Viewport = !NetworkManager.Instance.Connected
            ? InGameManager.Instance.NormalViewport
            : InGameManager.Instance.MultiViewport;
        spriteBatch.Begin(transformMatrix: Matrix.CreateScale(Size));
        foreach (var msg in messageList)
        {
            spriteBatch.DrawCenteredString(Font, msg, new Vector2(0, startY + count * 40), Color, 0.3f * Opacity,
                Opacity);
            count++;
        }

        spriteBatch.End();
    }
}
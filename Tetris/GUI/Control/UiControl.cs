using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Tetris.Game;
using Tetris.Sound;

namespace Tetris.GUI.Control;

public class UiControl
{
    /// <summary>
    ///     Event is raised when the mouse begins hovering over control.
    /// </summary>
    private readonly EventHandler<Vector2> BeginHover;

    /// <summary>
    ///     Event is raised when the mouse is hovering over control.
    /// </summary>
    private readonly EventHandler<Vector2> Hovering;

    /// <summary>
    ///     Event is raised when the mouse is no longer hovering over control.
    /// </summary>
    private readonly EventHandler<Vector2> HoverRelease;

    /// <summary>
    ///     Event is raised the control is clicked.
    /// </summary>
    private readonly EventHandler<Vector2> OnClick;

    protected MouseState OldMouseState;

    protected UiControl()
    {
        ID = Gui.Instance.CurrentScreen.TotalControls();
        Position = new Vector2();
        Size = new Vector2();
        BeginHover += ControlBeginHovering;
        HoverRelease += ControlStopHover;
        Hovering += ControlHover;
        OnClick += ControlClick;
    }

    /// <summary>
    ///     Returns the assigned ID of the control
    /// </summary>
    public int ID { get; }

    /// <summary>
    ///     Current X/Y position of the control
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    ///     Current Width/Height of the control
    /// </summary>
    public Vector2 Size { get; set; }

    /// <summary>
    ///     Returns whether or not the control can be interacted with.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    ///     Returns the current SpriteFont for text rendering.
    /// </summary>
    protected SpriteFont Font { get; set; } = Fonts.Hoog18;

    /// <summary>
    ///     An assignable text variable used for drawing button text/category information.
    /// </summary>
    public string Text { get; set; } = "";

    /// <summary>
    ///     Returns whether or not the control is being hovered.
    /// </summary>
    public bool Hover { get; private set; }

    private void HandleHover(MouseState mse)
    {
        var mousePos = Gui.Instance.TranslateMousePosition(mse);
        var controlBox = GetHoverRect();
        if (controlBox.Contains(new Point2(mousePos.X, mousePos.Y)) && Enabled &&
            Gui.Instance.CurrentScreen.Opacity >= 1f)
        {
            if (!Hover)
                BeginHover?.Invoke(this, mousePos);

            Hovering?.Invoke(this, mousePos);

            if (OnClick != null && mse.LeftButton == ButtonState.Released &&
                OldMouseState.LeftButton == ButtonState.Pressed)
                // Someone's listening, and we have a click
                OnClick?.Invoke(this, mousePos);
        }
        else
        {
            if (Hover)
                HoverRelease?.Invoke(this, mousePos);
        }
    }

    protected virtual RectangleF GetHoverRect()
    {
        return new RectangleF(Position.X, Position.Y, Size.X, Size.Y);
    }

    protected virtual void ControlBeginHovering(object sender, Vector2 mousePos)
    {
        Hover = true;
    }

    protected virtual void ControlStopHover(object sender, Vector2 mousePos)
    {
        Hover = false;
    }

    protected virtual void ControlHover(object sender, Vector2 mousePos)
    {
    }

    protected virtual void ControlClick(object sender, Vector2 mousePos)
    {
        if (Gui.Instance.CurrentScreen.Closing)
            return;
        //Play click sound event
        Sfx.Instance.PlaySoundEffect("click");
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
    }

    public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
    }

    /// <summary>
    ///     Returns whether or not the controls parent is a panel.
    /// </summary>
    private bool IsPanel()
    {
        if (Gui.Instance.CurrentPanel == null)
            return false;
        if (Gui.Instance.CurrentPanel == this)
            return true;
        if (this == Gui.Instance.CurrentPanel.MinimizeBut)
            return true;
        foreach (var con in Gui.Instance.CurrentPanel.Controls)
            if (con == this)
                return true;

        return false;
    }

    public virtual void Update(GameTime gameTime)
    {
        if (Gui.Instance.CurrentScreen.Closing || !TetrisGame.Instance.IsActive)
            return;
        var mse = Mouse.GetState();
        //Handle hovering of control.
        if (Gui.Instance.CurrentPanel == null)
        {
            HandleHover(mse);
            OldMouseState = mse;
        }
        else
        {
            if (IsPanel() && Gui.Instance.CurrentPanel.Hover)
                HandleHover(mse);
            else if (!Gui.Instance.CurrentPanel.Hover)
                HandleHover(mse);
            else if (!IsPanel() && Gui.Instance.CurrentPanel.Hover)
                if (Hover)
                {
                    var mousePos = Gui.Instance.TranslateMousePosition(mse);
                    HoverRelease?.Invoke(this, mousePos);
                }

            OldMouseState = mse;
        }
    }
}
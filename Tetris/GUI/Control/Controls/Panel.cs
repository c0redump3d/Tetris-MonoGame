using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Tetris.Game;
using Tetris.GUI.UiColor;
using Tetris.Util;

namespace Tetris.GUI.Control.Controls;

/// <summary>
///     A draggable window that holds and draws other controls.
/// </summary>
public class Panel : UiControl
{
    public readonly List<UiControl> Controls;
    public readonly Button MinimizeBut;
    private Vector2 origClick;

    public Panel(string categoryName, int x, int y, bool persistent = false, int maxRow = 5)
    {
        Gui.Instance.CurrentPanel = null;
        Text = categoryName;
        Font = Fonts.Hoog12;
        Position = new Vector2(x, y);
        Controls = new List<UiControl>();
        MaxRowCount = maxRow;
        Persistent = persistent;
        Size = new Vector2(Font.MeasureString(Text).X + 40, 25);

        MinimizeBut = new Button(new Vector2(Position.X + (Size.X - 12), Position.Y + 3), "+",
            Fonts.Hoog12, false);
        MinimizeBut.OnClick += o =>
        {
            UpdatePanControls();
            Hidden = !Hidden;
            MinimizeBut.Text = Hidden ? "+" : "-";
        };
    }

    /// <summary>
    ///     Returns whether or not the panel is in a 'minimized' state or not
    /// </summary>
    public bool Hidden { get; private set; } = true;

    /// <summary>
    ///     Returns whether or not the panel is being dragged or not.
    /// </summary>
    public bool Dragging { get; private set; }

    /// <summary>
    ///     Returns whether or not the panel should be removed when moving to a new screen.
    /// </summary>
    public bool Persistent { get; }

    /// <summary>
    ///     Gets the allowed amount of rows in the panel.
    /// </summary>
    private int MaxRowCount { get; }

    /// <summary>
    ///     Returns whether or not the panel is the active control.
    /// </summary>
    private bool IsActive()
    {
        return Gui.Instance.CurrentPanel != null && Gui.Instance.CurrentPanel == this;
    }

    /// <summary>
    ///     Adds a UiControl to the panel.
    /// </summary>
    public void AddControl(UiControl control)
    {
        Controls.Add(control);
    }

    /// <summary>
    ///     Updates control positions relative to the panels position.
    /// </summary>
    private void UpdatePanControls()
    {
        int row = 0, column = 0, startY = (int) Position.Y + 25;
        int largWidth = 0, largHeight = 0, split = 0;

        int x = (int) Position.X + 20, y = startY;

        foreach (var con in Controls)
        {
            var button = con.GetType() == typeof(Button);
            con.Position = new Vector2(button ? x + con.Size.X / 2 : x, y);
            if ((int) con.Size.X > largWidth)
                largWidth = (int) con.Size.X;
            if ((int) con.Size.Y > largHeight)
                largHeight = (int) con.Size.Y;
            column++;

            y += (int) con.Size.Y + 10;

            if (column == MaxRowCount)
            {
                row++;
                x += largWidth + 20;
                y = startY;

                if (row > 3)
                {
                    x = (int) Position.X + 20;
                    y = LowestY(split);
                    startY = y;
                    row = 0;
                    split++;
                }

                largWidth = 0;
                largHeight = 0;
                column = 0;
            }
        }

        Size = UpdatePanel();
        MinimizeBut.Position = new Vector2(Position.X + (Size.X - 12), Position.Y);
    }

    private int LowestY(int start)
    {
        var offset = start == 0 ? 80 : 40;
        start *= MaxRowCount * 4;
        var tempHeight = 0;
        for (var i = start; i < start + MaxRowCount; i++)
        {
            var con = Controls[i];
            if ((int) con.Position.Y + (int) con.Size.Y > tempHeight)
                tempHeight = (int) con.Position.Y + (int) con.Size.Y;
        }

        return tempHeight + offset;
    }

    /// <summary>
    ///     Returns an updated size of the panel.
    /// </summary>
    private Vector2 UpdatePanel()
    {
        var size = new Vector2(20, 25);
        int tempWidth = 0, tempHeight = 0;
        //Find what the largest control is on the panel.
        for (var f = 0; f < Controls.Count; f++)
        {
            var con = Controls[f];
            if ((int) (con.Position.X - Position.X) + (int) con.Size.X > tempWidth)
                tempWidth = (int) (con.Position.X - Position.X) + (int) con.Size.X;

            if ((int) (con.Position.Y - Position.Y) + (int) con.Size.Y > tempHeight)
                tempHeight = (int) (con.Position.Y - Position.Y) + (int) con.Size.Y;
        }

        //if the new width is larger than current, update it
        if (tempWidth > size.X)
            size.X = tempWidth;

        //same thing, just height
        if (tempHeight > size.Y)
            size.Y = tempHeight;

        size.X += 20;
        size.Y += 25;
        if (Hidden)
            size.Y = 20;

        return size;
    }

    /// <summary>
    ///     Will attempt to set the active panel to the current instance.
    /// </summary>
    private void SetActive()
    {
        var mse = Mouse.GetState();
        //If there is no current panel, there is nothing to be checked.
        if (Gui.Instance.CurrentPanel == null)
        {
            Gui.Instance.CurrentPanel = this;
            return;
        }

        var curPanel = Gui.Instance.CurrentPanel;
        var mouseVec = Gui.Instance.TranslateMousePosition(mse);
        var panelPos = new RectangleF(Position.X, Position.Y, Size.X, Size.Y);
        var mousePos = new RectangleF(mouseVec.X, mouseVec.Y, 1, 1);
        var activePanel = new RectangleF(curPanel.Position.X, curPanel.Position.Y, curPanel.Size.X,
            curPanel.Size.Y);

        //Check to see if active panel is above any other panels, if so we don't want the panel below becoming parent.
        if (activePanel.Intersects(mousePos))
            return;

        //If we are over the panel and click, set it to the current panel.
        if (panelPos.Intersects(mousePos))
            if (OldMouseState.LeftButton == ButtonState.Pressed && mse.LeftButton == ButtonState.Pressed)
                if (!Gui.Instance.CurrentPanel.Dragging)
                    Gui.Instance.CurrentPanel = this;
    }

    /// <summary>
    ///     Updates and handles the panels dragging state.
    /// </summary>
    private void HandleDrag()
    {
        //Make sure it is the active panel!
        if (Gui.Instance.CurrentPanel == this)
        {
            var mse = Mouse.GetState();
            var mousePos = Gui.Instance.TranslateMousePosition(mse);
            var panelPos = new RectangleF(Position.X, Position.Y, Size.X, 20);

            //Check if mouse is over the 'title bar' of the panel and is clicking.
            if (panelPos.Intersects(new RectangleF(mousePos.X, mousePos.Y, 1, 1)))
            {
                if (OldMouseState.LeftButton == ButtonState.Pressed && mse.LeftButton == ButtonState.Pressed)
                {
                    if (!Dragging) origClick = new Vector2(mousePos.X - panelPos.X, mousePos.Y - panelPos.Y);

                    Dragging = true;
                }
                else
                {
                    Dragging = false;
                    //Make sure the panel was not dragged off-screen.
                    MoveOnScreen();
                }
            }

            if (Dragging)
            {
                //This sets the panels position to be offset from the original mouses click position.
                panelPos.X = mousePos.X - origClick.X;
                panelPos.Y = mousePos.Y - origClick.Y;
                Position = new Vector2(panelPos.X, panelPos.Y);
                //Update controls relative to the new position.
                UpdatePanControls();
            }
        }
    }

    /// <summary>
    ///     If the panel is moved off-screen, this will move it back.
    /// </summary>
    private void MoveOnScreen()
    {
        var offScreen = Position;
        if (Position.X > 1280 - Size.X / 2f)
            offScreen.X = 1280 - Size.X / 2f;

        if (Position.X + Size.X / 2f < 0)
            offScreen.X = -(Size.X / 2f);

        if (Position.Y > 720 - Size.Y / 2f)
            offScreen.Y = 720 - Size.Y / 2f;

        if (Position.Y < 5)
            offScreen.Y = 5;
        Position = offScreen;
        UpdatePanControls();
    }

    /// <summary>
    ///     Sets the panel as active when clicked.
    /// </summary>
    protected override void ControlClick(object sender, Vector2 mousePos)
    {
        SetActive();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var panelBackCol = ColorManager.Instance.GuiColor["Panel Background"];
        var panelBordCol = ColorManager.Instance.GuiColor["Panel Border"];
        //Opacity is always 1 if panel is persistent, since it won't be deleted.
        var opac = Persistent ? 1f : Gui.Instance.CurrentScreen.Opacity;
        //Background opacity is darker if it is the current active panel.
        var backOpac = IsActive() ? 0.5f : 0.3f;
        var mult = opac > backOpac ? backOpac : opac;
        //Draws the panels window.
        spriteBatch.DrawBorderedRect(
            new Rectangle((int) Position.X, (int) Position.Y,
                (int) Size.X, (int) Size.Y),
            panelBackCol * mult,
            panelBordCol * opac);
        MinimizeBut.Draw(spriteBatch, Color.White);
        spriteBatch.DrawCenteredString(Fonts.Hoog12, Text, new Vector2(Position.X + Size.X / 2f, Position.Y + 10),
            Color.White);
        //If the panel is not hidden, draw the controls too.
        if (!Hidden)
            foreach (var con in Controls)
            {
                //special draw for textboxes cus they're dumb
                if (con.GetType() == typeof(TextBox))
                    continue;
                con.Draw(spriteBatch);
            }

        base.Draw(spriteBatch);
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (!Hidden)
            foreach (var con in Controls)
                if (con.GetType() == typeof(TextBox))
                    con.Draw(spriteBatch, gameTime);
    }

    public override void Update(GameTime gameTime)
    {
        //If dragging, update the panels position.
        HandleDrag();

        //Update the controls
        UpdatePanControls();

        //Update the minimize button
        if (IsActive())
            MinimizeBut.Update(gameTime);

        //Check if it is possible to be set as the active panel.
        if (!IsActive())
            SetActive();

        if (!Hidden)
            foreach (var con in Controls)
            {
                //Controls are disabled on all but the currently active panel.
                if (!IsActive() && con.Enabled)
                    con.Enabled = false;
                if (IsActive() && !con.Enabled)
                    con.Enabled = true;
                con.Update(gameTime);
            }

        base.Update(gameTime);
    }
}
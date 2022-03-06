using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Tetris.Game;
using Tetris.GUI.DebugMenu;
using Tetris.GUI.UiColor;
using Tetris.Util;

namespace Tetris.GUI.Control.Controls
{
    public class Panel : UiControl
    {
        private Vector2 origClick;
        public bool Dragging { get; set; }
        public bool IsActive() => (Gui.Instance.CurrentPanel != null) && (Gui.Instance.CurrentPanel == this);
        private List<UiControl> controls;
        private int MaxRowCount { get; set; }
        private Vector2 DefaultSize { get; set; }
        private Button minimizeBut;
        private bool hidden = true;
        
        public Panel(string categoryName, int x, int y, int maxRow = 5)
        {
            Gui.Instance.CurrentPanel = null;
            Text = categoryName;
            Font = Globals.Hoog12;
            Position = new Vector2(x, y);
            controls = new List<UiControl>();
            MaxRowCount = maxRow;
            DefaultSize = new (Font.MeasureString(Text).X + 40, 25);
            Size = DefaultSize;

            minimizeBut = new Button(new Vector2(Position.X + (Size.X - 12), Position.Y + 3), "+",
                Globals.Hoog12, false);
            minimizeBut.OnClick += o =>
            {
                UpdatePositions();
                hidden = !hidden;
                minimizeBut.Text = hidden ? "+" : "-";
            };
        }

        public void AddControl(UiControl control)
        {
            controls.Add(control);
        }

        private void UpdatePositions()
        {
            int count = 0;
            int x = (int)Position.X + 20;
            int y = (int)Position.Y + 25;
            float maxX = -9999, maxY = -9999;
            foreach (UiControl con in controls)
            {
                con.Position = new Vector2(x, y + (count * con.Size.Y));
                
                count++;
                if (con.Position.X + con.Size.X > Position.X + Size.X)
                    maxX = con.Size.X;
                if (con.Position.Y + con.Size.Y > Position.Y + Size.Y)
                    maxY = con.Size.Y;
                
                if (count == MaxRowCount)
                {
                    count = 0;
                    x += (int)maxX + 200;
                }
                
                if (hidden)
                    Size = new Vector2(maxX + 40, DefaultSize.Y);
                else
                    Size = new Vector2(maxX + 40, maxY + 35);
            }

            minimizeBut.Position = new Vector2(Position.X + (Size.X - 12), Position.Y);
        }
        
        private void SetActive()
        {
            MouseState mse = Mouse.GetState();
            if (Gui.Instance.CurrentPanel == null)
            {
                Gui.Instance.CurrentPanel = this;
                return;
            }

            var curPanel = Gui.Instance.CurrentPanel;
            Vector2 mouseVec = Gui.Instance.TranslateMousePosition(mse);
            RectangleF panelPos = new RectangleF(Position.X, Position.Y, Size.X, Size.Y);
            RectangleF mousePos = new RectangleF(mouseVec.X, mouseVec.Y, 1, 1);
            RectangleF activePanel = new RectangleF(curPanel.Position.X, curPanel.Position.Y, curPanel.Size.X,
                curPanel.Size.Y);

            //Check to see if active panel is above any other panels, if so we don't want the panel below becoming parent.
            if (activePanel.Intersects(mousePos))
                return;

            if (panelPos.Intersects(mousePos))
            {
                if (OldMouseState.LeftButton == ButtonState.Pressed && mse.LeftButton == ButtonState.Pressed)
                {
                    if (!Dragging)
                    {
                        Gui.Instance.CurrentPanel = this;
                    }
                }
            }
        }

        private void HandleDrag()
        {
            if (Gui.Instance.CurrentPanel == this)
            {
                MouseState mse = Mouse.GetState();
                var mousePos = Gui.Instance.TranslateMousePosition(mse);
                RectangleF panelPos = new RectangleF(Position.X, Position.Y, Size.X, 25);

                if (panelPos.Intersects(new RectangleF(mousePos.X, mousePos.Y, 1, 1)))
                {
                    if (OldMouseState.LeftButton == ButtonState.Pressed && mse.LeftButton == ButtonState.Pressed)
                    {
                        if (!Dragging)
                        {
                            origClick = new Vector2((mousePos.X - panelPos.X), mousePos.Y - panelPos.Y);
                        }

                        Dragging = true;
                    }
                    else
                    {
                        Dragging = false;
                        MoveOnScreen();
                    }
                }

                if (Dragging)
                {
                    panelPos.X = mousePos.X - origClick.X;
                    panelPos.Y = mousePos.Y - origClick.Y;
                    Position = new Vector2(panelPos.X, panelPos.Y);
                    UpdatePositions();
                }
            }
        }
        
        private void MoveOnScreen()
        {
            var offScreen = Position;
            if (Position.X > 1280 - (Size.X / 2f))
                offScreen.X = 1280 - Size.X / 2f;
            
            if (Position.X + Size.X/2f < 0)
                offScreen.X = -(Size.X/2f);

            if (Position.Y > 720 - (Size.Y / 2f))
                offScreen.Y = 720 - Size.Y / 2f;
            
            if (Position.Y < 5)
                offScreen.Y = 5;
            Position = offScreen;
            UpdatePositions();
        }

        public override void ControlClick(object sender, Vector2 mousePos)
        {
            SetActive();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var ButtonBackCol = ColorManager.Instance.GuiColor["Panel Background"];
            var ButtonBorderCol = ColorManager.Instance.GuiColor["Panel Border"];
            var opac = Gui.Instance.CurrentScreen.Opacity;
            float backOpac = IsActive() ? 0.5f : 0.3f;
            var mult = opac > backOpac ? backOpac : opac;
            spriteBatch.DrawBorderedRect(
                new Rectangle((int)Position.X, (int)Position.Y,
                    (int)Size.X, (int)Size.Y),
                ButtonBackCol * mult,
                ButtonBorderCol * opac);
            minimizeBut.Draw(spriteBatch, Color.White);
            spriteBatch.DrawCenteredString(Globals.Hoog12, Text, new Vector2(Position.X + (Size.X/2f), Position.Y + 10), Color.White);
            if (!hidden)
            {
                foreach (var con in controls)
                {
                    if (con.GetType() == typeof(TextBox))
                        continue;
                    con.Draw(spriteBatch);
                }
            }
            base.Draw(spriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var con in controls)
            {
                if (con.GetType() == typeof(TextBox))
                    con.Draw(spriteBatch, gameTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if(IsActive())
                minimizeBut.Update(gameTime);

            HandleDrag();
            if(!IsActive())
                SetActive();
            UpdatePositions();
            
            if (IsActive() && !hidden)
            {
                foreach (var con in controls)
                {
                    if (!IsActive() && con.Enabled)
                        con.Enabled = false;
                    if (IsActive() && !con.Enabled)
                        con.Enabled = true;
                    con.Update(gameTime);
                }
            }
            base.Update(gameTime);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Game;
using Tetris.GUI.Animators;
using Tetris.GUI.Control.Controls;
using Tetris.GUI.Particle;
using Tetris.GUI.Screens;
using Tetris.GUI.UiColor;

namespace Tetris.GUI;

public class Gui
{
    private static Gui _instance;
    public UiColorOption CurrentColorPanel;
    public Panel CurrentPanel;

    public GuiScreen CurrentScreen;

    public TextBox CurrentTextBox = null;

    public string MultiplayerMessage = "";

    //Next screen is used so that we can have a type of transition between the two menus(Fade out old menu, fade in new)
    private GuiScreen nextScreen;
    public bool StartUp = true;

    public static Gui Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new Gui();

            return result;
        }
    }

    public void SetCurrentScreen(GuiScreen screen)
    {
        //If a screen is already being drawn on-screen we want to use the fade animator.
        if (CurrentScreen != null)
        {
            nextScreen = screen;
            CurrentScreen.Closing = true;
        }
        else
        {
            //If no screen had been previously shown, there is no previous menu to set to close.
            CurrentScreen = screen;
            CurrentScreen.SetUp();
        }
    }

    public void DrawPanels(SpriteBatch spriteBatch, GameTime gameTime)
    {
        spriteBatch.Begin();
        if (CurrentScreen != null && CurrentPanel != null)
        {
            foreach (var panel in CurrentScreen.Controls)
                if (panel.GetType() == typeof(Panel) && panel != CurrentPanel)
                {
                    panel.Draw(spriteBatch);
                    panel.Draw(spriteBatch, gameTime);
                }

            CurrentPanel.Draw(spriteBatch);
            CurrentPanel.Draw(spriteBatch, gameTime);
        }

        spriteBatch.End();
    }

    public void DrawGui(SpriteBatch spriteBatch, GameTime gameTime)
    {
        spriteBatch.Begin();
        if (CurrentScreen != null) CurrentScreen.DrawScreen(spriteBatch, gameTime);

        DebugMenu.DebugMenu.Instance.Draw(spriteBatch, gameTime);
        spriteBatch.End();
    }

    public void AddDebugMessage(string msg, [CallerMemberName] string caller = "")
    {
        var console = DebugMenu.DebugMenu.Instance.Console;
        if (console == null)
            return;

        console.AddMessage(msg, caller);
    }

    public void Update(GameTime gameTime)
    {
        if (CurrentScreen != null)
        {
            if (CurrentScreen.Closing && CurrentScreen.Opacity < 0f)
            {
                //Panels have the ability to be persistent, so add back the panels that have this flag.
                var persistentPanels = new List<Panel>();
                foreach (var uiControl in CurrentScreen.Controls)
                    if (uiControl.GetType() == typeof(Panel))
                        if (((Panel) uiControl).Persistent)
                            persistentPanels.Add((Panel) uiControl);
                CurrentScreen = nextScreen;
                CurrentScreen.SetUp();
                foreach (var pan in persistentPanels)
                    CurrentScreen.AddControl(pan);
            }

            foreach (var img in CurrentScreen.ImageAnim.ToList())
                img.Update(gameTime);

            CurrentScreen.Update(gameTime);
        }

        ParticleManager.Instance.UpdateParticles(gameTime);

        DebugMenu.DebugMenu.Instance.Update(gameTime);
    }

    /// <summary>
    /// </summary>
    /// <param name="image"></param>
    /// <param name="time"></param>
    public void AnimateImage(Texture2D image, float time = 1000f, EventHandler runFunc = null)
    {
        if (CurrentScreen.GetType() != typeof(GuiInGame))
        {
            AddDebugMessage("Animating images is disabled for all menus except GuiInGame.");
            return;
        }

        CurrentScreen.ImageAnim.Add(new ImageAnimator(image, time, runFunc));
        AddDebugMessage($"Animating image: {image.Name} for {time}ms.");
    }

    /// <summary>
    ///     Translates the mouse position of the screen to the virtual resolution of the game(1280x720)
    /// </summary>
    public Vector2 TranslateMousePosition(MouseState state)
    {
        var virtualX = Convert.ToSingle(state.X) * Convert.ToSingle(1280) / Convert.ToSingle(Globals.ScreenWidth);
        var virtualY = Convert.ToSingle(state.Y) * Convert.ToSingle(720) / Convert.ToSingle(Globals.ScreenHeight);
        var mousePos = new Vector2(virtualX, virtualY);

        return mousePos;
    }
}
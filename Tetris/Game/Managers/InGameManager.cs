﻿using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game.Events;
using Tetris.Game.InGame;
using Tetris.Game.Mode;
using Tetris.Game.Player;
using Tetris.GUI;
using Tetris.GUI.Animators;
using Tetris.GUI.Particle;
using Tetris.GUI.Screens;
using Tetris.Multiplayer;
using Tetris.Multiplayer.Network;
using Tetris.Sound;
using Tetris.Util;

namespace Tetris.Game.Managers;

/// <summary>
///     Manages all in-game events(NOT menu-related items!)
/// </summary>
public class InGameManager
{
    private static InGameManager _instance;
    public readonly Viewport MultiViewport = new(279, 32, 320, 656);

    public readonly Viewport NormalViewport = new(480, 32, 320, 656);
    private int pinchDirection;
    private float pinchOpacity;
    private bool showPinch;

    public bool Paused { get; private set; }
    public bool Stopped { get; private set; } = true;
    public bool GameOver { get; set; }
    public bool CanMove => !BoardShakeAnimator.Instance.Shaking && !BoardShakeAnimator.Instance.Animating;
    public bool Waiting => PlayerController.Instance.Frozen;
    public bool Winner { get; set; }
    private bool Started { get; set; }
    public bool IsCountdown { get; set; }

    private InGameManager()
    {
        EventManager.Instance.CreateEvent("endgame", EndGame);
        EventManager.Instance.CreateEvent("pause", PauseGame);
    }
    
    public static InGameManager Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new InGameManager();

            return result;
        }
    }

    /// <summary>
    ///     Starts the game.
    /// </summary>
    public void StartGame()
    {
        if (Started)
        {
            Sfx.Instance.PlayPauseMusic(ScoreHandler.Instance.Level);
            IsCountdown = false;
            Paused = false;
            return;
        }

        //hides labels, starts timers and starts to play music
        GameOver = false;
        Winner = false;
        PlayerController.Instance.Reset();
        ModeManager.Instance.GetCurrentMode().SetUp(); // setup events for game mode
        Paused = false;
        Stopped = false;
        Sfx.Instance.PlayMusic(ScoreHandler.Instance.Level);
        PlayerController.Instance.SetGravity(ScoreHandler.Instance.Level);

        RichPresence.Instance.UpdatePresence(); // update discord rpc to reflect new level/score.
        Started = true;
    }

    public void PauseGame()
    {
        if (!Paused)
        {
            Paused = !Paused;
            Sfx.Instance.PlayPauseMusic(ScoreHandler.Instance.Level);
            Sfx.Instance.PlaySoundEffect("pause");
        }
        else
        {
            if (!IsCountdown)
                StartCountdown();
        }

        RichPresence.Instance.UpdatePresence();
    }

    /// <summary>
    ///     Starts the in-game countdown, and once completed will start the actual game.
    /// </summary>
    public void StartCountdown()
    {
        if (Started)
        {
            CountdownAnimator.Instance.BeginCountdown();
            IsCountdown = true;
            return;
        }

        RichPresence.Instance.SetPresence(1);
        //reset stats
        ScoreHandler.Instance.Reset();
        var addZero = ScoreHandler.Instance.Level < 10 ? "0" : "";
        ScoreHandler.Instance.ScoreFont = Fonts.Hoog28;
        ScoreHandler.Instance.LevelText = $@"{addZero}{ScoreHandler.Instance.Level}";
        ScoreHandler.Instance.ScoreText = @"0";
        ScoreHandler.Instance.LineText = @"000";
        CountdownAnimator.Instance.BeginCountdown();
        IsCountdown = true;
    }

    public void DrawGame(SpriteBatch gameBatch, SpriteBatch screenBatch)
    {
        if (Gui.Instance.CurrentScreen is GuiInGame && !Stopped)
        {
            if (NetworkManager.Instance.Connected)
                PlayerMP.Instance.Draw(gameBatch);
            gameBatch.GraphicsDevice.Viewport = !NetworkManager.Instance.Connected ? NormalViewport : MultiViewport;
            BoardShakeAnimator.Instance.StartBatch(gameBatch);
            Prediction.Instance.Draw(gameBatch);
            PlayerController.Instance.DrawPlayer(gameBatch);
            DrawPinch(gameBatch);
            gameBatch.End();
            gameBatch.GraphicsDevice.Viewport = TetrisGame.Instance.DefaultViewport;
            gameBatch.Begin();
            ParticleManager.Instance.DrawParticles(gameBatch);
            gameBatch.End();
            NextShape.Instance.RenderNextShapes(screenBatch);
            HoldShape.Instance.DrawHoldShape(screenBatch);
        }

        gameBatch.GraphicsDevice.Viewport =
            !NetworkManager.Instance.Connected ? new Viewport(480, 32, 320, 656) : MultiViewport;
        foreach (var img in Gui.Instance.CurrentScreen.ImageAnim.ToList())
            img.DrawImage(screenBatch);
        //MessageAnimator.Instance.Draw(gameBatch);
        gameBatch.GraphicsDevice.Viewport = TetrisGame.Instance.DefaultViewport;
    }

    public void UpdateGame(GameTime gameTime)
    {
        if (Stopped && Paused)
            return;
        MessageAnimator.Instance.Update(gameTime);
        PlayerController.Instance.Update(gameTime);
        BoardShakeAnimator.Instance.Update(gameTime);
        Prediction.Instance.BlockCollision();
        ScoreHandler.Instance.Update();
        UpdatePinch();
    }

    /// <summary>
    ///     Draws the pinch overlay on screen when any placed blocks have reached a Y level lower than 128.
    /// </summary>
    private void DrawPinch(SpriteBatch spriteBatch)
    {
        if (showPinch)
        {
            spriteBatch.Draw(Globals.PinchOverlay, new Vector2(0, 0), Color.White * pinchOpacity);

            pinchOpacity += 0.02f * pinchDirection;
            if (pinchOpacity > 1.0)
                pinchDirection = -1;
            if (pinchOpacity < 0.3)
                pinchDirection = 1;
        }
    }

    /// <summary>
    ///     Checks the board for any placed blocks lower than Y level 128.
    /// </summary>
    private void UpdatePinch()
    {
        var found = false;
        for (var i = 0; i < TetrisBoard.Instance.Board.GetLength(0); i++)
            if (i * 32 + Globals.LowestY < 128)
                for (var f = 0; f < 10; f++)
                    if (TetrisBoard.Instance.Board[i, f] != 0)
                        found = true;

        if ((found || ModeManager.Instance.GetCurrentMode().ShowPinch) &&
            CanMove && !Paused)
        {
            showPinch = true;
            Sfx.Instance.PlayPinch();
        }
        else
        {
            showPinch = false;
            Sfx.Instance.StopPinch();
        }
    }

    public void EndCountdown()
    {
        IsCountdown = false;
    }

    /// <summary>
    ///     Stops in-game updates and sets screen to GameOver.
    /// </summary>
    private void EndGame()
    {
        if (!GameOver)
            GameOver = true;
        showPinch = false;
        Paused = true; // pause game
        Stopped = true; // stop music
        Sfx.Instance.StopMusic();
        Gui.Instance.SetCurrentScreen(new GuiGameOver());
        Started = false;
        Gui.Instance.AddDebugMessage("Ended Game");
    }
}
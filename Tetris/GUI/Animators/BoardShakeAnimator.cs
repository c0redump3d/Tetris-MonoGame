using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game.Events;
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.Game.Player;
using Tetris.Sound;
using Tetris.Util;

namespace Tetris.GUI.Animators;

/// <summary>
///     Animator that shakes the game screen when end game event has occured.
/// </summary>
public class BoardShakeAnimator
{
    private static BoardShakeAnimator _instance;
    public bool Animating;
    private Vector2 offset = new(0, 0);
    private bool placedFinished;
    private double shakeRadius = 15;
    private double shakeStartAngle;
    private double timeLeft;
    public double ShakeStart { get; set; }
    public bool Shaking { get; set; }

    public static BoardShakeAnimator Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new BoardShakeAnimator();

            return result;
        }
    }

    public void FinishAnim()
    {
        placedFinished = true;
    }

    public void Update(GameTime gameTime)
    {
        if (Shaking)
        {
            offset = new Vector2((float) (Math.Sin(shakeStartAngle) * shakeRadius),
                (float) (Math.Cos(shakeStartAngle) * shakeRadius));
            shakeRadius -= 0.25f;
            shakeStartAngle += 150 + GameManager.Instance.Random.Next(60);
            PlayerController.Instance.PlyY = 9999;
            if (gameTime.ElapsedGameTime.TotalSeconds - ShakeStart > 2F || shakeRadius <= 0)
            {
                Animating = true;
                Shaking = false;
                shakeRadius = 15;
                shakeStartAngle = 0;
                Sfx.Instance.PlaySoundEffect(InGameManager.Instance.Winner ? "gamewin" : "gameover");
            }
        }


        if (!Animating)
            return;
        timeLeft -= gameTime.ElapsedGameTime.Milliseconds;

        if (timeLeft <= 0)
        {
            if (!placedFinished)
            {
                //Lazy way of keeping player off-screen
                PlayerController.Instance.PlyY = 9999;
                //each tick, move blocks on board down.
                TetrisBoard.Instance.MoveDown();
                timeLeft = 150;
            }
            else
            {
                timeLeft = 0;
                //Once all blocks are off-screen, call end game event.
                RichPresence.Instance.UpdatePresence();
                EventManager.Instance.GetEvent("endgame").Call();
                Animating = false;
                placedFinished = false;
            }
        }
    }

    public void StartBatch(SpriteBatch gameBatch)
    {
        //Don't really know how to feel about this one.
        //When screen shake is animating, it is required to create a translation matrix to actually draw the effect.
        if (Shaking)
            gameBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null,
                Matrix.CreateTranslation(offset.X, offset.Y, 0));
        else
            gameBatch.Begin();
    }
}
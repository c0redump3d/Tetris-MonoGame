using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Game.Events;
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.Game.Mode;
using Tetris.GUI;
using Tetris.GUI.Animators;
using Tetris.GUI.DebugMenu;
using Tetris.GUI.Particle;
using Tetris.GUI.Particle.Particles;
using Tetris.GUI.Screens;
using Tetris.Multiplayer;
using Tetris.Multiplayer.Network;
using Tetris.Settings;
using Tetris.Sound;
using Tetris.Util;

namespace Tetris.Game.Player;

public class PlayerController
{
    private static PlayerController _instance;

    /// <summary>
    ///     The Rectangles that make up the player. (Player[0] = plyX,plyY, Player[1] = R1,R2 and so on)
    /// </summary>
    public readonly Rectangle[] PlayerBlocks = new Rectangle[4];

    /// <summary>
    ///     Represents the X,Y positions of each B rectangle. (ex: Player[1].X = PlayerPos[0])
    /// </summary>
    public readonly int[] PlayerPos = new int[6];

    public int ConfirmTime = -1;

    private Texture2D currentTetImage;
    private double gravityInterval = 1000;
    private double gravityTime;

    public int PlyX;
    public int PlyY;
    public int[] TimeElapsed = new int[2];

    private int TimerWait = 1000;

    private TetrisBoard gameBoard => TetrisBoard.Instance;

    /// <summary>
    ///     Whether or not the player is on the ground(or on top of a block).
    /// </summary>
    public bool Grounded { get; private set; }

    /// <summary>
    ///     If true, player y movement is disabled.
    /// </summary>
    public bool Frozen { get; set; }

    /// <summary>
    ///     Flag for whether the player should be in HardDrop state or not.
    /// </summary>
    private bool HardDrop { get; set; }

    public static PlayerController Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new PlayerController();

            return result;
        }
    }

    private PlayerController()
    {
        EventManager.Instance.CreateEvent("gravity", Gravity);
        EventManager.Instance.CreateEvent("forcedrop", () => HardDrop = true);
        EventManager.Instance.CreateEvent("playerdeath", EndGame);
    }

    /// <summary>
    ///     Returns the current gravity interval.
    /// </summary>
    public double GetGravityTime()
    {
        return gravityInterval;
    }

    private void SetUp()
    {
        if (ScoreHandler.Instance.SelectedLevel > 10)
            gravityInterval = 325 - 5 * (ScoreHandler.Instance.Level - 10);
        TimerWait = 1000;
        TimeElapsed = new int[2];
    }

    private void EndGame()
    {
        for (var l = 0; l < 4; l++)
            PlayerBlocks[l] = new Rectangle();
        for (var l = 0; l < 2; l++)
        for (var i = 0; i < 10; i++)
        for (var f = 0; f < 8; f++)
            ParticleManager.Instance.Create(new Ruble(i * 32, 0));

        TimerUtil.Instance.CreateTimer(5000, () => BoardShakeAnimator.Instance.FinishAnim(), "shakeTime");
        Sfx.Instance.StopPinch();
        Sfx.Instance.StopMusic();
        BoardShakeAnimator.Instance.ShakeStart = Globals.GameTime.ElapsedGameTime.Milliseconds;
        BoardShakeAnimator.Instance.Shaking = true;
    }

    /// <summary>
    ///     This is a very important function. This adds proper collision detection to the tetris blocks
    ///     to allow the blocks to land on top of each other and makes sure the tetris blocks do not go past
    ///     the bottom of the board.
    /// </summary>
    private void BlockCollision(GameTime gameTime)
    {
        if (IsColliding())
        {
            if (ConfirmTime == -1)
                ConfirmTime = 750;
            ConfirmTime -= gameTime.ElapsedGameTime.Milliseconds;
            HoldShape.Instance.DisallowSwap();
            Grounded = true;
            if (ConfirmTime > 0 || PlyY < -112)
                return;
            var tSpin = false;
            if (Rotate.Instance.GetCurShape() == 1) // if we are a t-block, check to see if we did a t-spin
                tSpin = RotateCheck.Instance.TSpinLock();

            //we duplicate the four player rectangles pos
            for (var i = 0; i < 4; i++)
            {
                gameBoard.AddBlockToBoard(Rotate.Instance.GetCurShape(), PlayerBlocks[i].X, PlayerBlocks[i].Y);
                PlayerBlocks[i] = new Rectangle();
                PlyX = 9999;
                PlyY = 9999;
            }

            TetrisBoard.Instance.UpdateRows();

            if (Rotate.Instance.TimesRotated() > 0 && tSpin)
            {
                ScoreHandler.Instance.WasTSpin = true;
                if (GetLinesCleared() == 0)
                {
                    Sfx.Instance.PlaySoundEffect("tspin0");
                }
                else if (GetLinesCleared() == 1)
                {
                    Gui.Instance.AnimateImage(Globals.ScoreTextures[3]);
                    ScoreHandler.Instance.Bonuses[0]++;
                    ScoreHandler.Instance.SetBonus(800);
                }
                else if (GetLinesCleared() == 2)
                {
                    Gui.Instance.AnimateImage(Globals.ScoreTextures[4]);
                    ScoreHandler.Instance.Bonuses[1]++;
                    ScoreHandler.Instance.SetBonus(1200);
                }
                else if (GetLinesCleared() == 3)
                {
                    Gui.Instance.AnimateImage(Globals.ScoreTextures[5]);
                    ScoreHandler.Instance.Bonuses[2]++;
                    ScoreHandler.Instance.SetBonus(1600);
                }
            }

            ConfirmTime = -1;
            SetPlayerShape(NextShape.Instance.GetNextShape(), true);
            Sfx.Instance.PlaySoundEffect("fall");

            if (HardDrop)
                Sfx.Instance.PlaySoundEffect("harddrop");

            HoldShape.Instance.AllowSwap();
            NetworkManager.Instance.SendPacket(2); // Send row packet
            HardDrop = false;
        }
        else
        {
            Grounded = false;
        }
    }

    private void Drop()
    {
        //This is now run on every tick until the player has successfully locked in place.
        for (var i = 0; i < 25; i++)
        {
            //If we have not reached the bottom, keep moving the player down.
            if (!IsColliding())
            {
                PlyY += 32;
                ParticleManager.Instance.Create(new Star(PlyX + 16, PlyY - 16));
                ScoreHandler.Instance.Score += 2;
            }
            else
            {
                ConfirmTime = -2;
            }

            UpdateRectangles();
        }
    }

    /// <summary>
    ///     Checks if the player is above any blocks within the PlacedRect array.
    /// </summary>
    /// <returns>True if player is above any blocks</returns>
    public bool IsColliding()
    {
        UpdateRectangles();
        if (gameBoard.WouldCollide(PlayerBlocks))
            return true;

        if (PlayerBlocks[0].Y == Globals.MaxY || PlayerBlocks[1].Y == Globals.MaxY ||
            PlayerBlocks[2].Y == Globals.MaxY ||
            PlayerBlocks[3].Y == Globals.MaxY)
            return true;

        return false;
    }

    /// <summary>
    ///     Calculates the gravity for the given level (equation from: https://tetris.fandom.com/wiki/Tetris_Worlds#Gravity)
    /// </summary>
    /// <param name="level">Current level</param>
    public void SetGravity(int level)
    {
        if (level > 20)
            level = 20;

        gravityInterval = Math.Round(Math.Pow(0.8 - (level - 1) * 0.007, level - 1), 5) * 1000;
        Gui.Instance.AddDebugMessage($"Set gravity to {gravityInterval}ms");
    }

    /// <summary>
    ///     Gives the amount of lines that the player last cleared.
    /// </summary>
    /// <returns>Lines cleared</returns>
    internal int GetLinesCleared()
    {
        return gameBoard.GetTotalLines();
    }

    /// <summary>
    ///     When called, will drop player down by 1(32) if it meets the given conditions(ply is not over any blocks or at
    ///     bottom)
    /// </summary>
    public void Gravity()
    {
        // if player is above an already placed shape, we return.
        if (IsColliding() || Frozen || InGameManager.Instance.Paused)
            return;

        gravityTime -= Globals.GameTime.ElapsedGameTime.Milliseconds;

        if (gravityTime <= 0)
        {
            // make sure we aren't at the bottom of the board.
            if (!IsColliding() && !Keyboard.GetState().IsKeyDown((Keys) GameSettings.Instance.GetOptionValue("Down")))
                PlyY += 32;
            NetworkManager.Instance.SendPacket(3);
            gravityTime = gravityInterval;
        }
    }

    /// <summary>
    ///     Resets all player variables to default values.
    /// </summary>
    public void Reset()
    {
        Gui.Instance.AddDebugMessage("Game Started");
        gameBoard.Reset();

        Globals.CurrentLevelUpImage = Globals.LevelUpTextures[0];
        NextShape.Instance.ResetList(); // reset shape list and shuffle
        NextShape.Instance.GenerateNextShape(); // get our next shape
        SetPlayerShape(NextShape.Instance.GetNextShape(), true); // set the player to the shape

        SetUp();
        gravityTime = 0;
        HardDrop = false;
    }

    public void Update(GameTime gameTime)
    {
        if (HardDrop) Drop();
        //call gravity event to cause player to fall.
        EventManager.Instance.GetEvent("gravity").Call();

        //Used for keeping track of the elapsed time of the current game
        TimerWait -= gameTime.ElapsedGameTime.Milliseconds;

        if (NetworkManager.Instance.Connected)
            PlayerMP.Instance.Update();

        TetrisBoard.Instance.UpdateLasers(gameTime);

        ModeManager.Instance.UpdateCurrentMode(gameTime);

        BlockCollision(gameTime);
        gameBoard.HitTop();

        if (TimerWait <= 0 && InGameManager.Instance.CanMove && !InGameManager.Instance.Paused)
        {
            if (TimeElapsed[1] != 59)
            {
                TimeElapsed[1]++;
            }
            else
            {
                TimeElapsed[1] = 0;
                TimeElapsed[0]++;
            }

            TimerWait = 1000;
        }
    }

    public void DrawPlayer(SpriteBatch spriteBatch)
    {
        UpdateRectangles();
        //draw player rectangles
        for (var i = 0; i < 4; i++) spriteBatch.Draw(currentTetImage, PlayerBlocks[i], Color.White);

        for (var i = 0; i < 22; i++)
        for (var f = 0; f < 10; f++)
        {
            if (gameBoard.Board[i, f] == 0)
                continue;
            spriteBatch.Draw(Globals.BlockTexture[gameBoard.Board[i, f] - 1],
                new Rectangle(f * 32, i * 32 + Globals.LowestY, 32, 32), Color.White);
        }

        if (gameBoard.Opacity > 0f) gameBoard.Opacity -= 0.05f;

        TetrisBoard.Instance.DrawLasers(spriteBatch);

        if (InGameManager.Instance.Paused && !InGameManager.Instance.IsCountdown &&
            Gui.Instance.CurrentScreen is GuiInGame && !Gui.Instance.CurrentScreen.Closing)
            spriteBatch.DrawCenteredString(Fonts.Hoog36, @"PAUSED", new Vector2(160, 320), Color.White);

        ModeManager.Instance.DrawCurrentMode(spriteBatch);

        DebugMenu.Instance.DrawPlayerDebug(spriteBatch);
    }

    /// <summary>
    ///     Updates the player rectangles to the current position and rotations.
    /// </summary>
    public void UpdateRectangles()
    {
        PlayerBlocks[0] = new Rectangle(PlyX, PlyY, 32, 32); // player controlled rect
        PlayerBlocks[1] = new Rectangle(PlyX + PlayerPos[0], PlyY + PlayerPos[1], 32, 32);
        PlayerBlocks[2] = new Rectangle(PlyX - PlayerPos[2], PlyY - PlayerPos[3], 32, 32);
        PlayerBlocks[3] = new Rectangle(PlyX + PlayerPos[5], PlyY - PlayerPos[4], 32, 32);
    }

    /// <summary>
    ///     Sets the player to the given shape.
    ///     Note: generateNew is only false if you basically want to swap out the current players shape without interfering
    ///     with NextShape
    /// </summary>
    /// <param name="shape">The desired shape</param>
    /// <param name="generateNew">Generate next shape</param>
    public void SetPlayerShape(int shape, bool generateNew)
    {
        if (BoardShakeAnimator.Instance.Animating)
            return;

        //reset player pos
        PlyY = shape == 5 ? -144 : -112;
        PlyX = 128;

        Rotate.Instance.SetCurShape(shape);
        RotateCheck.Instance.SetAllPositions();

        //set player to what ever number was selected
        SetShape();

        for (var i = 0; shape == 4 ? i < 2 : i < 3; i++)
        {
            UpdateRectangles();
            if (!IsColliding())
                PlyY += 32;
        }

        gravityTime = 0;

        if (!generateNew) return;
        NextShape.Instance.GenerateNextShape();
        NextShape.Instance.SetNextShapes();
        NetworkManager.Instance.SendPacket(5);
    }

    public void SetShape()
    {
        var shape = Rotate.Instance.GetCurShape() - 1;

        for (var i = 0; i < PlayerPos.Length; i++)
            PlayerPos[i] = Rotate.Instance.Blocks[shape, 0, i] * 32;
        currentTetImage = Globals.BlockTexture[shape];
        Rotate.Instance.ResetRot();
        RotateCheck.Instance.SetAllPositions(); // update the rotate check positions
    }
}
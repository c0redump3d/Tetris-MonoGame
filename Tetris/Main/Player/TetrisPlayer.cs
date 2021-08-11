using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;
using Tetris.GameDebug;
using Tetris.Other;

namespace Tetris.Main.Player
{
    public class TetrisPlayer
    {

        /// <summary>
        /// Holds all currently placed rectangles on GameBoard.
        /// </summary>
        public Rectangle[] PlacedRect;
        /// <summary>
        /// Stores the textures of the PlacedRectangles on GameBoard.
        /// </summary>
        public Texture2D[] StoredImage;
        /// <summary>
        /// The Rectangles that make up the player. (Player[0] = plyX,plyY, Player[1] = R1,R2 and so on)
        /// </summary>
        public readonly Rectangle[] Player = new Rectangle[4];
        public int PlyX = 0;
        public int PlyY = 0;
        /// <summary>
        /// Whether or not the player is on the ground(or on top of a block).
        /// </summary>
        public bool Grounded { get; private set; }

        private int randX;
        private float gravityInterval = 1000;
        private double gravityTime = 0;

        public bool PlacedAnimation = false;
        private bool placedFinished = false;
        private double timeLeft = 0;
        private float pinchOpacity = 0;
        private int pinchDirection = 0;
        private bool showPinch = false;
        
        /// <summary>
        /// Represents the X,Y positions of each B rectangle. (ex: Player[1].X = PlayerPos[0])
        /// </summary>
        public readonly int[] PlayerPos = new int[6];

        private Texture2D currentTetImage;
        public int ConfirmTime = -1;
        private bool forceDrop = false;
        private RowCheck checkRow;
        private readonly Random rand;
        private readonly Prediction predict;

        public TetrisPlayer()
        {
            PlacedRect = new Rectangle[0];
            StoredImage = new Texture2D[0];
            checkRow = Instance.GetRowCheck();
            predict = Instance.GetPredict();
            rand = Instance.GetRandom();
        }

        private void SetUp()
        {
            PlacedRect.Add(new Rectangle(0,9999,0,0),Globals.BlockTexture[7]);
            PlacedRect.Add(new Rectangle(0,9999,0,0),Globals.BlockTexture[7]);
        }

        /// <summary>
        /// This function is used to check if any placed tetris blocks reaches the top of the screen.
        /// If it does, it will end the game.
        /// </summary>
        private void HitTop(GameTime gameTime)
        {
            if (PlacedAnimation || Instance.GetGame().ScreenShake)
                return;
            for (int i = PlacedRect.Length - 1; i > 1; i--)
            {
                for (int f = 0; f < 4; f++)
                {
                    if (PlacedRect[i].Y == Globals.TopOut) // if placed rectangle reaches top of board, end game.
                    {
                        Debug.DebugMessage($"Rectangle {i} reached top of screen.", 1);
                        for (int l = 0; l < 4; l++)
                            Player[l] = new();
                        showPinch = false;
                        Instance.GetSound().StopPinch();
                        Instance.GetSound().StopMusic();
                        Instance.GetGame().ShakeStart = gameTime.ElapsedGameTime.Milliseconds;
                        Instance.GetGame().ScreenShake = true;
                        Instance.GetPacket().SendPacketFromName("end");
                        return;
                    }
                }
            }
        }
        
        private void UpdatePlacedAnim(GameTime gameTime)
        {
            if (!PlacedAnimation)
                return;
            timeLeft -= gameTime.ElapsedGameTime.Milliseconds;
            bool done = true;
            
            if (timeLeft <= 0)
            {
                if (!placedFinished)
                {
                    PlyY = 9999;
                    for (int i = 0; i < PlacedRect.Length; i++)
                    {
                        PlacedRect[i].Y += 32;

                        if (PlacedRect[i].Y < 800)
                        {
                            done = false;
                        }
                    }
                    timeLeft = 150;

                    if (done)
                    {
                        placedFinished = true;
                    }
                }
                else
                {
                    timeLeft = 0;
                    placedFinished = false;
                    PlacedAnimation = false;
                    Instance.GetGame().GameOver = true;
                    Instance.GetGame().EndGame();
                }
            }
        }

        /// <summary>
        /// This is a very important function. This adds proper collision detection to the tetris blocks
        /// to allow the blocks to land on top of each other and makes sure the tetris blocks do not go past
        /// the bottom of the board.
        /// </summary>
        /// <param name="confirm"></param>
        /// <param name="hardDrop"></param>
        /// <param name="remove"></param>
        /// <param name="level"></param>
        private void BlockCollision(GameTime gameTime)
        {
            try
            {
                if (IsColliding())
                {
                    if (ConfirmTime == -1)
                        ConfirmTime = 750;
                    ConfirmTime -= gameTime.ElapsedGameTime.Milliseconds;
                    if (forceDrop)
                        ConfirmTime = -1;
                    Instance.GetHoldShape().DisallowSwap();
                    Grounded = true;
                    if (ConfirmTime > 0 || PlyY < Globals.TopOut)
                        return;
                    bool tSpin = false;
                    if (Instance.GetRotate().GetCurShape() == 1) // if we are a tblock, check to see if we did a tspin
                        tSpin = Instance.GetRotateCheck().TSpinLock();

                    //we duplicate the four player rectangles pos
                    for (int i = 0; i < 4; i++)
                    {
                        PlacedRect.Add(Player[i], Globals.BlockPlacedTexture[Instance.GetRotate().GetCurShape()-1]);
                        Player[i] = new();
                    }

                    checkRow.UpdateRowCheck(ref PlacedRect);

                    if (Instance.GetRotate().TimesRotated() > 0 && tSpin)
                    {
                        Instance.GetScoreHandler().WasTSpin = true;
                        if (GetLinesCleared() == 0)
                        {
                            Instance.GetSound().PlaySoundEffect("tspin0");
                        }
                        else if (GetLinesCleared() == 1)
                        {
                            Globals.scoreTextures[3].AnimateImage(new Vector2(52, 250));
                            Instance.GetScoreHandler().Bonuses[0]++;
                            Instance.GetScoreHandler().SetBonus(800);
                        }
                        else if (GetLinesCleared() == 2)
                        {
                            Globals.scoreTextures[4].AnimateImage(new Vector2(40, 250));
                            Instance.GetScoreHandler().Bonuses[1]++;
                            Instance.GetScoreHandler().SetBonus(1200);
                        }
                        else if (GetLinesCleared() == 3)
                        {
                            Globals.scoreTextures[5].AnimateImage(new Vector2(53, 250));
                            Instance.GetScoreHandler().Bonuses[2]++;
                            Instance.GetScoreHandler().SetBonus(1600);
                        }
                    }

                    if (GetLinesCleared() > 1 && Instance.GetScoreHandler().Level > 6)
                    {
                        RandomBlock(1);
                    }

                    SetPlayerShape(Instance.GetNextShape().GetNextShape(), true);
                    ConfirmTime = -1;

                    if (forceDrop)
                        Instance.GetSound().PlaySoundEffect("harddrop");
                    else
                        Instance.GetSound().PlaySoundEffect("fall");

                    forceDrop = false;
                    
                    Instance.GetHoldShape().AllowSwap();

                    Instance.GetPacket().SendPacketFromName("plc");
                }
                else { Grounded = false; }
            }// Don't worry to much if exception is raised, OutOfBoundsException can be raised if PlacedRect is modified while looping through its content
            catch (Exception) { }
        }

        public bool InstantFall()
        {
            if (PlyY < -48)
                return false;
            forceDrop = true;
            for (int i = 0; i < 25; i++)
            {
                if (!IsColliding())
                    PlyY += 32;
                else
                    break;
                UpdateRectangles();
            }

            return true;
        }

        /// <summary>
        /// Checks if the player is above any blocks within the PlacedRect array.
        /// </summary>
        /// <returns>True if player is above any blocks</returns>
        private bool IsColliding()
        {
            for (int i = PlacedRect.Length - 1; i > 0; i--)
                if (Player[0].Y == PlacedRect[i].Y - 32 && Player[0].X == PlacedRect[i].X
                    || Player[1].Y == PlacedRect[i].Y - 32 && Player[1].X == PlacedRect[i].X
                    || Player[2].Y == PlacedRect[i].Y - 32 && Player[2].X == PlacedRect[i].X
                    || Player[3].Y == PlacedRect[i].Y - 32 && Player[3].X == PlacedRect[i].X)
                    return true;

            if (Player[0].Y == Globals.MaxY || Player[1].Y == Globals.MaxY || Player[2].Y == Globals.MaxY ||
                Player[3].Y == Globals.MaxY)
                return true;

            return false;
        }
        
        /// <summary>
        /// Sets predefined gravity intervals for game, if # > 9, we subtract 5 each level
        /// </summary>
        /// <param name="level">Current level</param>
        public void SetGravity(int level)
        {
            gravityInterval = level switch
            {
                1 => 1000,
                2 => 925,
                3 => 850,
                4 => 775,
                5 => 700,
                6 => 625,
                7 => 550,
                8 => 475,
                9 => 400,
                10 => 325,
                _ => gravityInterval > 50 ? gravityInterval - 5 : 50
            };
        }
        
        /// <summary>
        /// Gives the amount of lines that the player last cleared.
        /// </summary>
        /// <returns>Lines cleared</returns>
        internal int GetLinesCleared()
        {
            return checkRow.GetLinesCleared();
        }
        
        /// <summary>
        /// When called, will drop player down by 1(32) if it meets the given conditions(ply is not over any blocks or at bottom)
        /// </summary>
        public void Gravity(GameTime gameTime)
        {
            // if player is above an already placed shape, we return.
            if (IsColliding() || forceDrop)
                return;

            gravityTime -= gameTime.ElapsedGameTime.Milliseconds;

            if (gravityTime <= 0)
            {
                if (Player[0].Y != Globals.MaxY && Player[1].Y != Globals.MaxY && Player[2].Y != Globals.MaxY &&
                    Player[3].Y != Globals.MaxY &&
                    !Keyboard.GetState().IsKeyDown(Keys.S)) // make sure we aren't at the bottom of the board.
                    PlyY += 32;
                gravityTime = gravityInterval;
                Instance.GetPacket().SendPacketFromName("pos"); // send current player position to client/server
            }
        }

        /// <summary>
        /// Resets the entire game(player, nextShape, placedRect, storedColor, etc) to their default values.
        /// </summary>
        public void Reset()
        {
            Debug.DebugMessage("GAME: Start", 1);

            Instance.CurrentLevelUpImage = Globals.levelUpTextures[0];
            Instance.GetNextShape().ResetList(); // reset shape list and shuffle
            Instance.GetNextShape().GenerateNextShape(); // get our next shape
            SetPlayerShape(Instance.GetNextShape().GetNextShape(), true); // set the player to the shape

            checkRow.Reset();
            PlacedRect = new Rectangle[0];
            StoredImage = new Texture2D[0];
            SetUp();
            checkRow = Instance.ResetRow();
            gravityTime = 0;
            forceDrop = false;
        }

        public void Update(GameTime gameTime)
        {
            bool found = false;
            for (int i = 0; i < PlacedRect.Length; i++)
            {
                if (PlacedRect[i].Y < 128)
                    found = true;
            }

            if (found && !PlacedAnimation)
            {
                showPinch = true;
                Instance.GetSound().PlayPinch();
            }
            else
            {
                showPinch = false;
                Instance.GetSound().StopPinch();
            }

            BlockCollision(gameTime);
            HitTop(gameTime);
            UpdatePlacedAnim(gameTime);
        }
        
        public void DrawPlayer(SpriteBatch _spriteBatch, GameTime gameTime)
        {
            UpdateRectangles();
            //draw player rectangles
            for (int i = 0; i < 4; i++)
            {
                _spriteBatch.Draw(currentTetImage, Player[i], Color.White);
            }
            for (int i = 0; i < PlacedRect.Length; i++)
            {
                _spriteBatch.Draw(StoredImage[i], PlacedRect[i], Color.White);
            }
            
            if (showPinch)
            {
                _spriteBatch.Draw(Globals.pinchOverlay, new Vector2(0,0), Color.White * pinchOpacity);
                
                pinchOpacity += 0.02f * pinchDirection;
                if (pinchOpacity > 1.0)
                    pinchDirection = -1;
                if (pinchOpacity < 0.3)
                    pinchDirection = 1;
            }

            if (Debug.IsEnabled() && Debug.GetSelection() == 1)
            {
                Rectangle[] positions = {Player[0], Player[1], Player[2], Player[3]};
                int lastY = 0;
                for (int i = 0; i < positions.Length; i++)
                {
                    string blPos = "";
                    switch (i)
                    {
                        case 1:
                            blPos = $"R1: {PlayerPos[0]} R2: {PlayerPos[1]}";
                            break;
                        case 2:
                            blPos = $"L1: {PlayerPos[2]} L2: {PlayerPos[3]}";
                            break;
                        case 3:
                            blPos = $"T1: {PlayerPos[4]} T2: {PlayerPos[5]}";
                            break;
                    }

                    _spriteBatch.DrawString(Globals.hoog_12, $"({i})X: {positions[i].X} Y: {positions[i].Y} {blPos}", new Vector2(0, (i * 18)), Color.White * 0.5F);
                    lastY = i + 1;
                }

                _spriteBatch.DrawString(Globals.hoog_12, "ply", new Vector2(Player[0].X + 3, Player[0].Y + 5), Color.Black);
                _spriteBatch.DrawString(Globals.hoog_12, "rgt", new Vector2(Player[1].X + 4, Player[1].Y + 5), Color.Black);
                _spriteBatch.DrawString(Globals.hoog_12, "lft", new Vector2(Player[2].X + 5, Player[2].Y + 5), Color.Black);
                _spriteBatch.DrawString(Globals.hoog_12, "top", new Vector2(Player[3].X + 3, Player[3].Y + 5), Color.Black);
                _spriteBatch.DrawString(Globals.hoog_12, $"shape/ang: {Instance.GetRotate().GetCurShape()},{Instance.GetRotate().GetCurAngle()}", new Vector2(0, lastY * 18), Color.White * 0.5F);
                _spriteBatch.DrawString(Globals.hoog_12, $"gravity: {gravityInterval}ms", new Vector2(0, ++lastY * 18), Color.White * 0.5F);
                _spriteBatch.DrawString(Globals.hoog_12, $"grounded: {Grounded}", new Vector2(0, ++lastY * 18), Color.White * 0.5F);
                _spriteBatch.DrawString(Globals.hoog_12, $"framerate: {(int)(1 / gameTime.ElapsedGameTime.TotalSeconds)}", new Vector2(0,++lastY * 18), Color.White * 0.5F);
                for (int i = 0; i < 4; i++)
                    _spriteBatch.Draw(currentTetImage,
                        Instance.GetRotateCheck().GetRotationBlocks()[i], Color.White * 0.2F);
            }
        }

        private void UpdateRectangles()
        {
            Player[0] = new Rectangle(PlyX, PlyY, 32, 32); // player controlled rect
            Player[1] = new Rectangle(PlyX + PlayerPos[0], PlyY + PlayerPos[1], 32, 32);
            Player[2] = new Rectangle(PlyX - PlayerPos[2], PlyY - PlayerPos[3], 32, 32);
            Player[3] = new Rectangle(PlyX + PlayerPos[5], PlyY - PlayerPos[4], 32, 32);
        }

        /// <summary>
        /// Sets the player to the given shape.
        /// Note: generateNew is only false if you basically want to swap out the current players shape without interfering with NextShape
        /// </summary>
        /// <param name="shape">The desired shape</param>
        /// <param name="generateNew">Generate next shape</param>
        public void SetPlayerShape(int shape, bool generateNew)
        {
            if (PlacedAnimation)
                return;
            //reset player pos
            PlyY = -112;
            PlyX = 160;

            Instance.GetRotate().SetCurShape(shape);
            Instance.GetRotateCheck().SetAllPositions();

            //set player to what ever number was selected
            SetShape();

            for (int i = 0; i < 3; i++) {
                UpdateRectangles();
                if (!IsColliding())
                    PlyY += 32;
            }

            gravityTime = 0;

            if (!generateNew) return;
            Instance.GetNextShape().GenerateNextShape();
            Instance.GetNextShape().SetNextShapes();
        }

        public void SetShape()
        {
            int shape = Instance.GetRotate().GetCurShape() - 1;

            for (int i = 0; i < PlayerPos.Length; i++)
                PlayerPos[i] = Instance.GetRotate().blocks[shape, 0, i] * 32;
            currentTetImage = Globals.BlockTexture[shape];
            Instance.GetRotate().ResetRot();
            Instance.GetRotateCheck().SetAllPositions(); // update the rotate check positions
            Debug.DebugMessage("PLAYER: Shape set(" + Instance.GetRotate().GetCurShape() + ")", 1);
        }
        
        /// <summary>
        /// Generates a row with one random hole. (Mainly used for multiplayer and higher levels)
        /// </summary>
        /// <param name="rows">Rows to add to board</param>
        public void RandomBlock(int rows)
        {
            int y = Globals.MaxY;
            Debug.DebugMessage($"GAME: Generating {rows} random row(s)", 1);
            
            for (int f = 0; f < rows; f++)
            for (int g = PlacedRect.Length - 1; g > 0; g--)
                PlacedRect[g].Y -= 32; // move all placedRectangles up to make room for new row.

            for (int f = 0; f < rows; f++) {
                
                if(PlyY > -16) // lazy way of fixing collision issues with rows.
                    PlyY -= 32;

                int x = 0;
                randX = rand.Next(1, 11); // where the empty hole will be

                for (int i = PlacedRect.Length - 1; i > 1; i--)
                    if (x == PlacedRect[i].X && y == PlacedRect[i].Y) // if there is for some reason a block at the location already, do not place
                        continue;

                for (int i = 10; i > 0; i--)
                {
                    if (randX != i)
                    {
                        PlacedRect.Add(new(x, y, 32, 32), Globals.BlockPlacedTexture[7]);
                    }
                    x += 32;
                }
                y -= 32;
            }
            Instance.GetPacket().SendPacketFromName("plc"); // send updated placedRect to client/server.
        }

    }
}
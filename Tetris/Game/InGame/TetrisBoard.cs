using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game.Events;
using Tetris.Game.Managers;
using Tetris.Game.Mode;
using Tetris.Game.Player;
using Tetris.GUI;
using Tetris.GUI.Animators;
using Tetris.GUI.Particle;
using Tetris.GUI.Particle.Particles;
using Tetris.Multiplayer.Network;
using Tetris.Sound;
using Tetris.Util;

namespace Tetris.Game.InGame;

/// <summary>
///     TetrisBoard.cs is the actual game board the player plays within.
///     This class keeps check of how many blocks are on the board, and where.
///     Along with that, it detects when a row is full (all 10 spaces on X coord)
///     and correspondingly, removes them.
/// </summary>
public class TetrisBoard
{
    private static TetrisBoard _instance;
    //Very happy with the newly refactored version of this class :).

    public int[,] Board = new int[22, 10];
    private int greyLines;
    private List<BlockAnimator> laser = new();
    public float Opacity;
    private List<int> rowsToRemove = new();
    private int totalLines;
    private int yLev = -48;

    private TetrisBoard()
    {
        EventManager.Instance.CreateEvent("rowfall", FallDown);
    }
    
    public static TetrisBoard Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new TetrisBoard();

            return result;
        }
    }

    /// <summary>
    ///     Adds a specific block to the given position.
    /// </summary>
    public void AddBlockToBoard(int shape, int x, int y)
    {
        for (var i = 0; i < 22; i++)
        for (var f = 0; f < 10; f++)
            if (f * 32 == x && i * 32 + Globals.LowestY == y)
            {
                Board[i, f] = shape;
                break;
            }
    }

    public void PrintToConsole()
    {
        for (var i = 0; i < 22; i++)
        {
            var row = "";
            for (var f = 0; f < 10; f++) row += $"{Board[i, f]},";
            Gui.Instance.AddDebugMessage($"Row:{i}:{row}", null);
        }
    }

    /// <summary>
    ///     Checks each row to see if the current row is full and if so, it is added to a list to be removed.
    /// </summary>
    public void UpdateRows()
    {
        totalLines = 0;
        greyLines = 0;
        for (var col = 0; col < Board.GetLength(0); col++)
        {
            var blocks = 0;
            //check to see if column is full
            for (var i = 0; i < 10; i++)
                if (Board[col, i] !=
                    0) // if the block space is not empty, add to the total count of blocks for the given row
                    blocks++;

            if (blocks == 10) // if full, add to list to be removed
                rowsToRemove.Add(col);
        }

        RemoveFullRows(); // remove the rows added to the list
    }

    public void Reset()
    {
        laser = new List<BlockAnimator>();
        Board = new int[22, 10];
        totalLines = 0;
        greyLines = 0;
        rowsToRemove = new List<int>();
        yLev = -48;
    }

    /// <summary>
    ///     After blocks have been removed from the array, move all remaining rows down(No empty rows in-between)
    /// </summary>
    private void Fall()
    {
        var gameBoardTempRow = Board.GetLength(0) - 1;
        var gameBoardTemp = new int[22, 10];

        for (var row = Board.GetLength(0) - 1; row > 0; row--)
            if (!rowsToRemove.Contains(row)) // only copy data from rows that aren't going to be removed.
            {
                for (var col = 0; col < Board.GetLength(1); col++)
                    gameBoardTemp[gameBoardTempRow, col] = Board[row, col]; // set the temp value
                gameBoardTempRow--;
            }
            else
            {
                for (var col = 0; col < Board.GetLength(1); col++)
                    Board[row, col] = 0; //sets the removed rows column to 0(no block)
            }

        for (var row = 0; row < gameBoardTemp.GetLength(0); row++)
        for (var col = 0; col < gameBoardTemp.GetLength(1); col++)
            Board[row, col] = gameBoardTemp[row, col]; // now copy temp to board for updated array
    }

    /// <summary>
    ///     Forces board to move down(NOT the same as Fall())
    /// </summary>
    public void MoveDown()
    {
        for (var i = Board.GetLength(0) - 1; i > 0; i--)
        for (var f = 0; f < Board.GetLength(1); f++)
        {
            if (i == 21)
                continue;

            Board[i + 1, f] = Board[i, f]; //copy below array
            Board[i, f] = 0;
        }
    }

    /// <summary>
    ///     Forces board to move up(used to add a new row to bottom of board(RandomRow))
    /// </summary>
    public void MoveUp()
    {
        for (var i = 0; i < Board.GetLength(0); i++)
        for (var f = 0; f < Board.GetLength(1); f++)
        {
            if (i == 21)
                continue;

            Board[i, f] = Board[i + 1, f];
            Board[i + 1, f] = 0;
        }
    }

    /// <summary>
    ///     Checks if the Y-position of the block would collide with placed blocks.
    /// </summary>
    public bool WouldCollide(Rectangle[] rects, bool noChange = false)
    {
        for (var i = 0; i < Board.GetLength(0); i++)
        for (var f = 0; f < Board.GetLength(1); f++)
        {
            //convert our array position to x,y positions to create rectangle.
            var rect = new Rectangle(f * 32, i * 32 + Globals.LowestY - (noChange ? 0 : 32), 32, 32);
            //now, check if the rectangle would intersect with the given rects
            for (var l = 0; l < rects.Length; l++)
                if (rect.Intersects(rects[l]) && Board[i, f] != 0)
                    return true;
        }

        return false;
    }

    /// <summary>
    ///     Checks if the X-position of the block would collide with the game board or another placed block.
    /// </summary>
    public bool WouldCollideLR(Rectangle[] rects, bool left)
    {
        for (var i = 0; i < Board.GetLength(0); i++)
        for (var f = 0; f < Board.GetLength(1); f++)
        {
            var rect = new Rectangle(f * 32 + (left ? 32 : -32), i * 32 + Globals.LowestY, 32, 32);

            for (var l = 0; l < rects.Length; l++)
                if (rect.Intersects(rects[l]) && Board[i, f] != 0)
                    return true;
        }

        //If we collide with side of board.
        for (var l = 0; l < rects.Length; l++)
        {
            if (rects[l].X >= 288 && !left)
                return true;
            if (rects[l].X <= 0 && left)
                return true;
        }

        return false;
    }

    /// <summary>
    ///     Removes all rows added to the rowsToRemove array.
    /// </summary>
    private void RemoveFullRows()
    {
        if (rowsToRemove.Count <= 0)
            return;
        ScoreHandler.Instance.Remove = true; // let the scorehandler know we are removing rows
        var grey = 0;

        foreach (var col in rowsToRemove)
        {
            var addedGrey = false;
            totalLines++;
            if (col * 32 + Globals.LowestY > yLev) // find the lowest y value.
                yLev = col * 32 + Globals.LowestY;

            laser.Add(new BlockAnimator(col * 32 + Globals.LowestY)); // add animated laser thing

            //check to see if row is full
            for (var i = 0; i < 10; i++)
            {
                if (Board[col, i] == 8 && !addedGrey)
                {
                    // This is used for keeping score and getting actual rows completed
                    grey++;
                    addedGrey = true;
                }

                Board[col, i] = 0;
            }
        }

        greyLines = grey;

        //freeze player above playing field.
        PlayerController.Instance.Frozen = true;
        Opacity = 1.0f;
        //wait 700ms before moving row down.
        TimerUtil.Instance.CreateTimer(700, EventManager.Instance.GetEvent("rowfall").Call, "falltime");
    }

    public void UpdateLasers(GameTime gameTime)
    {
        if (laser.Count <= 0)
            return;
        for (var i = 0; i < laser.Count; i++) laser[i].Update(gameTime);
    }

    public void DrawLasers(SpriteBatch spriteBatch)
    {
        foreach (var las in laser)
        {
            for (var f = 0; f < 10; f++)
                spriteBatch.Draw(las.CopyCatTex[f], las.CopyCat[f],
                    Color.White * (las.Opacity - 0.4f));
            spriteBatch.Draw(Globals.TexBox, las.Laser[0], las.LaserColor * las.Opacity);
            spriteBatch.Draw(Globals.TexBox, las.Laser[1], las.LaserColor * las.Opacity);
        }
    }

    public int GetActualLines()
    {
        return totalLines - greyLines;
    }

    public int GetTotalLines()
    {
        return totalLines;
    }

    private void FallDown()
    {
        Fall();

        NetworkManager.Instance.SendPacket(4); // Send Garbage Packet
        NetworkManager.Instance.SendPacket(2); // Send row packet

        for (var i = 0; i < 10; i++)
        for (var f = 0; f < GameManager.Instance.Random.Next(totalLines - 1, totalLines + 2); f++)
            ParticleManager.Instance.Create(new Ruble(i * 32, yLev + 16)); // idk, thought it looked kinda cool

        if (totalLines > 0)
            Sfx.Instance.PlaySoundEffect("rowfall");

        Gui.Instance.AddDebugMessage($"Actual Cleared:{GetActualLines()}, Lines Cleared:{GetTotalLines()}");
        PlayerController.Instance.Frozen = false; // unfreeze player y movement

        //reset variables.
        yLev = Globals.LowestY;
        rowsToRemove = new List<int>();
        laser = new List<BlockAnimator>();

        ScoreHandler.Instance.WasTSpin = false;
    }

    /// <summary>
    ///     This function is used to check if any placed tetris blocks reaches the top of the screen.
    ///     If it does, it will end the game.
    /// </summary>
    public void HitTop()
    {
        if (BoardShakeAnimator.Instance.Shaking || BoardShakeAnimator.Instance.Animating ||
            PlayerController.Instance.Frozen)
            return;
        for (var i = 0; i < 10; i++)
            if (Board[0, i] != 0)
            {
                Gui.Instance.AddDebugMessage($"Rectangle {i} reached top of screen.");
                NetworkManager.Instance.SendPacket(7);
                EventManager.Instance.GetEvent("playerdeath").Call();
                Sfx.Instance.PlaySoundEffect("crush");
                break;
            }
    }

    /// <summary>
    ///     Generates a row with one random hole. (Mainly used for multiplayer and higher levels)
    /// </summary>
    /// <param name="rows">Rows to add to board</param>
    public void RandomBlock(int rows)
    {
        var y = Globals.MaxY;
        Gui.Instance.AddDebugMessage($"Generating {rows} random row(s)");

        for (var f = 0; f < rows; f++)
            MoveUp();

        for (var f = 0; f < rows; f++)
        {
            var ply = PlayerController.Instance;
            if (ply.PlyY > -16) // lazy way of fixing collision issues with rows.
                ply.PlyY -= 32;

            var x = 0;
            var randX = GameManager.Instance.Random.Next(1, 11);

            for (var i = 10; i > 0; i--)
            {
                if (randX != i) AddBlockToBoard(8, x, y);
                x += 32;
            }

            y -= 32;
        }
    }
}
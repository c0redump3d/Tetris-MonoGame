using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.GameDebug;
using Tetris.Other;

namespace Tetris.Main
{
    /// <summary>
    /// This class is in charge of checking rows, meaning check if a row is full.
    /// If a row is full this class also is in charge of removing the corresponding
    /// row and update the form.
    /// </summary>
    public class RowCheck
    {

        private int[] bank = new int[0];
        private int rowRemove = Globals.MaxY;
        private bool removing = false;
        private readonly Rectangle[] row = new Rectangle[220];
        private int moveDown = 0;
        private readonly int[] xRow = new int[10] { 0, 32, 64, 96, 128, 160, 192, 224, 256, 288 };
        private readonly int[] yRow = new int[22] { -48, -16, 16, 48, 80, 112, 144, 176, 208, 240, 272, 304,
            336, 368, 400, 432, 464, 496, 528, 560, 592, 624}; // offset by 16 as gui shows half of row 21
        private int[,] board = new int[22, 10];

        public RowCheck()
        {
            int currentRect = 0;

            //creates a 10x22 grid
            for (int y = 0; y < 22; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    row[currentRect] = new Rectangle(x * 32, y * 32, 32, 32);
                    currentRect++;
                }
            }
        }

        /// <summary>
        /// When this is called, it will store all current placedRect indexes into an array here, where we then check to see if there any full rows.
        /// </summary>
        /// <param name="placedRect"></param>
        /// <param name="remove"></param>
        public void UpdateRowCheck(ref Rectangle[] placedRect)
        {
            if (removing)
                return;
            Reset();
            moveDown = 0;
            rowRemove = Globals.MaxY;
            try
            {
                for (int i = 0; i < placedRect.Length; i++)
                    for (int j = 0; j < row.Length; j++)
                        if (placedRect[i].Intersects(row[j])) // Basically means a block has landed on a specific square
                        {
                            board[GetLocationY(placedRect[i].Y), GetLocationX(placedRect[i].X)] = i; // store the index of the placedRect inside the board array
                            break;
                        }

                CheckRow(ref placedRect); // check to see if any removals are needed
            }
            catch (Exception ex) { Debug.DebugMessage($"ERROR: Updating row check failed with {ex.Message}", 1, true); }
        }

        /// <summary>
        /// Will check to see if any rows need to be removed, and if so deletes them from the placedRect array.
        /// </summary>
        /// <param name="placedRect"></param>
        /// <param name="storedColor"></param>
        /// <param name="remove"></param>
        private void CheckRow(ref Rectangle[] placedRect)
        {
            moveDown = CheckIfFull(ref placedRect);

            if (removing) // if there are rows to remove.
            {
                for (int i = moveDown * 10 - 1; i >= 0; i--)
                {
                    placedRect.RemoveAt(bank[i]);
                }

                Reset();
                Instance.GetScoreHandler().Remove = true; // let mainform know we are removing rows.

                Instance.GetPacket().SendPacketFromName("plc"); // send new placedRect positions to client/server
            }
        }

        public void Reset()
        {
            board = new int[22, 10];
            bank = new int[0];
            removing = false;
            rowRemove = Globals.MaxY;
        }

        /// <summary>
        /// Checks each row to see if it needs to be removed(full row)
        /// </summary>
        /// <param name="placedRect"></param>
        /// <returns>Count of rows that are to be removed</returns>
        private int CheckIfFull(ref Rectangle[] placedRect)
        {
            int lines = 0;

            for (int curRow = 0; curRow < board.GetLength(0); curRow++)
            {
                bool rowFull = true; // assume row is full by default.
                for (int curCol = 0; curCol < board.GetLength(1); curCol++)
                {
                    if (board[curRow, curCol] == 0) // if board position returns a 0, it means there are currently no blocks at that position
                    {
                        rowFull = false; // since row is not full, do not remove.
                        break;
                    }
                }

                if (rowFull)
                {
                    removing = true; // let CheckRow() know there are rows to be removed.
                    List<int> addBank = bank.ToList();
                    for (int i = 0; i < board.GetLength(1); i++)
                    {
                        addBank.Add(board[curRow, i]); // add blocks that need to be removed
                        rowRemove = placedRect[board[curRow, i]].Y; // gives us the lowest Y of the row removed.
                    }
                    bank = addBank.ToArray();
                    Array.Sort(bank); // sort by positions.

                    for (int g = placedRect.Length - 1; g > 0; g--)
                        if (placedRect[g].Y < rowRemove) // if the given placedRect is above the removed row, bring it down.
                            placedRect[g].Y += 32;

                    lines++;
                }
            }

            return lines;
        }

        /// <summary>
        /// Gives the index of the given X position.
        /// </summary>
        /// <param name="x">placedRect.X</param>
        /// <returns>Index of the given X position.</returns>
        private int GetLocationX(int x)
        {
            for (int i = 0; i < xRow.Length; i++)
                if (x == xRow[i])
                {
                    return i;
                }

            return 0;
        }

        /// <summary>
        /// Gives the index of the given Y position.
        /// </summary>
        /// <param name="y">placedRect.Y</param>
        /// <returns>Index of the given Y position.</returns>
        private int GetLocationY(int y)
        {
            for (int i = 0; i < yRow.Length; i++)
                if (y == yRow[i])
                {
                    return i;
                }

            return 0;
        }

        internal int GetLinesCleared()
        {
            return moveDown;
        }
    }
}

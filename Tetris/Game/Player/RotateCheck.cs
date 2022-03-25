using Microsoft.Xna.Framework;
using Tetris.Game.InGame;
using Tetris.GUI;
using Tetris.Sound;

namespace Tetris.Game.Player;

/// <summary>
///     Adds Tetris-like SRS system to game.
///     NOTE: Although very accurate to SRS, it is not 100% accurate and does have some issues.
///     Specifically I-block rotations are something that are not 100% SRS accurate.
/// </summary>
public class RotateCheck
{
    private static RotateCheck _instance;
    private readonly Rectangle[] checkBlock = new Rectangle[4];

    /// <summary>
    ///     Gives the positions to check for each rotate if the block is unable to stay in its current position
    ///     (see https://tetris.fandom.com/wiki/SRS for info) (Note: y on the chart equals -y here)
    /// </summary>
    private readonly int[,,] checkPos =
    {
        {{-1, 0}, {-1, -1}, {0, 2}, {-1, 2}}, // 0 to 1 or 2 to 1
        {{1, 0}, {1, 1}, {0, -2}, {1, -2}}, // 1 to 0 or 1 to 2
        {{1, 0}, {1, -1}, {0, 2}, {1, 2}}, // 2 to 3 or 0 to 3
        {{-1, 0}, {-1, 1}, {0, -2}, {-1, -2}} // 3 to 2 or 3 to 0
    };

    private readonly int[,,] checkPosI =
    {
        {{-2, 0}, {1, 0}, {-2, 1}, {1, -2}}, // 0 to 1 or 3 to 2
        {{-1, 0}, {2, 0}, {-1, -2}, {2, 1}}, // 1 to 0 or 2 to 3
        {{2, 0}, {-1, 0}, {2, -1}, {-1, 2}}, // 1 to 2 or 0 to 3
        {{1, 0}, {-2, 0}, {1, 2}, {-2, -1}} // 2 to 1 or 3 to 0
    };

    public readonly int[] RotPos = {0, 0, 0, 0, 0, 0}; // [0] = R1, [1] = R2, [2] = L1, etc.
    private int checkX = 160;
    private int checkY = 16;
    private PlayerController ply;

    public static RotateCheck Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new RotateCheck();

            return result;
        }
    }

    private int FromTo(int to, bool clockwise, bool iBlock = false)
    {
        var from = to + (clockwise ? -1 : 1);
        if (from > 3)
            from = 0;
        else if (from < 0)
            from = 3;

        var fromTo = $"{from},{to}";

        Gui.Instance.AddDebugMessage($"WallKick Data: {fromTo}");

        //Find the correct index in the rotation check array to find positions to be checked.
        if (!iBlock)
            switch (fromTo)
            {
                case "0,1":
                case "2,1":
                    return 0;
                case "1,0":
                case "1,2":
                    return 1;
                case "2,3":
                case "0,3":
                    return 2;
                case "3,2":
                case "3,0":
                    return 3;
            }
        else
            switch (fromTo)
            {
                case "0,1":
                case "3,2":
                    return 0;
                case "1,0":
                case "2,3":
                    return 1;
                case "1,2":
                case "0,3":
                    return 2;
                case "2,1":
                case "3,0":
                    return 3;
            }

        return 0;
    }

    /// <summary>
    ///     Checks multiple possible positions for a stuck block to move to(This allows for triple t-spins).
    ///     See https://tetris.fandom.com/wiki/SRS for the table that was used.
    /// </summary>
    /// <param name="clockwise"></param>
    private void WallKick(bool clockwise)
    {
        var iBlock = Rotate.Instance.GetCurShape() == 4;

        var check = iBlock ? checkPosI : checkPos; // because I block is weird, it has its own rotation checks

        UpdateXY();
        UpdateRectangles();

        //ICheck(checkPosAt, clockwise, false);
        var found = false;
        var checkPosAt = Rotate.Instance.GetCurAngle();
        ICheck(checkPosAt, clockwise, false);
        if (IsOutOfBounds())
        {
            UpdateXY();
            for (var g = 0; g < 5; g++)
            {
                for (var i = 0; i < 4; i++)
                {
                    checkX += check[FromTo(checkPosAt, clockwise, iBlock), i, 0] * 32;
                    checkY += check[FromTo(checkPosAt, clockwise, iBlock), i, 1] * 32;

                    ICheck(checkPosAt, clockwise, false);
                    UpdateRectangles();
                    if (!IsOutOfBounds())
                    {
                        Gui.Instance.AddDebugMessage($"passed:{FromTo(checkPosAt, clockwise, iBlock)},{i}");
                        SetPlayerPosition(checkPosAt);
                        found = true;
                        break;
                    }

                    UpdateXY();
                    UpdateRectangles();
                    Gui.Instance.AddDebugMessage($"failed:{i}");
                }

                if (found)
                    break;

                checkPosAt += clockwise ? 1 : -1;
                if (checkPosAt > 3)
                    checkPosAt = 0;
                else if (checkPosAt < 0)
                    checkPosAt = 3;

                UpdateXY();
                ICheck(checkPosAt, clockwise, false);

                for (var l = 0; l < 6; l++)
                    RotPos[l] = Rotate.Instance
                        .Blocks[Rotate.Instance.GetCurShape() - 1, checkPosAt, l] * 32;
                //checkAng += sub ? 1 : -1;

                UpdateXY();
                UpdateRectangles();
            }

            if (!found)
                Rotate.Instance.RotatePiece(!clockwise);
            else
                Sfx.Instance.PlaySoundEffect("rotate");
        }
        else
        {
            Sfx.Instance.PlaySoundEffect("rotate");
            SetPlayerPosition(Rotate.Instance.GetCurAngle());
        }
    }

    /// <summary>
    ///     Very shady way of adding accurate I block rotations.
    ///     I assume this is the reason I-block rotation checks are not 100% accurate.
    /// </summary>
    private void ICheck(int checkAng, bool clockwise, bool reverse)
    {
        //Best way I've found to implement semi-proper i-block rotating as implementing it properly would require LOTS of rewriting.

        UpdateRectangles();
        if (reverse)
            clockwise = !clockwise;
        if (Rotate.Instance.GetCurShape() == 4)
        {
            if (clockwise)
                switch (checkAng)
                {
                    case 0:
                        checkY -= 32;
                        break;
                    case 1:
                        checkX += 32;
                        break;
                    case 2:
                        checkY += 32;
                        break;
                    case 3:
                        checkX -= 32;
                        break;
                }
            else
                switch (checkAng)
                {
                    case 3:
                        checkY += 32;
                        break;
                    case 2:
                        checkX += 32;
                        break;
                    case 1:
                        checkY -= 32;
                        break;
                    case 0:
                        checkX -= 32;
                        break;
                }
        }
    }

    public void UpdateCheck(bool clockwise)
    {
        UpdateXY();

        UpdateRectangles();

        WallKick(clockwise);
    }

    private void UpdateRectangles()
    {
        checkBlock[0] = new Rectangle(checkX, checkY, 32, 32); // player controlled rect
        checkBlock[1] = new Rectangle(checkX + RotPos[0], checkY + RotPos[1], 32, 32);
        checkBlock[2] = new Rectangle(checkX - RotPos[2], checkY - RotPos[3], 32, 32);
        checkBlock[3] = new Rectangle(checkX + RotPos[5], checkY - RotPos[4], 32, 32);
    }

    public bool TSpinLock()
    {
        //A T-Spin Lock is determined if the tetrimino is unable to move left,right,up,or down
        for (var i = 0; i < 4; i++)
        {
            SetAllPositions();
            UpdateRectangles();
            switch (i)
            {
                case 0:
                    checkX += 32;
                    if (!IsOutOfBounds())
                    {
                        Gui.Instance.AddDebugMessage($"T-Spin failed at check {i} (X+32)");
                        return false;
                    }

                    break;
                case 1:
                    checkX -= 32;
                    if (!IsOutOfBounds())
                    {
                        Gui.Instance.AddDebugMessage($"T-Spin failed at check {i} (X-32)");
                        return false;
                    }

                    break;
                case 2:
                    checkY += 32;
                    if (!IsOutOfBounds())
                    {
                        Gui.Instance.AddDebugMessage($"T-Spin failed at check {i} (Y+32)");
                        return false;
                    }

                    break;
                case 3:
                    checkY -= 32;
                    if (!IsOutOfBounds())
                    {
                        Gui.Instance.AddDebugMessage($"T-Spin failed at check {i} (Y-32)");
                        return false;
                    }

                    break;
            }
        }

        Gui.Instance.AddDebugMessage("T-Spin lock detected!");
        return true;
    }

    private void UpdateXY()
    {
        ply ??= PlayerController.Instance;
        //set our x,y to the players x,y
        checkX = ply.PlyX;
        checkY = ply.PlyY;
    }

    /// <summary>
    ///     Checks to see if the rotated piece is considered out of bounds(inside a block, or off-screen)
    /// </summary>
    /// <returns>true if out of bounds</returns>
    private bool IsOutOfBounds()
    {
        UpdateRectangles();
        Rectangle[] boxes = {checkBlock[0], checkBlock[1], checkBlock[2], checkBlock[3]};

        for (var i = 0; i < boxes.Length; i++)
        {
            UpdateRectangles();
            if (boxes[i].X < 0 || boxes[i].X > 288) // if we are out of screen horizontally
            {
                Gui.Instance.AddDebugMessage($"Failed at x check, {i},{boxes[i].X}");
                return true;
            }

            if (boxes[i].Y > Globals.MaxY) // if we are off screen vertically
            {
                Gui.Instance.AddDebugMessage("Failed at y check");
                return true;
            }


            if (TetrisBoard.Instance.WouldCollide(boxes, true))
            {
                Gui.Instance.AddDebugMessage("Failed at box check");
                return true;
            }
        }

        return false;
    }

    public Rectangle[] GetRotationBlocks()
    {
        return new[] {checkBlock[0], checkBlock[1], checkBlock[2], checkBlock[3]};
    }

    public void SetAllPositions()
    {
        if (ply == null)
        {
            ply = PlayerController.Instance;
            return;
        }

        //set our positions
        for (var i = 0; i < 4; i++)
            checkBlock[i] = ply.PlayerBlocks[i];
        checkX = ply.PlyX;
        checkY = ply.PlyY;
        for (var i = 0; i < 6; i++)
            RotPos[i] = ply.PlayerPos[i];
    }

    private void SetPlayerPosition(int ang)
    {
        //set the players position to ours
        Rotate.Instance.SetCurAngle(ang);
        for (var i = 0; i < 4; i++)
            ply.PlayerBlocks[i] = checkBlock[i];
        ply.PlyX = checkX;
        ply.PlyY = checkY;
        for (var i = 0; i < 6; i++)
            ply.PlayerPos[i] = Rotate.Instance.Blocks[Rotate.Instance.GetCurShape() - 1, ang, i] * 32;
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Tetris.Other;
using Tetris.Settings;

namespace Tetris.Main
{
    public class KeyboardInput
    {
        private KeyboardState keyState;
        private KeyboardState oldKeyState;
        private bool movingLeftRight = false;
        private bool rotating = false;
        private bool fastFall = false;
        private readonly double[] elapsedTime = new double[3];
        public void HandleKeyPress(GameTime gameTime)
        {
            keyState = Keyboard.GetState();
            if (Instance.GetGuiSettings().CurrentlyFocused)
            {
                if (keyState.GetPressedKeyCount() == 0)
                    return;
                
                for (int i = 0; i < keyState.GetPressedKeyCount(); i++)
                {
                    if (!keyState.IsKeyDown(keyState.GetPressedKeys()[i]))
                        continue;
                    Instance.GetGuiSettings().KeyPressed(keyState.GetPressedKeys()[i]);
                }
            }

            if (!Instance.GetGame().Stopped && Instance.GetGame().CurrentScreen == 1 && Instance.GetGame().CanMove)
            {
                if (IsKeyPress(Keys.Escape))
                {
                    if (!Instance.GetGame().Paused)
                    {
                        Instance.GetGame().Paused = !Instance.GetGame().Paused;
                        Instance.GetSound().PlayPauseMusic(Instance.GetScoreHandler().Level);
                        Instance.GetSound().PlaySoundEffect("pause");
                    }
                    else
                    {
                        if(!Instance.GetGame().GetCountdown())
                            Instance.GetGame().StartCountdown();
                    }

                    Instance.GetRichPresence().UpdatePresence();
                    
                }
            }

            if (IsKeyPress(Keys.F4))
            {
                Instance.GetGuiDebug().ShowMenu();
            }

            if (Instance.GetGame().Stopped || Instance.GetGame().Paused || !Instance.GetGame().CanMove)
            {
                oldKeyState = keyState;
                return;
            }

            for(int i = 0; i < elapsedTime.Length; i++)
                elapsedTime[i] -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (IsKeyPress(MovementKeys.FORCEDROP))
            {
                if (fastFall || rotating || movingLeftRight)
                    return;
                if (Instance.GetPlayer().InstantFall())
                {
                    fastFall = true;
                }
                return;
            }
            else
            {
                fastFall = false;
            }

            if (IsKeyPress(MovementKeys.HOLD))
            {
                Instance.GetHoldShape().SetHoldShape(Instance.GetRotate().GetCurShape());
            }
            
            if (keyState.IsKeyDown(MovementKeys.LEFT) && !oldKeyState.IsKeyDown(MovementKeys.RIGHT))
            {
                if (elapsedTime[0] <= 0)
                {
                    movingLeftRight = true;
                    Instance.GetMovement().MoveLeft();
                    elapsedTime[0] = 150;
                }
            }
            else
            {
                movingLeftRight = false;
                elapsedTime[0] = 0;
            }
            if (keyState.IsKeyDown(MovementKeys.RIGHT) && !oldKeyState.IsKeyDown(MovementKeys.LEFT))
            {
                if (elapsedTime[1] <= 0)
                {
                    movingLeftRight = true;
                    Instance.GetMovement().MoveRight();
                    elapsedTime[1] = 150;
                }
            }
            else
            {
                movingLeftRight = false;
                elapsedTime[1] = 0;
            }

            if (keyState.IsKeyDown(MovementKeys.DOWN))
            {
                if (elapsedTime[2] <= 0)
                {
                    Instance.GetMovement().MoveDown();
                    elapsedTime[2] = 50;
                }
            }
            else
            {
                elapsedTime[2] = 0;
            }

            if (IsKeyPress(Keys.F1) && Instance.GetGuiDebug().IsOptionEnabled(2))
            {
                int shape = Instance.GetRotate().GetCurShape();
                if (shape < 8)
                    shape++;
                if (shape == 8)
                    shape = 1;

                Instance.GetRotate().SetCurShape(shape);
                Instance.GetRotate().ResetRot();
                Instance.GetPlayer().SetShape();
            }

            if (IsKeyPress(MovementKeys.ROTATERIGHT) || IsKeyPress(MovementKeys.ROTATELEFT))
            {
                rotating = true;
                Instance.GetRotate().RotateTetris(!keyState.IsKeyDown(MovementKeys.ROTATELEFT));
                Instance.GetSound().PlaySoundEffect("rotate");
            }
            else
            {
                rotating = false;
            }

            oldKeyState = keyState;
        }

        private bool IsKeyPress(Keys key)
        {
            if (keyState.IsKeyDown(key) && !oldKeyState.IsKeyDown(key))
                return true;

            return false;
        }
    }
}
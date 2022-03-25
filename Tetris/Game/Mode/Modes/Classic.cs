using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Game.Events;
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.Game.Player;
using Tetris.GUI;
using Tetris.Multiplayer.Network;
using Tetris.Settings;

namespace Tetris.Game.Mode.Modes;

public class Classic : CustomMode
{
    private float gravityTime = 0;
    private float gravityInterval = 48;
    private int level;
    private PlayerController ply;
    
    public Classic(string name, string objective) : base(name, objective)
    {
    }
    
    public override void SetUp()
    {
        ply = PlayerController.Instance;
        level = 0;
        gravityInterval = 48;
        UpdateInterval();
        base.SetUp();
        //Disable gravity as we will be implementing our own system.
        EventManager.Instance.GetEvent("gravity").SetCancelled(true);
        AddPreEvent("gravity", "classicgrav", ClassicGrav);
        //No holding in classic tetris.
        EventManager.Instance.GetEvent("hold").SetCancelled(true);
        //Also, no force drop in classic tetris.
        EventManager.Instance.GetEvent("forcedrop").SetCancelled(true);
        //Add a post event to update the gravities interval when player levels up.
        AddPostEvent("levelup", "gravlevel", UpdateInterval);
        
        AddPreEvent("endgame", "classicreenable", () =>
        {
            EventManager.Instance.GetEvent("gravity").SetCancelled(false);
            EventManager.Instance.GetEvent("hold").SetCancelled(false);
            EventManager.Instance.GetEvent("forcedrop").SetCancelled(false);
        });
    }

    private void UpdateInterval()
    {
        for(int i = level; i < ScoreHandler.Instance.Level-1; i++){
            switch (i)
            {
                case < 8:
                    gravityInterval -= 5f;
                    break;
                case 8:
                    gravityInterval -= 2f;
                    break;
                case 9:
                case 12:
                case 15:
                case 18:
                case > 28:
                    if (gravityInterval > 1f)
                        gravityInterval--;
                    break;
            }
            level++;
        }
        
        Gui.Instance.AddDebugMessage($"Updated classic gravity interval to: {gravityInterval*22}ms, {level}");
    }
    
    private void ClassicGrav()
    {
        // if player is above an already placed shape, we return.
        if (ply.IsColliding() || ply.Frozen || InGameManager.Instance.Paused)
            return;

        gravityTime -= Globals.GameTime.ElapsedGameTime.Milliseconds;

        if (gravityTime <= 0)
        {
            // make sure we aren't at the bottom of the board.
            if (!ply.IsColliding() && !Keyboard.GetState().IsKeyDown((Keys) GameSettings.Instance.GetOptionValue("Down")))
                ply.PlyY += 32;
            NetworkManager.Instance.SendPacket(3);
            //needs to be multiplied by 22, as NES tetris ran at 60FPS, and gravity was updated every 22 frames.
            gravityTime = (gravityInterval * 22);
        }
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}
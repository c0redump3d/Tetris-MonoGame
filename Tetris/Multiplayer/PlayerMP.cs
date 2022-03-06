using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.Game.Player;

namespace Tetris.Multiplayer
{
    
    /// <summary>
    /// This class is essentially a very basic version of PlayerController.cs
    /// Better to control these objects/values locally instead of having to send more data over network.
    /// </summary>
    public class PlayerMP
    {
        //TODO: Needs to be updated to support the new Color manager.
        
        public int PosX, PosY;
        private int rotationAngle;
        private int currentShape;
        private readonly int[] blockPositions;
        public int[,] MultiplayerBoard;
        private Viewport oldViewport;
        private readonly Viewport mpViewport;

        private readonly Rectangle[] rotationBlock;

        private PlayerMP()
        {
            mpViewport = new Viewport(857, 32, 320, 656);
            rotationBlock = new Rectangle[4];
            PosX = 0;
            PosY = 0;
            rotationAngle = 0;
            currentShape = 1;
            blockPositions = new int[6];
            MultiplayerBoard = new int[22, 10];
        }

        private void SetRotation(int rotation)
        {
            rotationAngle = rotation;

            for (int i = 0; i < 6; i++)
                blockPositions[i] = Rotate.Instance.Blocks[currentShape - 1, rotationAngle, i] * 32;
        }

        public void SetShape(int shape, int rotation)
        {
            currentShape = shape;
            SetRotation(rotation);
            for (int i = 0; i < 6; i++)
                blockPositions[i] = Rotate.Instance.Blocks[currentShape - 1, rotationAngle, i] * 32;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            oldViewport = spriteBatch.GraphicsDevice.Viewport;
            spriteBatch.GraphicsDevice.Viewport = mpViewport;
            spriteBatch.Begin();
            for(int i =0;i<4;i++)
                spriteBatch.Draw(Globals.BlockTexture[currentShape-1], rotationBlock[i], Color.White);
            
            for (var i = 0; i < 22; i++)
            for (var f = 0; f < 10; f++)
            {
                if (MultiplayerBoard[i, f] == 0)
                    continue;
                spriteBatch.Draw(Globals.BlockTexture[MultiplayerBoard[i, f] - 1],
                    new Rectangle(f * 32, i * 32 + Globals.LowestY, 32, 32), Color.White);
            }
            spriteBatch.End();
            spriteBatch.GraphicsDevice.Viewport = oldViewport;
        }

        public void Update()
        {
            rotationBlock[0] = new Rectangle(PosX, PosY, 32, 32); // player controlled rect
            rotationBlock[1] = new Rectangle(PosX + blockPositions[0], PosY + blockPositions[1], 32, 32);
            rotationBlock[2] = new Rectangle(PosX - blockPositions[2], PosY - blockPositions[3], 32, 32);
            rotationBlock[3] = new Rectangle(PosX + blockPositions[5], PosY - blockPositions[4], 32, 32);
        }
        
        
        private static PlayerMP _instance;
        public static PlayerMP Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new PlayerMP();
                }

                return result;
            }
        }
    }
}
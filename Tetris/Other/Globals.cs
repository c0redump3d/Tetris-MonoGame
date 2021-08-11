using System.Net.Mime;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris.Other
{
    public class Globals
    {
        public static readonly int BlockSize = 32; // the size of the blocks
        public static readonly int MaxY = 624; // the highest possible Y on screen
        public static readonly int LowestY = -48; // the lowest possible Y for the player to be at
        public static readonly int TopOut = -80; // if a block reaches this y position, the game will end
        public static Texture2D[] GuiImage = new Texture2D[2];
        public static Texture2D GuiGrid;
        public static Texture2D CurrentGuiImage;
        public static Texture2D Stats;
        public static Texture2D TextBoxGui;
        public static int ScreenWidth = 789;
        public static int ScreenHeight = 694;
        public static string Version = "v1.1";
        public static bool ResizedWindow = false;
        public static string CurrentOS;
        public static readonly Texture2D[] BlockTexture = new Texture2D[8];
        public static readonly Texture2D[] BlockPlacedTexture = new Texture2D[8];
        public static readonly Texture2D[] scoreTextures = new Texture2D[6];
        public static readonly Texture2D[] levelUpTextures = new Texture2D[3];
        public static readonly Texture2D[] countTextures = new Texture2D[4];
        public static Texture2D Logo;
        public static Texture2D pinchOverlay;
        public static SpriteFont ConsoleFont;
        public static SpriteFont hoog_12;
        public static SpriteFont hoog_16;
        public static SpriteFont hoog_18;
        public static SpriteFont hoog_24;
        public static SpriteFont hoog_28;
        public static SpriteFont hoog_38;

        public static void SetUp(ContentManager content)
        {
            BlockTexture[0] = content.Load<Texture2D>("blocks/tetris_t_full");
            BlockPlacedTexture[0] = content.Load<Texture2D>("blocks/tetris_t_placed");
            BlockTexture[1] = content.Load<Texture2D>("blocks/tetris_z1_full");
            BlockPlacedTexture[1] = content.Load<Texture2D>("blocks/tetris_z1_placed");
            BlockTexture[2] = content.Load<Texture2D>("blocks/tetris_l1_full");
            BlockPlacedTexture[2] = content.Load<Texture2D>("blocks/tetris_l1_placed");
            BlockTexture[3] = content.Load<Texture2D>("blocks/tetris_i_full");
            BlockPlacedTexture[3] = content.Load<Texture2D>("blocks/tetris_i_placed");
            BlockTexture[4] = content.Load<Texture2D>("blocks/tetris_o_full");
            BlockPlacedTexture[4] = content.Load<Texture2D>("blocks/tetris_o_placed");
            BlockTexture[5] = content.Load<Texture2D>("blocks/tetris_l2_full");
            BlockPlacedTexture[5] = content.Load<Texture2D>("blocks/tetris_l2_placed");
            BlockTexture[6] = content.Load<Texture2D>("blocks/tetris_z2_full");
            BlockPlacedTexture[6] = content.Load<Texture2D>("blocks/tetris_z2_placed");
            BlockTexture[7] = content.Load<Texture2D>("blocks/tetris_x_full");
            BlockPlacedTexture[7] = content.Load<Texture2D>("blocks/tetris_x_placed");
            scoreTextures[0] = content.Load<Texture2D>("gui/doubleline");
            scoreTextures[1] = content.Load<Texture2D>("gui/tripleline");
            scoreTextures[2] = content.Load<Texture2D>("gui/tetris");
            scoreTextures[3] = content.Load<Texture2D>("gui/tspinsingle");
            scoreTextures[4] = content.Load<Texture2D>("gui/tspindouble");
            scoreTextures[5] = content.Load<Texture2D>("gui/tspintriple");
            countTextures[0] = content.Load<Texture2D>("gui/countdown3");
            countTextures[1] = content.Load<Texture2D>("gui/countdown2");
            countTextures[2] = content.Load<Texture2D>("gui/countdown1");
            countTextures[3] = content.Load<Texture2D>("gui/start");
            levelUpTextures[0] = content.Load<Texture2D>("gui/levelup");
            levelUpTextures[1] = content.Load<Texture2D>("gui/levelup5");
            levelUpTextures[2] = content.Load<Texture2D>("gui/levelup8");
            Logo = content.Load<Texture2D>("gui/tetrislogo");
            GuiGrid = content.Load<Texture2D>("gui/grid");
            GuiImage[0] = content.Load<Texture2D>("gui/gui");
            GuiImage[1] = content.Load<Texture2D>("gui/guimultiplayer");
            CurrentGuiImage = GuiImage[0];
            Stats = content.Load<Texture2D>("gui/stats");
            TextBoxGui = content.Load<Texture2D>("gui/textbox");
            pinchOverlay = content.Load<Texture2D>("gui/pinchglow");
            ConsoleFont = content.Load<SpriteFont>("gui/font/consolas_10");
            hoog_12 = content.Load<SpriteFont>("gui/font/hoog_12");
            hoog_16 = content.Load<SpriteFont>("gui/font/hoog_16");
            hoog_18 = content.Load<SpriteFont>("gui/font/hoog_18");
            hoog_24 = content.Load<SpriteFont>("gui/font/hoog_24");
            hoog_28 = content.Load<SpriteFont>("gui/font/hoog_28");
            hoog_38 = content.Load<SpriteFont>("gui/font/hoog_38");
            CurrentOS = GetOS();
        }

        private static string GetOS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "win";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "osx";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "lin";

            return "unknown";
        }
    }
}
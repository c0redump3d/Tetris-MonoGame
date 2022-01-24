using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Util;

namespace Tetris.Game
{
    public class Globals
    {
        
        /*
         *
         *  Static global properties for the game.
         * 
         */
        public static readonly int BlockSize = 32; // the size of the blocks
        public static readonly int MaxY = 624; // the highest possible Y on screen
        public static readonly int LowestY = -48; // the lowest possible Y for the player to be at
        public static readonly int TopOut = -80; // if a block reaches this y position, the game will end
        public static Texture2D[] GuiImage = new Texture2D[4];
        public static Texture2D[] GuiSPLayers = new Texture2D[6];
        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;
        public static string Version = "v2.0";
        public static string CurrentOS;
        public static Texture2D CurrentLevelUpImage;
        public static Texture2D TexBox;
        public static Texture2D GradientBackground;
        public static readonly Texture2D[] BlockTexture = new Texture2D[8];
        public static readonly Texture2D[] StarTexture = new Texture2D[8];
        public static readonly Texture2D[] RubleTexture = new Texture2D[3];
        public static readonly Texture2D[] ScoreTextures = new Texture2D[6];
        public static readonly Texture2D[] LevelUpTextures = new Texture2D[3];
        public static readonly Texture2D[] CountTextures = new Texture2D[4];
        public static Texture2D Logo;
        public static Texture2D PinchOverlay;
        public static Texture2D BallTexture;
        public static SpriteFont ConsoleFont;
        public static SpriteFont Hoog12;
        public static SpriteFont Hoog16;
        public static SpriteFont Hoog18;
        public static SpriteFont Hoog24;
        public static SpriteFont Hoog28;
        public static SpriteFont Hoog36;
        public static SpriteFont Hoog48;

        public void SetUp(ContentManager content)
        {
            for (int i = 0; i < GuiSPLayers.Length; i++)
                GuiSPLayers[i] = content.Load<Texture2D>($"gui/ingame/layer{i+1}");
            CurrentLevelUpImage = LevelUpTextures[0];
            GradientBackground = new Texture2D(TetrisGame.Instance.Graphics.GraphicsDevice, 1280, 720);
            GradientBackground.SetData(Utils.CreateGradient(new Color(65,0,190), new Color(0,90,90), 1280, 720));
            TexBox = new Texture2D(TetrisGame.Instance.Graphics.GraphicsDevice, 1, 1); 
            TexBox.SetData(new[] {Color.White});
            BlockTexture[0] = content.Load<Texture2D>("blocks/tetris_t_full");
            BlockTexture[1] = content.Load<Texture2D>("blocks/tetris_z1_full");
            BlockTexture[2] = content.Load<Texture2D>("blocks/tetris_l1_full");
            BlockTexture[3] = content.Load<Texture2D>("blocks/tetris_i_full");
            BlockTexture[4] = content.Load<Texture2D>("blocks/tetris_o_full");
            BlockTexture[5] = content.Load<Texture2D>("blocks/tetris_l2_full");
            BlockTexture[6] = content.Load<Texture2D>("blocks/tetris_z2_full");
            BlockTexture[7] = content.Load<Texture2D>("blocks/tetris_x_full");
            BallTexture = content.Load<Texture2D>("FX/Background/particle");
            StarTexture[0] = content.Load<Texture2D>("FX/Sparkle/sparkle00");
            StarTexture[1] = content.Load<Texture2D>("FX/Sparkle/sparkle01");
            StarTexture[2] = content.Load<Texture2D>("FX/Sparkle/sparkle02");
            StarTexture[3] = content.Load<Texture2D>("FX/Sparkle/sparkle03");
            StarTexture[4] = content.Load<Texture2D>("FX/Sparkle/sparkle04");
            StarTexture[5] = content.Load<Texture2D>("FX/Sparkle/sparkle05");
            StarTexture[6] = content.Load<Texture2D>("FX/Sparkle/sparkle06");
            StarTexture[7] = content.Load<Texture2D>("FX/Sparkle/sparkle07");
            RubleTexture[0] = content.Load<Texture2D>("FX/Ruble/ruble00");
            RubleTexture[1] = content.Load<Texture2D>("FX/Ruble/ruble01");
            RubleTexture[2] = content.Load<Texture2D>("FX/Ruble/ruble02");
            ScoreTextures[0] = content.Load<Texture2D>("gui/doubleline");
            ScoreTextures[1] = content.Load<Texture2D>("gui/tripleline");
            ScoreTextures[2] = content.Load<Texture2D>("gui/tetris");
            ScoreTextures[3] = content.Load<Texture2D>("gui/tspinsingle");
            ScoreTextures[4] = content.Load<Texture2D>("gui/tspindouble");
            ScoreTextures[5] = content.Load<Texture2D>("gui/tspintriple");
            CountTextures[0] = content.Load<Texture2D>("gui/countdown3");
            CountTextures[1] = content.Load<Texture2D>("gui/countdown2");
            CountTextures[2] = content.Load<Texture2D>("gui/countdown1");
            CountTextures[3] = content.Load<Texture2D>("gui/start");
            LevelUpTextures[0] = content.Load<Texture2D>("gui/levelup");
            LevelUpTextures[1] = content.Load<Texture2D>("gui/levelup5");
            LevelUpTextures[2] = content.Load<Texture2D>("gui/levelup8");
            Logo = content.Load<Texture2D>("gui/tetrislogo");
            GuiImage[0] = content.Load<Texture2D>("gui/background");
            GuiImage[1] = content.Load<Texture2D>("gui/ingamegui");
            GuiImage[2] = content.Load<Texture2D>("gui/ingameguimultiplayer");
            GuiImage[3] = content.Load<Texture2D>("gui/stars");
            PinchOverlay = content.Load<Texture2D>("gui/pinchglow");
            ConsoleFont = content.Load<SpriteFont>("gui/font/consolas_10");
            Hoog12 = content.Load<SpriteFont>("gui/font/hoog_12");
            Hoog16 = content.Load<SpriteFont>("gui/font/hoog_16");
            Hoog18 = content.Load<SpriteFont>("gui/font/hoog_18");
            Hoog24 = content.Load<SpriteFont>("gui/font/hoog_24");
            Hoog28 = content.Load<SpriteFont>("gui/font/hoog_28");
            Hoog36 = content.Load<SpriteFont>("gui/font/hoog_38");
            Hoog48 = content.Load<SpriteFont>("gui/font/hoog_48");
            CurrentOS = GetOS();
        }

        /// <summary>
        /// Returns the current operating system the user is running.
        /// </summary>
        private string GetOS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "win";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "osx";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "lin";

            return "unknown";
        }

        private static Globals _instance;
        public static Globals Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new Globals();
                }

                return result;
            }
        }
    }
}
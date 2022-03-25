using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris.GUI;

public static class Fonts
{
    public static SpriteFont ConsoleFont;
    public static SpriteFont Hoog12;
    public static SpriteFont Hoog16;
    public static SpriteFont Hoog18;
    public static SpriteFont Hoog24;
    public static SpriteFont Hoog28;
    public static SpriteFont Hoog36;
    public static SpriteFont Hoog48;

    public static void LoadFonts(ContentManager content)
    {
        ConsoleFont = content.Load<SpriteFont>("gui/font/consolas_10");
        Hoog12 = content.Load<SpriteFont>("gui/font/hoog_12");
        Hoog16 = content.Load<SpriteFont>("gui/font/hoog_16");
        Hoog18 = content.Load<SpriteFont>("gui/font/hoog_18");
        Hoog24 = content.Load<SpriteFont>("gui/font/hoog_24");
        Hoog28 = content.Load<SpriteFont>("gui/font/hoog_28");
        Hoog36 = content.Load<SpriteFont>("gui/font/hoog_38");
        Hoog48 = content.Load<SpriteFont>("gui/font/hoog_48");
    }
}
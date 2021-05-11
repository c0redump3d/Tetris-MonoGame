using System;
using Tetris.Main;

namespace Tetris
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new TetrisGame())
                game.Run();
        }
    }
}

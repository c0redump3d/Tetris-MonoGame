using System;
using Tetris.Game.Managers;

namespace Tetris
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            GameManager.Instance.RunGame();
        }
    }
}
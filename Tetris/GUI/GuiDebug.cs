using System.Collections.Generic;

namespace Tetris.GUI
{
    public class GuiDebug
    {
        //TODO
        private List<string> debugMessages = new(6);
        private int count;
        public bool Enabled { get; set; }

        public void PrintMessage(string message)
        {
            if (count == 6)
            {
                MoveUp();
                count = 0;
            }

            debugMessages[count] = message;
            count++;
        }

        private void MoveUp()
        {
            debugMessages[0] = debugMessages[1];
            debugMessages[1] = debugMessages[2];
            debugMessages[3] = debugMessages[4];
            debugMessages[4] = debugMessages[5];
        }
    }
}
namespace Tetris.GUI
{
    public class DebugLine
    {

        public string Message;
        public int UpdateCounter;
        
        public DebugLine(string s)
        {
            Message = s;
            UpdateCounter = 0;
        }
        
    }
}
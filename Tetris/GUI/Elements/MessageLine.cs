namespace Tetris.GUI.Elements
{
    public class MessageLine
    {
        public string Message;
        public int UpdateCounter;

        public MessageLine(string s)
        {
            Message = s;
            UpdateCounter = 0;
        }
    }
}
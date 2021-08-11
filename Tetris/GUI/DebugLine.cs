namespace Tetris.GUI
{
    public class ChatLine
    {

        public string Message;
        public int UpdateCounter;
        
        public ChatLine(string s)
        {
            Message = s;
            UpdateCounter = 0;
        }
        
    }
}
using System.Xml.Serialization;

namespace Tetris.Settings
{ 
    public class GameOption<T>
    {
        [XmlElement]
        public string Name { get; set; }
        
        public GameOption(){}

        [XmlElement]
        public T Value;
        private T DefaultValue;
        
        public GameOption(string name, T defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
            Value = defaultValue;
        }

        public void SetValue(T val)
        {
            Value = val;
        }

        public T GetValue()
        {
            return Value;
        }
    }
}
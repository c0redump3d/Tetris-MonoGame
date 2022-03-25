using System.Xml.Serialization;

namespace Tetris.Settings;

public class GameOption<T>
{
    private T DefaultValue;
    [XmlElement] public string Name { get; set; }
    [XmlElement] public T Value;

    public GameOption()
    {
    }

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
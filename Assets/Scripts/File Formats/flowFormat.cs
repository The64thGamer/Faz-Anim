using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class flowFormat
{
    
    public flowControls[] characters { get; set; }

    public void Save(string filePath)
    {
        var formatter = new BinaryFormatter();
        using (var stream = File.Open(filePath, FileMode.Create))
            formatter.Serialize(stream, this);
    }
    public static flowFormat ReadFromFile(string filepath)
    {
        var formatter = new BinaryFormatter();
        using (var stream = File.OpenRead(filepath))
            return (flowFormat)formatter.Deserialize(stream);
    }
}
[System.Serializable]
public class flowControls
{
    public string name;
    public int[] flowsIn { get; set; }
    public int[] flowsOut { get; set; }
    public int[] weightIn { get; set; }
    public int[] weightOut { get; set; }
}

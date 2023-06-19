using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class rshwFile
{
    public byte[] audioData { get; set; }
    public int[] signalData { get; set; }
    public void Save(string filePath)
    {
        var formatter = new BinaryFormatter();
        using (var stream = File.Open(filePath, FileMode.Create))
            formatter.Serialize(stream, this);
    }
    public static rshwFile ReadFromFile(string filepath)
    {
        var formatter = new BinaryFormatter();
        using (var stream = File.OpenRead(filepath))
            return (rshwFile)formatter.Deserialize(stream);
    }
}

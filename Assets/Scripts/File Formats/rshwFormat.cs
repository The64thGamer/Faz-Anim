using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class rshwFormat
{
    public byte[] audioData { get; set; }
    public int[] signalData { get; set; }
    public byte[] videoData { get; set; }

    public void Save(string filePath)
    {
        var formatter = new BinaryFormatter();
        using (var stream = File.Open(filePath, FileMode.Create))
            formatter.Serialize(stream, this);
    }
    public static rshwFormat ReadFromFile(string filepath)
    {
        var formatter = new BinaryFormatter();
        using (var stream = File.OpenRead(filepath))
            if (stream.Length != 0)
            {
                stream.Position = 0;
                try
                {
                    return (rshwFormat)formatter.Deserialize(stream);
                }
                catch (System.Exception)
                {
                    return null;
                    throw;
                }

            }
            else
            {
                return null;
            }
    }
}

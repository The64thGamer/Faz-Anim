using SFB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertRshw : MonoBehaviour
{
    rshwFormat showtape;

    public void StartConvert()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape Audio", "", "rshw", false);
        if (paths.Length > 0)
        {
            SaveRecording(paths[0]);
        }
    }
    private void SaveRecording(string url)
    {
        //Check if null
        if (url != "")
        {
            //Load File
            rshwFile thefile = rshwFile.ReadFromFile(url);

            //Create Data
            showtape = new rshwFormat();
            showtape.audioData = thefile.audioData;
            showtape.signalData = thefile.signalData;

            List<int> newSignals = new List<int> { 0 };
            int e = 0;
            //Convert File
            for (int i = 0; i < showtape.signalData.Length; i++)
            {
                if (e < 2)
                {
                    while (showtape.signalData[i] != 0)
                    {
                        if (showtape.signalData[i] < 100)
                        {
                            newSignals.Add(showtape.signalData[i]);
                        }
                        else
                        {
                            newSignals.Add(showtape.signalData[i] + 50);
                        }
                        i++;
                    }
                    newSignals.Add(showtape.signalData[i]);
                    i++;
                    e++;
                }
                else
                {
                    e = 0;
                    while (showtape.signalData[i] != 0)
                    {
                        i++;
                    }
                    i++;
                }
            }
            showtape.signalData = newSignals.ToArray();

           //Save to file
           var path = StandaloneFileBrowser.SaveFilePanel("Save Showtape", "", "MyShowtape", "rshw");
            if (!string.IsNullOrEmpty(path))
            {
                showtape.Save(path);
            }
        }

    }
}

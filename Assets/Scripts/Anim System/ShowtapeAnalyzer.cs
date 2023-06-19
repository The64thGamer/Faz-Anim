using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShowtapeAnalyzer : MonoBehaviour
{
    public UI_ShowtapeManager manager;
    public TimelineEditor editor;
    public GameObject loadingScreen;
    public GameObject analyzeScreen;
    public loading_screen loading;
    bool cancelLoad;
    public Text analyzeHeader;
    public Text analyzeBody;
    public int exportAllSignals = 1;

    public void StartAnalysis(string type)
    {
        StartCoroutine(StartAnalysisCorou(type));
    }

    public void CancelLoad()
    {
        cancelLoad = true;
    }

    IEnumerator StartAnalysisCorou(string type)
    {
        loadingScreen.SetActive(true);
        loading.current = 0;
        loading.loadingMessage = "";
        loading.maximum = 1;
        yield return null;
        switch (type)
        {
            case "CompareTotalOn":
                {
                    float[] results = new float[300];
                    int[] bits = new int[300];
                    IEnumerator coroutine = CompareTotalOn(manager.rshwData, results, bits);
                    StartCoroutine(coroutine);
                    while (loading.current < loading.maximum)
                    {
                        if (cancelLoad)
                        {
                            break;
                        }
                        yield return null;
                    }
                    if (cancelLoad)
                    {
                        cancelLoad = false;
                        loading.current = 0;
                        loading.loadingMessage = "";
                        loading.maximum = 1;
                        loadingScreen.SetActive(false);
                        StopCoroutine(coroutine);
                        break;
                    }
                    yield return new WaitForSeconds(0.1f);
                    string header = "Compare total amount of times a bit is on for \"" + Path.GetFileName(manager.showtapeSegmentPaths[0]) + "\".";
                    string body = "";
                    for (int i = 0; i < results.Length; i++)
                    {
                        body += editor.windowMaker.SearchBitChartName(bits[i]) + "  >  " + results[i] + " seconds\n";
                    }
                    OpenWindow(header, body);
                    break;
                }
            case "CompareLongestOn":
                {
                    float[] results = new float[300];
                    int[] bits = new int[300];
                    IEnumerator coroutine = CompareLongestOn(manager.rshwData, results, bits);
                    StartCoroutine(coroutine);
                    while (loading.current < loading.maximum)
                    {
                        if (cancelLoad)
                        {
                            break;
                        }
                        yield return null;
                    }
                    if (cancelLoad)
                    {
                        cancelLoad = false;
                        loading.current = 0;
                        loading.loadingMessage = "";
                        loading.maximum = 1;
                        loadingScreen.SetActive(false);
                        StopCoroutine(coroutine);
                        break;
                    }
                    yield return new WaitForSeconds(0.1f);
                    string header = "Compare longest time a bit is on for \"" + Path.GetFileName(manager.showtapeSegmentPaths[0]) + "\".";
                    string body = "";
                    for (int i = 0; i < results.Length; i++)
                    {
                        body += editor.windowMaker.SearchBitChartName(bits[i]) + "  >  " + results[i] + " seconds\n";
                    }
                    OpenWindow(header, body);
                    break;
                }
            case "CompareTimesOn":
                {
                    float[] results = new float[300];
                    int[] bits = new int[300];
                    IEnumerator coroutine = CompareTimesOn(manager.rshwData, results, bits);
                    StartCoroutine(coroutine);
                    while (loading.current < loading.maximum)
                    {
                        if (cancelLoad)
                        {
                            break;
                        }
                        yield return null;
                    }
                    if (cancelLoad)
                    {
                        cancelLoad = false;
                        loading.current = 0;
                        loading.loadingMessage = "";
                        loading.maximum = 1;
                        loadingScreen.SetActive(false);
                        StopCoroutine(coroutine);
                        break;
                    }
                    yield return new WaitForSeconds(0.1f);
                    string header = "Compare every time a bit is on for \"" + Path.GetFileName(manager.showtapeSegmentPaths[0]) + "\".";
                    string body = "";
                    for (int i = 0; i < results.Length; i++)
                    {
                        body += editor.windowMaker.SearchBitChartName(bits[i]) + "  >  " + results[i] + " times pressed\n";
                    }
                    OpenWindow(header, body);
                    break;
                }
            case "ExportAllSignalsInBit":
                {
                    string body = "";
                    loading.loadingMessage = "(0%)";
                    int e = 0;
                    loading.maximum = manager.rshwData.Length;
                    for (int i = 0; i < manager.rshwData.Length; i++)
                    {
                        body += manager.rshwData[i].Get(exportAllSignals) ? 1 : 0;
                        loading.current = i;
                        if (i % (manager.rshwData.Length / 100) == 1)
                        {
                            e++;
                            loading.loadingMessage = "(" + e + "%)";
                            yield return null;
                        }
                    }
                    loading.loadingMessage = "(100%)";
                    yield return null;
                    if (cancelLoad)
                    {
                        cancelLoad = false;
                        loading.current = 0;
                        loading.loadingMessage = "";
                        loading.maximum = 1;
                        loadingScreen.SetActive(false);
                        break;
                    }
                    yield return new WaitForSeconds(0.1f);
                    analyzeBody.text = body;
                    loadingScreen.SetActive(false);
                    SaveWindowText();
                    break;
                }
            case "Extra Info":
                {
                    string header = "Extra info for \"" + Path.GetFileName(manager.showtapeSegmentPaths[0]) + "\".";
                    string body = "";
                    loading.loadingMessage = "(0%)";
                    yield return null;
                    for (int i = 0; i < 4; i++)
                    {
                        loading.loadingMessage = "(" + ((float)i / (float)5) + "%)";
                        switch (i)
                        {
                            case 0:
                                body += "Name: " + Path.GetFileName(manager.showtapeSegmentPaths[0]) + "\n";
                                break;
                            case 1:
                                body += "Signals Per Second: " + "60" + "\n";
                                break;
                            case 2:
                                body += "Audio Length: " + editor.audioLengthMax + "\n";
                                break;
                            case 3:
                                body += "Average Button Presses Per Second: " + AverageBitsPerSecond(manager.rshwData,editor.audioLengthMax) + "\n";
                                break;
                            default:
                                break;
                        }
                        if (cancelLoad)
                        {
                            break;
                        }
                        yield return null;
                    }
                    if (cancelLoad)
                    {
                        cancelLoad = false;
                        loading.current = 0;
                        loading.loadingMessage = "";
                        loading.maximum = 1;
                        loadingScreen.SetActive(false);
                        break;
                    }
                    yield return new WaitForSeconds(0.1f);
                    OpenWindow(header, body);
                    break;
                }
            default:
                break;
        }
    }

    float AverageBitsPerSecond(BitArray[] showTape, float length)
    {
        float times = 0;
        for (int i = 0; i < 300; i++)
        {
            bool previous = false;
            for (int e = 0; e < showTape.Length; e++)
            {
                if (showTape[e].Get(i))
                {
                    if (!previous)
                    {
                        times++;
                    }
                    previous = true;
                }
                else
                {
                    previous = false;
                }
            }
        }
        return times / length; ;
    }
    IEnumerator CompareTotalOn(BitArray[] showTape, float[] results, int[] bits)
    {
        loading.maximum = 300;
        loading.text.text = "(0/2)";
        for (int i = 0; i < 300; i++)
        {
            float frames = 0;
            for (int e = 0; e < showTape.Length; e++)
            {
                if (showTape[e].Get(i))
                {
                    frames++;
                }
            }
            bits[i] = i + 1;
            results[i] = frames / 60.0f; //IF VARIABLE SHOWTAPE DATA RATES ARE IMPLEMENTED FIX THIS
            loading.current = i + 1;
            if (i == 150)
            {
                loading.text.text = "(1/2)";
                yield return null;
            }
        }
        Array.Sort(results, bits);
        Array.Reverse(results);
        Array.Reverse(bits);
        loading.text.text = "(2/2)";
        yield return null;
        loading.current = loading.maximum;
        yield return null;
    }

    IEnumerator CompareLongestOn(BitArray[] showTape, float[] results, int[] bits)
    {
        loading.maximum = 300;
        loading.text.text = "(0/2)";
        for (int i = 0; i < 300; i++)
        {
            float current = 0;
            float longest = 0;
            for (int e = 0; e < showTape.Length; e++)
            {
                if (showTape[e].Get(i))
                {
                    current++;
                }
                else
                {
                    if (current > longest)
                    {
                        longest = current;
                    }
                    current = 0;
                }
            }
            bits[i] = i + 1;
            results[i] = longest / 60.0f; //IF VARIABLE SHOWTAPE DATA RATES ARE IMPLEMENTED FIX THIS
            loading.current = i + 1;
            if (i == 150)
            {
                loading.text.text = "(1/2)";
                yield return null;
            }
        }
        Array.Sort(results, bits);
        Array.Reverse(results);
        Array.Reverse(bits);
        loading.text.text = "(2/2)";
        yield return null;
        loading.current = loading.maximum;
        yield return null;
    }

    IEnumerator CompareTimesOn(BitArray[] showTape, float[] results, int[] bits)
    {
        loading.maximum = 300;
        loading.text.text = "(0/2)";
        for (int i = 0; i < 300; i++)
        {
            float times = 0;
            bool previous = false;
            for (int e = 0; e < showTape.Length; e++)
            {
                if (showTape[e].Get(i))
                {
                    if (!previous)
                    {
                        times++;
                    }
                    previous = true;
                }
                else
                {
                    previous = false;
                }

            }
            bits[i] = i + 1;
            results[i] = times;
            loading.current = i + 1;
            if (i == 150)
            {
                loading.text.text = "(1/2)";
                yield return null;
            }
        }
        Array.Sort(results, bits);
        Array.Reverse(results);
        Array.Reverse(bits);
        loading.text.text = "(2/2)";
        yield return null;
        loading.current = loading.maximum;
        yield return null;
    }


    void OpenWindow(string header, string body)
    {
        analyzeScreen.SetActive(true);
        loadingScreen.SetActive(false);
        analyzeHeader.text = header;
        analyzeBody.text = body;
    }

    public void CloseWindow()
    {
        analyzeScreen.SetActive(false);
    }

    public void SaveWindowText()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Save Analysis", "", "Analysis", "txt");
        if (!string.IsNullOrEmpty(path))
        {
            Debug.Log("Analysis Saved: " + Path.GetDirectoryName(path) + " >>>> " + Path.GetFileName(path));

            WriteFile(Path.GetDirectoryName(path) + "/", Path.GetFileName(path), analyzeBody.text);
        }
    }

    public static bool WriteFile(string path, string fileName, string data)
    {
        bool retValue = false;
        try
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            System.IO.File.WriteAllText(path + fileName, data);
            retValue = true;
        }
        catch (System.Exception ex)
        {
            string ErrorMessages = "File Write Error\n" + ex.Message;
            retValue = false;
            Debug.LogError(ErrorMessages);
        }
        return retValue;
    }
}

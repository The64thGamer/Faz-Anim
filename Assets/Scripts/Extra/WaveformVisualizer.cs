using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveformVisualizer : MonoBehaviour
{
    public RawImage raw;
    public AudioClip audioClip;
    public float[] samplesAll;
    int height = 20;
    public Color colwave = Color.white;
    public Color colbackground = Color.black;
    private void Start()
    {
        Texture2D tex = new Texture2D(Screen.width, height, TextureFormat.RGBA32, false);
        for (int x = 0; x < Screen.width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(x, y, colbackground);
            }
        }
        tex.Apply();
        raw.texture = tex;
    }
    public void GetAudioSamples()
    {
        samplesAll = new float[audioClip.samples];
        audioClip.GetData(samplesAll, 0);
    }

    public void PaintWaveformSpectrum(float viewzoomMin, float viewZoomMax, float audioLengthMax)
    {
        if(audioClip != null)
        {
            float screenPercent = 1 - ((viewZoomMax - viewzoomMin) / audioLengthMax);
            Texture2D tex = new Texture2D((Screen.width/2), height, TextureFormat.RGBA32, false);
            int offset = (int)remap(viewzoomMin, 0, audioLengthMax, 0, samplesAll.Length);
            int length = (int)remap(viewZoomMax, 0, audioLengthMax, 0, samplesAll.Length) - offset;

            float[] waveform = new float[(Screen.width / 2)];
            int packSize = (length / (Screen.width / 2)) + 1;
            int reducedPackSize = Mathf.Min(50,(int)(packSize * screenPercent * screenPercent)) +1;
            int s = 0;
            for (int i = offset; i + packSize < offset+length; i += packSize)
            {
                float samp = 0;
                for (int e = 0; e < packSize; e++)
                {
                    samp += Mathf.Abs(samplesAll[i + e]);
                    e += Mathf.FloorToInt(((float)packSize / (float)reducedPackSize) - 1);
                }
                waveform[s] = samp / (float)reducedPackSize;
                s++;
            }

            for (int x = 0; x < (Screen.width / 2); x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tex.SetPixel(x, y, colbackground);
                }
            }

            for (int x = 0; x < waveform.Length; x++)
            {
                for (int y = 0; y <= waveform[x] * ((float)height * .75f); y++)
                {
                    tex.SetPixel(x, (height / 2) + y, colwave);
                    tex.SetPixel(x, (height / 2) - y, colwave);
                }
            }
            tex.Apply();
            raw.texture = tex;
        }
    }
    public float remap(float val, float in1, float in2, float out1, float out2)
    {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }
}

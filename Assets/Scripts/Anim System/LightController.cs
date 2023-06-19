using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class LightController : MonoBehaviour
{
    public int lightBit;
    public char topOrBottom;
    [Range(0.01f, .3f)]
    public float fadeSpeed;
    public float intensity;
    public float intensityMultiplier = 1.0f;
    Mack_Valves bitChart;
    HDAdditionalLightData currentLight;
    public bool strobe;
    public bool flash;
    bool flashCheck;
    public bool materialLight = false;
    public GameObject emissiveObject;
    public string emmissiveMatName;
    Material emissiveTexture;
    public float emissiveMultiplier = 1;
    public Color emissiveMatColor = Color.white;
    public float nextTime = 0;
    public bool invertBit;

    private void Start()
    {
        bitChart = this.transform.root.Find("Mack Valves").GetComponent<Mack_Valves>();
        if (!materialLight)
        {
            currentLight = this.GetComponent<HDAdditionalLightData>();
        }
        else
        {
            foreach (Material matt in emissiveObject.GetComponent<MeshRenderer>().materials)
            {
                if (matt.name == emmissiveMatName)
                {
                    emissiveTexture = matt;
                    emissiveTexture.EnableKeyword("_EMISSIVE_COLOR_MAP");
                }
            }
        }
    }

    public void CreateMovements(float num3)
    {
        bool onOff = false;
        if (topOrBottom == 'T' && bitChart.topDrawer[lightBit - 1])
        {
            onOff = true;
        }
        else if (topOrBottom == 'B' && bitChart.bottomDrawer[lightBit - 1])
        {
            onOff = true;
        }
        if(invertBit)
        {
            onOff = !onOff;
        }
        if (flash)
        {
            if (onOff)
            {
                if (!flashCheck)
                {
                    flashCheck = true;
                    nextTime = 1;
                }
                else
                {
                    nextTime -= fadeSpeed * num3;
                }
            }
            else
            {
                if (flashCheck)
                {
                    flashCheck = false;
                }
                nextTime -= fadeSpeed * num3;
            }
        }
        else if (strobe)
        {
            if (onOff)
            {
                if (nextTime != 0)
                {
                    nextTime -= fadeSpeed * 2 * num3;
                }
                else
                {
                    nextTime = 1;
                }
            }
            else
            {
                nextTime -= fadeSpeed * 2 * num3;
            }
        }
        else
        {
            if (onOff)
            {
                nextTime += fadeSpeed * num3;
            }
            else
            {
                nextTime -= fadeSpeed * num3;
            }
        }
        nextTime = Mathf.Min(Mathf.Max(nextTime, 0), 1);
        if (!materialLight)
        {
            currentLight.intensity = intensity * nextTime * intensityMultiplier;
        }
        else
        {
            emissiveTexture.SetColor("_EmissiveColor", emissiveMatColor * nextTime * emissiveMultiplier * intensityMultiplier);
        }
    }
}

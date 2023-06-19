using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatSliderField : MonoBehaviour
{
    public Slider sli;
    public InputField inFi;
    public float min;
    public float max;
    public Slider color;
    public Slider hue;
    public Image colorImage;
    public Image hueImage;

    public void OnFieldChange()
    {
        inFi.text = (Mathf.Min(Mathf.Max(float.Parse(inFi.text),min),max)).ToString();
        if(sli != null)
        {
            sli.value = float.Parse(inFi.text);
        }
    }

    public void OnSliderChange()
    {
        inFi.text = sli.value.ToString();
    }

    public void OnColorChange()
    {
        colorImage.color = Color.HSVToRGB(color.value, hue.value, 1);
        hueImage.color = Color.HSVToRGB(color.value, 1, 1);
    }
}

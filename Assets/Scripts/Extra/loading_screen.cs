using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loading_screen : MonoBehaviour
{
    public int current;
    public int maximum;
    public string loadingMessage;
    public Image mask;
    public Text text;

    void Update()
    {
        float fillAmount = (float)current / (float)maximum;
        mask.fillAmount = fillAmount;
        text.text = loadingMessage;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    float FPS = 0;
    Text currentText;
    void Start()
    {
          currentText = this.GetComponent<Text>();
    }

    void Update()
    {
        FPS += ((Time.deltaTime / Time.timeScale) - FPS) * 0.03f;
        if (QualitySettings.vSyncCount == 1)
        {
            currentText.text = Mathf.Min((int)(1.0f / FPS), 60) + "FPS";
        }
        else
        {
            currentText.text = (int)(1.0f / FPS) + "FPS";
        }
        
    }
}

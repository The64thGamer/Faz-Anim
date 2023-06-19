using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FAde : MonoBehaviour
{
    public byte fadeTo;
    public byte fadeSpeed;
    byte fade = 255;
    CanvasGroup group;

    private void Awake()
    {
        group = this.GetComponent<CanvasGroup>();
    }

    void FixedUpdate()
    {
        for (int i = 0; i < fadeSpeed; i++)
        {

            if (fade < fadeTo)
            {
                fade++;
            }
            else if (fade > fadeTo)
            {
                fade--;
            }
            else
            {
                break;
            }
            group.alpha = (float)fade / 255.0f;
        }
    }
}

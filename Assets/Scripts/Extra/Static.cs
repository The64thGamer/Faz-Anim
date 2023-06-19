using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Static : MonoBehaviour
{
    RawImage staticUI;
    public bool flash;
    float alpha;
    public bool noAlpha = false;
    // Start is called before the first frame update
    void Start()
    {
        staticUI = this.GetComponent<RawImage>();
        alpha = staticUI.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        staticUI.uvRect = new Rect(Random.Range(0, 0.55f), Random.Range(0, 0.64f), staticUI.uvRect.width, staticUI.uvRect.height);
    }

    private void FixedUpdate()
    {
        if(!noAlpha)
        {
            if (staticUI.color.a > alpha)
            {
                Color currColor = staticUI.color;
                currColor.a -= .1f;
                staticUI.color = currColor;
            }
            if (flash)
            {
                Color currColor = staticUI.color;
                currColor.a = 1;
                staticUI.color = currColor;
                flash = false;
            }
        }
    }
}

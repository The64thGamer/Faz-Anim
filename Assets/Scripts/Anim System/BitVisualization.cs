using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BitVisualization : MonoBehaviour
{
    RawImage image;
    public Texture2D texture;
    public Mack_Valves mackvalves;
    public Color32[] colors;
    public Color offColor;
    public Color onColor;

    void Start()
    {
        colors = new Color32[20 * 60];
        image = this.GetComponent<RawImage>();
        texture = new Texture2D(20, 60, TextureFormat.ARGB32, false, false);
        texture.anisoLevel = 0;
        texture.filterMode = FilterMode.Point;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (mackvalves != null)
        {
            int e = 0;
            for (int i = 0; i < colors.Length; i++)
            {
                if (i % 2 == 0 && (i / 20) % 2 == 0)
                {
                    if (e < 150)
                    {
                        switch (mackvalves.topDrawer[e])
                        {
                            case true:
                                colors[i] = onColor;
                                break;
                            case false:
                                colors[i] = offColor;
                                break;
                        }
                    }
                    else if (e < 300)
                    {
                        switch (mackvalves.bottomDrawer[e - 150])
                        {
                            case true:
                                colors[i] = onColor;
                                break;
                            case false:
                                colors[i] = offColor;
                                break;
                        }

                    }
                    e++;
                }
            }
            texture.SetPixels32(colors);
            texture.Apply();
            image.texture = texture;
        }
    }
}

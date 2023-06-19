using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureController : MonoBehaviour
{
    public Mack_Valves bitChart;
    public List<int> texBits;
    public List<Texture2D> texes;
    public List<bool> boolChecks;
    public GameObject emissiveObject;
    public string emmissiveMatName;
    Material emissiveTexture;

    // Start is called before the first frame update
    private void Start()
    {

        foreach (Material matt in emissiveObject.GetComponent<MeshRenderer>().materials)
        {
            if (matt.name == emmissiveMatName)
            {
                emissiveTexture = matt;
                emissiveTexture.EnableKeyword("_EmissiveColorMap");
            }
        }
    }

    // Update is called once per frame
    public void CreateTex()
    {
        for (int i = 0; i < texBits.Count; i++)
        {
            if (bitChart.topDrawer[texBits[i] - 1])
            {
                if (!boolChecks[i])
                {
                    emissiveTexture.SetTexture("_EmissiveColorMap", texes[i]);
                    boolChecks[i] = true;
                }
            }
            else
            {
                boolChecks[i] = false;
            }
        }

    }
}

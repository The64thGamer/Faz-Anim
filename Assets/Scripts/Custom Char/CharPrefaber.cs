using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharPrefaber : MonoBehaviour
{
    public SkinnedMeshRenderer main;
    public MeshRenderer HandR;
    public MeshRenderer HandL;
    public CharacterPreset settings;
    public int playerNum = 1;

    public void LoadCharacterData()
    {
        int i = PlayerPrefs.GetInt("Player " + playerNum + ": CurrentPrefab") % 24;

        settings.bodySmoothness = PlayerPrefs.GetFloat("Player " + i + ": Smoothness");
        settings.bodyMetallic = PlayerPrefs.GetInt("Player " + i + ": Metallic") == 1;
        settings.bodyColor = ColorConvert("Player " + i + ": Body Color");
        settings.metalColor = ColorConvert("Player " + i + ": Metal Color");
        settings.characterHat = (uint)PlayerPrefs.GetInt("Player " + i + ": Hat");
        settings.characterFace = (uint)PlayerPrefs.GetInt("Player " + i + ": Face");
        settings.characterFaceDecal = (uint)PlayerPrefs.GetInt("Player " + i + ": Face Decal");
        UpdateCharacter();
    }
    Color32 ColorConvert(string path)
    {
        string str_color;
        str_color = PlayerPrefs.GetString(path);
        //Remove the header and brackets
        str_color = str_color.Replace("RGBA(", "");
        str_color = str_color.Replace(")", "");

        //Get the individual values (red green blue and alpha)
        var strings = str_color.Split(","[0]);

        Color outputcolor;
        outputcolor = Color.black;
        for (var i = 0; i < 4; i++)
        {
            outputcolor[i] = System.Single.Parse(strings[i]);
        }
        Color32 convert = outputcolor;
        return convert;
    }

    public void UpdateCharacter()
    {
        if (main != null)
        {
            main.materials[0].SetFloat("_Smoothness", 1 - Mathf.Min(settings.bodySmoothness * 2, 1));
            main.materials[0].SetFloat("_NormalScale", 2 * Mathf.Max(settings.bodySmoothness - 0.5f, 0));
            int metal = 0;
            if (settings.bodyMetallic)
            {
                metal = 1;
            }
            main.materials[0].SetFloat("_Metallic", metal);
            main.materials[0].color = settings.bodyColor;
            main.materials[1].color = settings.metalColor;
            main.materials[3].mainTexture = (Texture)Resources.Load("Face Decals/" + settings.characterFaceDecal);
        }
        if (HandR != null)
        {
            HandR.materials[0].SetFloat("_Smoothness", 1 - Mathf.Min(settings.bodySmoothness * 2, 1));
            HandR.materials[0].SetFloat("_NormalScale", 2 * Mathf.Max(settings.bodySmoothness - 0.5f, 0));
            int metal = 0;
            if (settings.bodyMetallic)
            {
                metal = 1;
            }
            HandR.materials[0].SetFloat("_Metallic", metal);
            HandR.materials[0].color = settings.bodyColor;
        }
        if (HandL != null)
        {
            HandL.materials[0].SetFloat("_Smoothness", 1 - Mathf.Min(settings.bodySmoothness * 2, 1));
            HandL.materials[0].SetFloat("_NormalScale", 2 * Mathf.Max(settings.bodySmoothness - 0.5f, 0));
            int metal = 0;
            if (settings.bodyMetallic)
            {
                metal = 1;
            }
            HandL.materials[0].SetFloat("_Metallic", metal);
            HandL.materials[0].color = settings.bodyColor;
        }
    }
}

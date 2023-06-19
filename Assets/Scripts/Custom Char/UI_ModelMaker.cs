using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ModelMaker : MonoBehaviour
{
    public CharacterPreset[] presets;
    public GameObject characterUIPrefab;
    public GameObject charIconHolder;
    public Sprite headMetal;
    public Sprite[] faceDecals;
    public GameObject selector;
    public Slider roughSlider;
    public Toggle toggle;
    public CharPrefaber model;
    public Slider huea, sata, vala;
    public Slider hueb, satb, valb;

    public GameObject menuPresetIcons;
    public GameObject menuEditCharacter;

    public int currentSelection;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("First Time: Characters") == 0)
        {
            FirstTimeCharacters();
        }
        LoadCharacterData();
        UpdateIcons();
        NewSelection(0);
    }

    public void MenuChangeEditCharacter()
    {
        menuEditCharacter.SetActive(true);
        menuPresetIcons.SetActive(false);
        Color.RGBToHSV(presets[currentSelection].bodyColor, out float H, out float S, out float V);
        huea.value = H;
        sata.value = S;
        vala.value = V;
        Color.RGBToHSV(presets[currentSelection].metalColor, out float HH, out float SS, out float VV);
        hueb.value = HH;
        satb.value = SS;
        valb.value = VV;
        toggle.isOn = presets[currentSelection].bodyMetallic;
        roughSlider.value = presets[currentSelection].bodySmoothness;
        model.UpdateCharacter();
    }

    public void MenuChangePresetIcons()
    {
        SaveCharacterData();
        menuEditCharacter.SetActive(false);
        menuPresetIcons.SetActive(true);
        UpdateIcons();
    }

    public void AssignPlayer(int p)
    {
        PlayerPrefs.SetInt("Player " + p + ": CurrentPrefab",currentSelection);
    }

    public void UpdateCharacter()
    {
        model.settings = presets[currentSelection];
        model.UpdateCharacter();
    }

    public void RoughSliderUpdate()
    {
        presets[currentSelection].bodySmoothness = roughSlider.value;
        UpdateCharacter();
    }

    public void MetallicUpdate()
    {
        presets[currentSelection].bodyMetallic = toggle.isOn;
        UpdateCharacter();
    }

    public void ColorMainUpdate()
    {
        presets[currentSelection].bodyColor = Color.HSVToRGB(huea.value, sata.value, vala.value);
        UpdateCharacter();
    }

    public void ColorMetalUpdate()
    {
        presets[currentSelection].metalColor = Color.HSVToRGB(hueb.value, satb.value, valb.value);
        UpdateCharacter();
    }

    public void FaceUpdate(int face)
    {
        presets[currentSelection].characterFaceDecal = (uint)face;
        UpdateCharacter();
    }

    public void NewSelection(int select)
    {
        currentSelection = select;
        selector.GetComponent<RectTransform>().anchoredPosition = new Vector2(((currentSelection % 6) * 160) - 1238, -(Mathf.Floor(currentSelection / 6) * 160) + 238);
        UpdateCharacter();
    }

    public void UpdateIcons()
    {
        foreach (Transform child in charIconHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < 24; i++)
        {
            GameObject icon = GameObject.Instantiate(characterUIPrefab, charIconHolder.transform);
            icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(((i % 6) * 400) - 1145, -(Mathf.Floor(i / 6) * 400) - 300);
            icon.transform.GetChild(0).GetComponent<Image>().color = presets[i].bodyColor;
            if (presets[i].bodyMetallic)
            {
                icon.transform.GetChild(0).GetComponent<Image>().sprite = headMetal;
            }
            icon.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = presets[i].metalColor;
            icon.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = faceDecals[presets[i].characterFaceDecal];
            if (faceDecals[presets[i].characterFaceDecal] == null)
            {
                icon.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                icon.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
            }
            icon.GetComponent<CharIconSelectButton>().num = i;
        }
    }

    void FirstTimeCharacters()
    {
        PlayerPrefs.SetInt("First Time: Characters", 1);

        Color c = new Color32(255, 190, 0, 255);

        for (int i = 0; i < 24; i++)
        {
            PlayerPrefs.SetFloat("Player " + i + ": Smoothness", 0.1f);
            PlayerPrefs.SetInt("Player " + i + ": Metallic", 0);
            PlayerPrefs.SetString("Player " + i + ": Body Color", c.ToString());
            PlayerPrefs.SetString("Player " + i + ": Metal Color", Color.white.ToString());
            PlayerPrefs.SetInt("Player " + i + ": Hat", 0);
            PlayerPrefs.SetInt("Player " + i + ": Face", 0);
            PlayerPrefs.SetInt("Player " + i + ": Face Decal", 0);
        }
    }

    void SaveCharacterData()
    {
        for (int i = 0; i < 24; i++)
        {
            PlayerPrefs.SetFloat("Player " + i + ": Smoothness", presets[i].bodySmoothness);
            if (presets[i].bodyMetallic)
            {
                PlayerPrefs.SetInt("Player " + i + ": Metallic", 1);
            }
            else
            {
                PlayerPrefs.SetInt("Player " + i + ": Metallic", 0);
            }
            PlayerPrefs.SetString("Player " + i + ": Body Color", ((Color)(presets[i].bodyColor)).ToString());
            PlayerPrefs.SetString("Player " + i + ": Metal Color", ((Color)(presets[i].metalColor)).ToString());
            PlayerPrefs.SetInt("Player " + i + ": Hat", (int)presets[i].characterHat);
            PlayerPrefs.SetInt("Player " + i + ": Face", (int)presets[i].characterFace);
            PlayerPrefs.SetInt("Player " + i + ": Face Decal", (int)presets[i].characterFaceDecal);
        }

        LoadCharacterData();
    }

    void LoadCharacterData()
    {
        for (int i = 0; i < 24; i++)
        {
            presets[i].bodySmoothness = PlayerPrefs.GetFloat("Player " + i + ": Smoothness");
            presets[i].bodyMetallic = PlayerPrefs.GetInt("Player " + i + ": Metallic") == 1;
            presets[i].bodyColor = ColorConvert("Player " + i + ": Body Color");
            presets[i].metalColor = ColorConvert("Player " + i + ": Metal Color");
            presets[i].characterHat = (uint)PlayerPrefs.GetInt("Player " + i + ": Hat");
            presets[i].characterFace = (uint)PlayerPrefs.GetInt("Player " + i + ": Face");
            presets[i].characterFaceDecal = (uint)PlayerPrefs.GetInt("Player " + i + ": Face Decal");
        }
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
        return outputcolor;
    }
}

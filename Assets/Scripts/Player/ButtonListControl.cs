using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonListControl : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonTemplate;

    [SerializeField]
    private GameObject buttonHolder;

    public LayerMask uiLayerMask;

    public string currentItemName;

    public GameObject attributeHolder;
    public GameObject attributeWindow;
    public Text attName;
    public Text attDesc;
    public Image attImage;
    public int attributeCount;

    //Attribute Templates
    public GameObject atttempSkin;
    public GameObject atttempScale;
    public GameObject atttempComplexScale;
    public GameObject atttempLightTemp;
    public GameObject atttempLightColor;
    public GameObject atttempLightIntensity;
    public GameObject atttempLightSpotSize;
    public GameObject atttempMaterialColor;

    public void Quit()
    {
        Application.Quit();
    }

    public void QuitToTitle()
    {
        SceneManager.LoadScene("Title Screen", LoadSceneMode.Single);
        Destroy(this.transform.root.gameObject);
    }

    private void OnDisable()
    {
        foreach (Transform child in buttonHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void CreateButtons(int index)
    {
        DisableAttributePage();

        //Delete previous buttons
        foreach (Transform child in buttonHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //Create buttons
        GlobalController gg = GameObject.Find("Global Controller").GetComponent<GlobalController>();
        for (int i = 0; i < gg.prizeStrings[index].prizeStrings.Length; i++)
        {
            if (PlayerPrefs.GetInt(gg.prizeStrings[index].prizeStrings[i].name) == 64 || gg.unlockAllPrizes)
            {
                Debug.Log("Prize UI Loader: " + gg.prizeStrings[index].prizeStrings[i].name + " is REAL");
                GameObject button = Instantiate(buttonTemplate);
                button.SetActive(true);
                button.name = gg.prizeStrings[index].prizeStrings[i].name;
                button.transform.SetParent(buttonHolder.transform, false);
                button.GetComponent<ButtonListButton>().StartAwake();
            }
            else if (PlayerPrefs.GetInt(gg.prizeStrings[index].prizeStrings[i].name) == 0)
            {
                Debug.Log("Prize UI Loader: " + gg.prizeStrings[index].prizeStrings[i].name + " is NOT REAL");
            }
            else
            {
                Debug.Log("Prize UI Loader: " + gg.prizeStrings[index].prizeStrings[i].name + " is CHEATED");
            }
        }
    }

    public void CreateAttributePage(string icon)
    {
        //Delete previous buttons
        foreach (Transform child in attributeHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        attributeCount = 0;
        attributeWindow.SetActive(true);
        currentItemName = icon;
        attName.text = currentItemName;
        //Create buttons
        GlobalController gg = GameObject.Find("Global Controller").GetComponent<GlobalController>();
        for (int e = 0; e < gg.prizeStrings.Length; e++)
        {
            for (int i = 0; i < gg.prizeStrings[e].prizeStrings.Length; i++)
            {
                if (currentItemName == gg.prizeStrings[e].prizeStrings[i].name)
                {
                    attDesc.text = gg.prizeStrings[e].prizeStrings[i].description;
                    attImage.sprite = Resources.Load<Sprite>("Merch/Icons/" + currentItemName) as Sprite;
                    if (gg.prizeStrings[e].prizeStrings[i].skins.Length > 0)
                    {
                        AddSkinsAttribute(gg.prizeStrings[e].prizeStrings[i]);
                    }
                    for (int j = 0; j < gg.prizeStrings[e].prizeStrings[i].attributes.Length; j++)
                    {
                        AddGenericAttribute(gg.prizeStrings[e].prizeStrings[i], gg.prizeStrings[e].prizeStrings[i].attributes[j]);
                    }
                }
            }
        }
    }

    public void UpdateItemIcon(Dropdown dropdown)
    {
        //Create buttons
        GlobalController gg = GameObject.Find("Global Controller").GetComponent<GlobalController>();
        for (int e = 0; e < gg.prizeStrings.Length; e++)
        {
            for (int i = 0; i < gg.prizeStrings[e].prizeStrings.Length; i++)
            {
                if (currentItemName == gg.prizeStrings[e].prizeStrings[i].name)
                {
                    if (gg.prizeStrings[e].prizeStrings[i].skins.Length > 0)
                    {
                        if (gg.prizeStrings[e].prizeStrings[i].skins[0] == "Building Block")
                        {
                            attImage.sprite = Resources.Load<Sprite>("Merch/Icons/" + currentItemName) as Sprite;
                        }
                        else
                        {
                            if (dropdown.value == 0)
                            {
                                attImage.sprite = Resources.Load<Sprite>("Merch/Icons/" + currentItemName) as Sprite;
                            }
                            else
                            {
                                attImage.sprite = Resources.Load<Sprite>("Merch/Icons/" + currentItemName + dropdown.value) as Sprite;
                            }
                        }
                    }
                    return;
                }
            }
        }
    }

    void AddSkinsAttribute(PrizeStrings ps)
    {
        GameObject att = Instantiate(atttempSkin) as GameObject;
        att.SetActive(true);
        att.name = "Skins";
        att.transform.parent = attributeHolder.transform;
        att.GetComponent<RectTransform>().anchoredPosition = new Vector2(342.7f + (310 * attributeCount), 790 - (90 * (attributeCount / 4)));
        att.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        att.transform.GetComponentInChildren<Dropdown>().options.Clear();
        if (ps.skins[0] == "Building Block")
        {
            GlobalController gg = GameObject.Find("Global Controller").GetComponent<GlobalController>();
            for (int i = 0; i < gg.buildMaterials.Length; i++)
            {
                att.transform.GetComponentInChildren<Dropdown>().options.Add(new Dropdown.OptionData(gg.buildMaterials[i].name));
            }
        }
        else
        {
            for (int i = 0; i < ps.skins.Length; i++)
            {
                att.transform.GetComponentInChildren<Dropdown>().options.Add(new Dropdown.OptionData(ps.skins[i]));
            }
        }
        attributeCount++;
    }

    void AddGenericAttribute(PrizeStrings ps, string theCase)
    {
        GameObject att = null;
        switch (theCase)
        {
            case "Scale":
                att = Instantiate(atttempScale);
                break;
            case "Complex Scale":
                att = Instantiate(atttempComplexScale);
                break;
            case "Light Temperature":
                att = Instantiate(atttempLightTemp);
                break;
            case "Light Intensity":
                att = Instantiate(atttempLightIntensity);
                break;
            case "Light Spot Size":
                att = Instantiate(atttempLightSpotSize);
                break;
            case "Light Color":
                att = Instantiate(atttempLightColor);
                break;
            case "Material Color":
                att = Instantiate(atttempMaterialColor);
                break;
            default:
                break;
        }
        att.name = theCase;
        att.SetActive(true);
        att.transform.parent = attributeHolder.transform;
        att.GetComponent<RectTransform>().anchoredPosition = new Vector2(334.0f + (305 * attributeCount), 790 - (85 * (attributeCount / 4)));
        att.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        attributeCount++;
    }



    public void DisableAttributePage()
    {
        attributeWindow.SetActive(false);
        attributeCount = 0;
    }

    public void CreatePrize()
    {
        RaycastHit hit = new RaycastHit();
        float length = 7;
        while (true)
        {
            if (Physics.SphereCast(Camera.main.transform.position, 2, Camera.main.transform.forward, out hit, length, uiLayerMask))
            {
                length -= 0.01f;
            }
            else
            {
                Debug.Log("Created Correctly > " + length);
                break;
            }
            if (length < 0.5f)
            {
                Debug.Log("Created Close");
                break;
            }
        }
        AudioSource ez = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        ez.clip = (AudioClip)Resources.Load("Create");
        ez.pitch = 1.0f;
        ez.Play();
        Vector3 finalPoint = Camera.main.transform.position + Camera.main.transform.forward * length;
        GameObject prize = GameObject.Instantiate(Resources.Load("Merch/Prefabs/" + currentItemName) as GameObject, finalPoint, Quaternion.identity);
        DontDestroyOnLoad(prize);

        //Create buttons
        GlobalController gg = GameObject.Find("Global Controller").GetComponent<GlobalController>();
        for (int e = 0; e < gg.prizeStrings.Length; e++)
        {
            for (int i = 0; i < gg.prizeStrings[e].prizeStrings.Length; i++)
            {
                if (currentItemName == gg.prizeStrings[e].prizeStrings[i].name)
                {
                    //Startup
                    prize.SendMessage("ATTStartup", SendMessageOptions.DontRequireReceiver);
                    foreach (Transform child in prize.transform)
                    {
                        child.SendMessage("ATTStartup", SendMessageOptions.DontRequireReceiver);
                    }

                    //Skins
                    if (gg.prizeStrings[e].prizeStrings[i].skins.Length > 0)
                    {
                        string skin;
                        if (gg.prizeStrings[e].prizeStrings[i].skins[0] == "Building Block")
                        {
                            skin = attributeHolder.transform.Find("Skins").Find("Dropdown").GetComponent<Dropdown>().options[attributeHolder.transform.Find("Skins").Find("Dropdown").GetComponent<Dropdown>().value].text;
                        }
                        else
                        {
                            skin = gg.prizeStrings[e].prizeStrings[i].skins[attributeHolder.transform.Find("Skins").Find("Dropdown").GetComponent<Dropdown>().value];
                        }
                        prize.SendMessage("ATTSkin", skin, SendMessageOptions.DontRequireReceiver);
                        foreach (Transform child in prize.transform)
                        {
                            child.SendMessage("ATTSkin", skin, SendMessageOptions.DontRequireReceiver);
                        }
                    }

                    //Attributes
                    for (int j = 0; j < gg.prizeStrings[e].prizeStrings[i].attributes.Length; j++)
                    {
                        switch (gg.prizeStrings[e].prizeStrings[i].attributes[j])
                        {
                            case "Scale":
                                float scale = attributeHolder.transform.Find(gg.prizeStrings[e].prizeStrings[i].attributes[j]).Find("Slider").GetComponent<Slider>().value;
                                prize.SendMessage("ATTScale", scale, SendMessageOptions.DontRequireReceiver);
                                foreach (Transform child in prize.transform)
                                {
                                    child.SendMessage("ATTScale", scale, SendMessageOptions.DontRequireReceiver);
                                }
                                break;
                            case "Complex Scale":
                                GameObject sc = attributeHolder.transform.Find(gg.prizeStrings[e].prizeStrings[i].attributes[j]).gameObject;
                                Vector3 cscale = new Vector3(float.Parse(sc.transform.Find("X").GetComponent<InputField>().text), float.Parse(sc.transform.Find("Y").GetComponent<InputField>().text), float.Parse(sc.transform.Find("Z").GetComponent<InputField>().text));
                                prize.SendMessage("ATTComplexScale", cscale, SendMessageOptions.DontRequireReceiver);
                                foreach (Transform child in prize.transform)
                                {
                                    child.SendMessage("ATTComplexScale", cscale, SendMessageOptions.DontRequireReceiver);
                                }
                                break;
                            case "Light Temperature":
                                GameObject lighttemp = attributeHolder.transform.Find(gg.prizeStrings[e].prizeStrings[i].attributes[j]).gameObject;
                                int temp = (int)lighttemp.transform.Find("Slider").GetComponent<Slider>().value;
                                prize.SendMessage("ATTLightTemperature", temp, SendMessageOptions.DontRequireReceiver);
                                foreach (Transform child in prize.transform)
                                {
                                    child.SendMessage("ATTLightTemperature", temp, SendMessageOptions.DontRequireReceiver);
                                }
                                break;
                            case "Light Intensity":
                                GameObject lightinten = attributeHolder.transform.Find(gg.prizeStrings[e].prizeStrings[i].attributes[j]).gameObject;
                                float intensity = lightinten.transform.Find("Slider").GetComponent<Slider>().value;
                                prize.SendMessage("ATTLightIntensity", intensity, SendMessageOptions.DontRequireReceiver);
                                foreach (Transform child in prize.transform)
                                {
                                    child.SendMessage("ATTLightIntensity", intensity, SendMessageOptions.DontRequireReceiver);
                                }
                                break;
                            case "Light Spot Size":
                                GameObject spotsize = attributeHolder.transform.Find(gg.prizeStrings[e].prizeStrings[i].attributes[j]).gameObject;
                                float outer = spotsize.transform.Find("Outer Angle").GetComponent<Slider>().value;
                                float inner = spotsize.transform.Find("Inner Angle").GetComponent<Slider>().value;
                                Vector2 both = new Vector2(outer, inner);
                                prize.SendMessage("ATTLightSpotSize", both, SendMessageOptions.DontRequireReceiver);
                                foreach (Transform child in prize.transform)
                                {
                                    child.SendMessage("ATTLightSpotSize", both, SendMessageOptions.DontRequireReceiver);
                                }
                                break;
                            case "Light Color":
                                GameObject color = attributeHolder.transform.Find(gg.prizeStrings[e].prizeStrings[i].attributes[j]).gameObject;
                                Color colorreal = color.transform.Find("Color").GetComponent<Image>().color;
                                prize.SendMessage("ATTLightColor", colorreal, SendMessageOptions.DontRequireReceiver);
                                foreach (Transform child in prize.transform)
                                {
                                    child.SendMessage("ATTLightColor", colorreal, SendMessageOptions.DontRequireReceiver);
                                }
                                break;
                            case "Material Color":
                                GameObject cc = attributeHolder.transform.Find(gg.prizeStrings[e].prizeStrings[i].attributes[j]).gameObject;
                                Color cr = cc.transform.Find("Color").GetComponent<Image>().color;
                                prize.SendMessage("ATTMaterialColor", cr, SendMessageOptions.DontRequireReceiver);
                                foreach (Transform child in prize.transform)
                                {
                                    child.SendMessage("ATTMaterialColor", cr, SendMessageOptions.DontRequireReceiver);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}


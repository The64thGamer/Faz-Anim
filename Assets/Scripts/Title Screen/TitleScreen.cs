
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public GameObject starlow;
    public GameObject starmed;
    public GameObject starhigh;

    public GameObject vsyncText;
    public GameObject textureText;
    public GameObject resText;
    public GameObject motionText;
    public GameObject exposureText;
    public GameObject versionText;
    public GameObject ssrText;
    public GameObject ssaoText;
    public GameObject timeofdayText;
    public GameObject resPercentText;
    public GameObject dlssText;

    public GameObject settingW;
    public GameObject faqW;
    public GameObject creditsW;
    public GameObject characterW;
    public GameObject downButton;

    public GameObject showtimeButton;
    public GameObject logo;
    public GameObject logoTwo;

    public Button charcustomBtn;
    public Button editorBtn;
    public Button sandboxBtn;

    CanvasGroup settingG;
    CanvasGroup faqG;
    CanvasGroup creditsG;
    CanvasGroup characterG;

    public RawImage fade;

    bool barUp;
    public BarMove[] bars;

    public VolumeProfile regularVolume;

    public bool stopUpdate;
    public bool deletePlayerPrefsAll;
    public GameObject player;

    public bool disableRaytracing;
    public bool fadeWhichWay = false;
    public string sceneLoadCache;

    private void Start()
    {
        TextureXR.maxViews = 2;
        if (!stopUpdate)
        {
            if (PlayerPrefs.GetInt("Tutorial Save 0") != 0 || GameVersion.gameName != "Faz-Anim")
            {
                charcustomBtn.interactable = true;
                editorBtn.interactable = true;
                sandboxBtn.interactable = true;
            }
            settingG = settingW.GetComponent<CanvasGroup>();
            faqG = faqW.GetComponent<CanvasGroup>();
            creditsG = creditsW.GetComponent<CanvasGroup>();
            characterG = characterW.GetComponent<CanvasGroup>();
            versionText.GetComponent<Text>().text = "Ver. " + GameVersion.gameVersion;
        }
        if (disableRaytracing)
        {
            if (PlayerPrefs.GetInt("Settings: SSR") >= 5)
            {
                PlayerPrefs.SetInt("Settings: SSR", 4);
            }
            if (PlayerPrefs.GetInt("Settings: SSAO") >= 3)
            {
                PlayerPrefs.SetInt("Settings: SSAO", 1);
            }
        }
        QualitySave.FirstTimeSave();
        UpdateSettings();
        if (GameVersion.isVR == "true")
        {
            Debug.Log("IsVR is True.");
            StartShow(false);
        }
        else
        {
            Debug.Log("IsVR is False.");
            fadeWhichWay = false;
        }
        //SceneManager.LoadScene("Arcade Mr Hugs", LoadSceneMode.Single);
    }

    private void Update()
    {
        if (fadeWhichWay)
        {
            fade.color = new Color(1, 1, 1, Mathf.Min(1,fade.color.a + (2 * Time.deltaTime)));
        }
        else
        {
            fade.color = new Color(1, 1, 1, Mathf.Max(0,fade.color.a - (.4f * Time.deltaTime)));
        }
    }

    void FixedUpdate()
    {
        if(sceneLoadCache != "")
        {
            SceneLoadCheck();
        }
        if (deletePlayerPrefsAll)
        {
            deletePlayerPrefsAll = false;
            PlayerPrefs.DeleteAll();
        }
        if (!stopUpdate)
        {
            if (GameVersion.gameName == "Faz-Anim")
            {
                Destroy(logoTwo);
            }
            else
            {
                Destroy(logo);
            }
            if (barUp)
            {
                settingG.alpha += .1f;
                faqG.alpha += .1f;
                creditsG.alpha += .1f;
                characterG.alpha += .1f;
            }
            else
            {
                settingG.alpha -= .15f;
                faqG.alpha -= .15f;
                creditsG.alpha -= .15f;
                characterG.alpha -= .15f;
            }
            if (settingG.alpha <= 0)
            {
                settingW.SetActive(false);
            }
            if (faqG.alpha <= 0)
            {
                faqW.SetActive(false);
            }
            if (creditsG.alpha <= 0)
            {
                creditsW.SetActive(false);
            }
            if (characterG.alpha <= 0)
            {
                characterW.SetActive(false);
            }
        }
    }

    public void StartEditor()
    {
        if (sceneLoadCache == "")
        {
            AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
            sc.clip = (AudioClip)Resources.Load("big tap");
            sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
            sc.Play();
            sceneLoadCache = "Bit Crusher";
            fadeWhichWay = true;
        }
    }
    public void StartShow(bool sandbox)
    {
        if (sceneLoadCache == "")
        {
            AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
            sc.clip = (AudioClip)Resources.Load("big tap");
            sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
            sc.Play();
            if (!sandbox)
            {
                if (GameVersion.gameName != "Faz-Anim")
                {
                    sceneLoadCache = "Rival Restaurant";
                }
                else
                {
                    sceneLoadCache = "Front Entrance";
                }
            }
            else
            {
                sceneLoadCache = "Sandbox";
            }
            fadeWhichWay = true;
        }
    }

    void SceneLoadCheck()
    {
        if(fadeWhichWay && fade.color.a >= 1)
        {
            SceneManager.LoadScene(sceneLoadCache, LoadSceneMode.Single);
        }
    }

    public void SettingsMenu()
    {
        MenuFunc();
        creditsW.SetActive(false);
        faqW.SetActive(false);
        settingW.SetActive(true);
        characterW.SetActive(false);
        showtimeButton.SetActive(false);
    }
    public void FAQMenu()
    {
        MenuFunc();
        creditsW.SetActive(false);
        faqW.SetActive(true);
        settingW.SetActive(false);
        characterW.SetActive(false);
        showtimeButton.SetActive(false);
    }
    public void CreditsMenu()
    {
        MenuFunc();
        creditsW.SetActive(true);
        faqW.SetActive(false);
        settingW.SetActive(false);
        characterW.SetActive(false);
        showtimeButton.SetActive(false);
    }
    public void CharacterMenu()
    {
        MenuFunc();
        creditsW.SetActive(false);
        faqW.SetActive(false);
        settingW.SetActive(false);
        characterW.SetActive(true);
        showtimeButton.SetActive(false);
    }

    public void MenuFunc()
    {
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("big tap");
        sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        sc.Play();
        barUp = true;
        downButton.SetActive(true);
        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].transition = true;
        }
    }

    public void Down()
    {
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("tap");
        sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        sc.Play();
        downButton.SetActive(false);
        barUp = false;
        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].transition = false;
        }
        showtimeButton.SetActive(true);
    }

    public void TitleScreenMenu()
    {
        if (sceneLoadCache == "")
        {
            AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
            sc.clip = (AudioClip)Resources.Load("tap");
            sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
            sc.Play();
            if (PlayerPrefs.GetInt("Intro: TutorialA") == 0)
            {
                ////////////////////////////////////////////////SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
                sceneLoadCache = "Title Screen";
                fadeWhichWay = true;
            }
            else
            {
                sceneLoadCache = "Title Screen";
                fadeWhichWay = true;
            }
        }
    }

    public void Exit()
    {
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("big tap");
        sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        sc.Play();
        Application.Quit();
    }

    public void SetQuality(int quality)
    {
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("ting");
        sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        sc.Play();
        PlayerPrefs.SetInt("Settings: Quality", quality);
        UpdateSettings();
    }
    public void SetSSR(int quality)
    {
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("ting");
        sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        sc.Play();
        int check = PlayerPrefs.GetInt("Settings: SSR") + quality;
        if (check < 0)
        {
            check = 0;
        }
        if (check > 6 && !disableRaytracing)
        {
            check = 6;
        }
        if (check > 4 && disableRaytracing)
        {
            check = 4;
        }
        PlayerPrefs.SetInt("Settings: SSR", check);
        UpdateSettings();
    }
    public void SetSSAO(int quality)
    {
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("ting");
        sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        sc.Play();
        int check = PlayerPrefs.GetInt("Settings: SSAO") + quality;
        if (check < 0)
        {
            check = 0;
        }
        if (check > 5 && !disableRaytracing)
        {
            check = 5;
        }
        if (check > 2 && disableRaytracing)
        {
            check = 2;
        }
        PlayerPrefs.SetInt("Settings: SSAO", check);
        UpdateSettings();
    }
    
    
    public void SetTextureQ(int quality)
    {
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("ting");
        sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        sc.Play();
        int check = PlayerPrefs.GetInt("Settings: Texture") + quality;
        if (check < 0)
        {
            check = 0;
        }
        if (check > 3)
        {
            check = 3;
        }
        PlayerPrefs.SetInt("Settings: Texture", check);
        UpdateSettings();
    }

    public void SetWindowed(int quality)
    {
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("ting");
        sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        sc.Play();
        PlayerPrefs.SetInt("Settings: Windowed", quality);
        UpdateSettings();
    }

    public void SetMotionBlur(int onoff)
    {
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("ting");
        sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        sc.Play();
        PlayerPrefs.SetInt("Settings: Motion Blur", onoff);
        UpdateSettings();
    }

    public void SetAutoExposure(int onoff)
    {
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("ting");
        sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        sc.Play();
        PlayerPrefs.SetInt("Settings: Auto Exposure", onoff);
        UpdateSettings();
    }

    public void SetPlaybackRate(int quality)
    {
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("ting");
        sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        sc.Play();
        int check = PlayerPrefs.GetInt("Settings: Playback") + quality;
        if (check < 0)
        {
            check = 0;
        }
        if (check > 1)
        {
            check = 1;
        }
        PlayerPrefs.SetInt("Settings: Playback", check);
        UpdateSettings();
    }
    public void SetTimeOfDay(int quality)
    {
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("ting");
        sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        sc.Play();
        int check = PlayerPrefs.GetInt("Settings: Time of Day") + quality;
        if (check < 0)
        {
            check = 0;
        }
        if (check > 4)
        {
            check = 4;
        }
        PlayerPrefs.SetInt("Settings: Time of Day", check);
        UpdateSettings();
    }
    public void SetVsync(int onOff)
    {

        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("ting");
        sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        sc.Play();
        PlayerPrefs.SetInt("Settings: VSync", onOff);
        UpdateSettings();
    }
    public void SetResPercent(int quality)
    {
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("ting");
        sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        sc.Play();
        int check = PlayerPrefs.GetInt("Settings: Res Percent") + quality;
        if (check < 0)
        {
            check = 0;
        }
        if (check > 18)
        {
            check = 18;
        }
        PlayerPrefs.SetInt("Settings: Res Percent", check);
        UpdateSettings();
    }
    public void SetDLSS(int quality)
    {
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("ting");
        sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        sc.Play();
        int check = PlayerPrefs.GetInt("Settings: DLSS") + quality;
        if (check < 0)
        {
            check = 0;
        }
        if (check > 4)
        {
            check = 4;
        }
        PlayerPrefs.SetInt("Settings: DLSS", check);
        UpdateSettings();
    }
    void UpdateSettings()
    {
        QualitySave.ApplySavedQualitySettings();
        if (!stopUpdate)
        {
            if (vsyncText != null)
            {
                if (QualitySettings.vSyncCount == 0)
                {
                    vsyncText.GetComponent<Text>().text = "Off";
                }
                else
                {
                    vsyncText.GetComponent<Text>().text = "On";
                }
                starlow.SetActive(false);
                starmed.SetActive(false);
                starhigh.SetActive(false);
                switch (PlayerPrefs.GetInt("Settings: Quality"))
                {
                    case 0:
                        starlow.SetActive(true);
                        break;
                    case 1:
                        starmed.SetActive(true);
                        break;
                    case 2:
                        starhigh.SetActive(true);
                        break;
                    default:
                        break;
                }
                switch (PlayerPrefs.GetInt("Settings: Texture"))
                {
                    case 0:
                        textureText.GetComponent<Text>().text = "Very Low";
                        break;
                    case 1:
                        textureText.GetComponent<Text>().text = "Low";
                        break;
                    case 2:
                        textureText.GetComponent<Text>().text = "Medium";
                        break;
                    case 3:
                        textureText.GetComponent<Text>().text = "High";
                        break;
                    default:
                        break;
                }
                switch (PlayerPrefs.GetInt("Settings: Windowed"))
                {

                    case 0:
                        resText.GetComponent<Text>().text = "Fullscreen";
                        break;
                    case 1:
                        resText.GetComponent<Text>().text = "Windowed";
                        break;
                    default:
                        break;
                }
                switch (PlayerPrefs.GetInt("Settings: Motion Blur"))
                {
                    case 0:
                        motionText.GetComponent<Text>().text = "Off";
                        break;
                    case 1:
                        motionText.GetComponent<Text>().text = "On";
                        break;
                    default:
                        break;
                }
                switch (PlayerPrefs.GetInt("Settings: Auto Exposure"))
                {
                    case 0:
                        exposureText.GetComponent<Text>().text = "Off";
                        break;
                    case 1:
                        exposureText.GetComponent<Text>().text = "On";
                        break;
                    default:
                        break;
                }
                switch (PlayerPrefs.GetInt("Settings: SSR"))
                {
                    case 0:
                        ssrText.GetComponent<Text>().text = "Off";
                        break;
                    case 1:
                        ssrText.GetComponent<Text>().text = "SSR Low";
                        break;
                    case 2:
                        ssrText.GetComponent<Text>().text = "SSR Medium";
                        break;
                    case 3:
                        ssrText.GetComponent<Text>().text = "SSR High";
                        break;
                    case 4:
                        ssrText.GetComponent<Text>().text = "SSR Ultra";
                        break;
                    case 5:
                        ssrText.GetComponent<Text>().text = "RT Performance";
                        break;
                    case 6:
                        ssrText.GetComponent<Text>().text = "RT Quality";
                        break;
                    default:
                        break;
                }
                switch (PlayerPrefs.GetInt("Settings: SSAO"))
                {
                    case 0:
                        ssaoText.GetComponent<Text>().text = "Off";
                        break;
                    case 1:
                        ssaoText.GetComponent<Text>().text = "SS AO";
                        break;
                    case 2:
                        ssaoText.GetComponent<Text>().text = "(WIP) SS GI";
                        break;
                    case 3:
                        ssaoText.GetComponent<Text>().text = "(WIP) RT AO";
                        break;
                    case 4:
                        ssaoText.GetComponent<Text>().text = "(WIP) RT GI Performance";
                        break;
                    case 5:
                        ssaoText.GetComponent<Text>().text = "(WIP) RT GI Quality";
                        break;
                    default:
                        break;
                }
                switch (PlayerPrefs.GetInt("Settings: Time of Day"))
                {
                    case 0:
                        timeofdayText.GetComponent<Text>().text = "PC Clock";
                        break;
                    case 1:
                        timeofdayText.GetComponent<Text>().text = "Day";
                        break;
                    case 2:
                        timeofdayText.GetComponent<Text>().text = "Night";
                        break;
                    case 3:
                        timeofdayText.GetComponent<Text>().text = "Rainy";
                        break;
                    case 4:
                        timeofdayText.GetComponent<Text>().text = "Sunset";
                        break;
                    default:
                        break;  
                }
                resPercentText.GetComponent<Text>().text = (100 - (PlayerPrefs.GetInt("Settings: Res Percent") * 5)).ToString();
                switch (PlayerPrefs.GetInt("Settings: DLSS"))
                {
                    case 0:
                        dlssText.GetComponent<Text>().text = "Off";
                        break;
                    case 1:
                        dlssText.GetComponent<Text>().text = "Ultra Performance";
                        break;
                    case 2:
                        dlssText.GetComponent<Text>().text = "Max Performance";
                        break;
                    case 3:
                        dlssText.GetComponent<Text>().text = "Balanced";
                        break;
                    case 4:
                        dlssText.GetComponent<Text>().text = "Quality";
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
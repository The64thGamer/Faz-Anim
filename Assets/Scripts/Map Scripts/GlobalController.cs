using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

public class GlobalController : MonoBehaviour
{
    [SerializeField]
    private Controller gamepad;
    public GameObject player;
    public VolumeProfile regularVolume;
    public bool TwoJoined = false;
    public bool debugJoinPlayerTwo;
    public bool debugResetProgress;
    public ShowObject[] showSceneNames;
    public PrizeStringGroups[] prizeStrings;
    public BuildBlockMats[] buildMaterials;
    public bool unlockAllPrizes = false;

    void OnEnable()
    {
        gamepad.Gamepad.Enable();
    }

    void OnDisable()
    {
        gamepad.Gamepad.Disable();
    }
    void Awake()
    {
        gamepad = new Controller();
        gamepad.Gamepad.Click.performed += ctx => JoinPlayerTwo();
        gamepad.Gamepad.Jump.performed += ctx => JoinPlayerTwo();
        gamepad.Gamepad.Flashlight.performed += ctx => JoinPlayerTwo();
        gamepad.Gamepad.Run.performed += ctx => JoinPlayerTwo();
        gamepad.Gamepad.Crouch.performed += ctx => JoinPlayerTwo();
        JoinPlayerOne();
        //Set Height
        player.transform.localScale = new Vector3(1.245614f, 1.245614f, 1.245614f);
    }


    private void FixedUpdate()
    {
        if (debugJoinPlayerTwo)
        {
            debugJoinPlayerTwo = false;
            JoinPlayerTwo();
        }
        if (debugResetProgress)
        {
            debugResetProgress = false;
            PlayerPrefs.SetInt("Item: Camera", 0);
            PlayerPrefs.SetInt("PrizeRoomLock", 0);
        }
    }

    void JoinPlayerOne()
    {
        if (GameObject.Find("Player") == null)
        {
            GameObject playernew = GameObject.Instantiate(player);
            playernew.name = "Player";
            DontDestroyOnLoad(playernew);
            Camera.main.GetComponent<Volume>().profile = regularVolume;
            if (GameVersion.isVR == "true")
            {
                QualitySettings.vSyncCount = 0;
                Screen.fullScreenMode = FullScreenMode.Windowed;
                playernew.GetComponent<Player>().isVR = true;
                Destroy(playernew.transform.Find("Main Camera").gameObject);
                playernew.transform.Find("VR").gameObject.SetActive(true);
                playernew.transform.Find("PlayerModel").gameObject.SetActive(false);
            }
            else
            {
                Destroy(playernew.transform.Find("VR").gameObject);
            }
            if (GameVersion.gameName == "Faz-Anim")
            {
                //Faz-Anim
                if (PlayerPrefs.GetInt("Tutorial Save 0") == 0 || !PlayerPrefs.HasKey("Tutorial Save 0"))
                {
                    playernew.transform.position = new Vector3(-14.11837f, -7.837707f, -15.63654f);
                    playernew.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 176.625f, 0.0f));
                }
                else
                {
                    playernew.transform.position = new Vector3(-0.7653216f, -0.7132773f, -1.245667f);
                }
            }
            else
            {
                //PTP
                playernew.transform.position = new Vector3(44.86588f, -1.078f, -4.248048f);
                playernew.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 283.725f, 0.0f));
            }
            GameObject.Find("Player").transform.Find("PlayerModel").Find("MainBody").GetComponent<CharPrefaber>().playerNum = 1;
            GameObject.Find("Player").transform.Find("PlayerModel").Find("MainBody").GetComponent<CharPrefaber>().LoadCharacterData();

            //Both
            ApplyCamSettings(GameObject.Find("Player"));
        }
    }

    public void JoinPlayerTwo()
    {
        if (!TwoJoined)
        {
            TwoJoined = true;

            //Player 2
            GameObject playernew = GameObject.Instantiate(player);
            playernew.name = "Player 2";
            playernew.transform.Find("PlayerModel").Find("MainBody").GetComponent<CharPrefaber>().playerNum = 2;
            playernew.transform.Find("PlayerModel").Find("MainBody").GetComponent<CharPrefaber>().LoadCharacterData();
            if (GameVersion.gameName == "Faz-Anim")
            {
                playernew.transform.position = new Vector3(0f, -0.7132773f, 0f);
            }
            else
            {
                playernew.transform.position = new Vector3(44.86588f, -1.078f, -4.248048f);
            }
            playernew.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 270.0f, 0.0f));
            DontDestroyOnLoad(playernew);
            playernew.GetComponent<Player>().controlType = Player.ControllerType.gamepad;
            playernew.transform.Find("Cursor").Find("Image").GetComponent<RectTransform>().anchoredPosition = new Vector2(500, 0);
            SetLayerRecursively(playernew.transform.Find("PlayerModel").gameObject, 7);
            playernew.transform.Find("Main Camera").GetComponent<Volume>().profile = regularVolume;
            Destroy(playernew.transform.Find("Main Camera").GetComponent<AudioListener>());
            Camera cam = playernew.transform.Find("Main Camera").GetComponent<Camera>();
            cam.cullingMask = cam.cullingMask & ~(1 << 7);
            cam.cullingMask = cam.cullingMask | (1 << 6);
            cam.rect = new Rect(0.5f, 0, 0.5f, 1);

            //Player 1
            GameObject playerold = GameObject.Find("Player");
            playerold.transform.Find("Main Camera").GetComponent<Camera>().rect = new Rect(0, 0, 0.5f, 1);
            playerold.transform.Find("Cursor").Find("Image").GetComponent<RectTransform>().anchoredPosition = new Vector2(-500, 0);
            playerold.transform.Find("PlayerModel").Find("MainBody").GetComponent<CharPrefaber>().playerNum = 1;
            playerold.transform.Find("PlayerModel").Find("MainBody").GetComponent<CharPrefaber>().LoadCharacterData();

            //Both
            ApplyCamSettings(GameObject.Find("Player"));
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public void LoadShowScene(int scene)
    {
        LoadShowScene(showSceneNames[scene].sceneName);
    }

    public void LoadShowScene(string scene)
    {
        for (int i = 0; i < showSceneNames.Length; i++)
        {
            if (SceneManager.GetSceneByName(showSceneNames[i].sceneName).IsValid())
            {
                SceneManager.UnloadSceneAsync(showSceneNames[i].sceneName);
                showSceneNames[i].objectShow.SetActive(true);
                showSceneNames[i].objectShow.GetComponentInChildren(typeof(Button3D), true).gameObject.SetActive(true);
            }
        }
        Resources.UnloadUnusedAssets();

        if (scene != "")
        {
            for (int i = 0; i < showSceneNames.Length; i++)
            {
                if (showSceneNames[i].sceneName == scene)
                {
                    showSceneNames[i].objectShow.GetComponentInChildren<Button3D>().gameObject.SetActive(false);
                    StartCoroutine(WaitingLoad(i));
                }
            }
        }
    }

    private IEnumerator WaitingLoad(int i)
    {
        yield return SceneManager.LoadSceneAsync(showSceneNames[i].sceneName, LoadSceneMode.Additive);
        showSceneNames[i].objectShow.SetActive(false);
    }

    public void LoadShowSceneALT(int scene)
    {
        LoadShowSceneALT(showSceneNames[scene].sceneName);
    }

    public void LoadShowSceneALT(string scene)
    {
        for (int i = 0; i < showSceneNames.Length; i++)
        {
            if (SceneManager.GetSceneByName(showSceneNames[i].sceneName).IsValid())
            {
                SceneManager.UnloadSceneAsync(showSceneNames[i].sceneName);
                showSceneNames[i].objectShow.SetActive(true);
                showSceneNames[i].objectShow.GetComponentInChildren(typeof(Button3D), true).gameObject.SetActive(true);
            }
        }
        Resources.UnloadUnusedAssets();

        if (scene != "")
        {
            for (int i = 0; i < showSceneNames.Length; i++)
            {
                if (showSceneNames[i].sceneName == scene)
                {
                    showSceneNames[i].objectShow.GetComponentInChildren<Button3D>().gameObject.SetActive(false);
                    StartCoroutine(WaitingLoadALT(i));
                }
            }
        }
    }

    private IEnumerator WaitingLoadALT(int i)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(showSceneNames[i].sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Scene scene = SceneManager.GetSceneByName(showSceneNames[i].sceneName);

        GameObject[] gg = scene.GetRootGameObjects();

        int gggg = 0;
        for (int e = 0; e < gg.Length; e++)
        {
            if (gg[e].transform.Find("Live Editor") != null)
            {
                gggg = e;
                gg[gggg].transform.position = new Vector3(0, -100, 0);
                gg[gggg].transform.Find("Live Editor").transform.position = showSceneNames[i].objectShow.transform.Find("Live Editor").transform.position;
                gg[gggg].transform.Find("Live Editor").transform.rotation = showSceneNames[i].objectShow.transform.Find("Live Editor").transform.rotation;
                gg[gggg].transform.Find("Show Selector").transform.position = showSceneNames[i].objectShow.transform.Find("Show Selector").transform.position;
                gg[gggg].transform.Find("Show Selector").transform.rotation = showSceneNames[i].objectShow.transform.Find("Show Selector").transform.rotation;
                break;
            }
        }
        showSceneNames[i].objectShow.SetActive(false);
    }


    public void ApplyCamSettings(GameObject player)
    {
        HDAdditionalCameraData camData = player.GetComponentInChildren<HDAdditionalCameraData>();
        FrameSettings frameSettings = camData.renderingPathCustomFrameSettings;
        FrameSettingsOverrideMask frameSettingsOverrideMask = camData.renderingPathCustomFrameSettingsOverrideMask;
        camData.customRenderingSettings = true;

        if (PlayerPrefs.GetInt("Settings: Motion Blur") == 0)
        {
            MotionBlur testDoF;
            player.GetComponentInChildren<Volume>().profile.TryGet<MotionBlur>(out testDoF);
            testDoF.intensity.value = 0;
        }
        if (PlayerPrefs.GetInt("Settings: Auto Exposure") == 0)
        {
            Exposure testDoF;
            player.GetComponentInChildren<Volume>().profile.TryGet<Exposure>(out testDoF);
            testDoF.mode.value = ExposureMode.Fixed;
            if (!(testDoF.limitMin.value == 1))
            {
                testDoF.fixedExposure.value = 0.71f;
            }
        }
        ScreenSpaceReflection ssr = null;
        player.GetComponentInChildren<Volume>().profile.TryGet<ScreenSpaceReflection>(out ssr);
        if (ssr != null)
        {
            switch (PlayerPrefs.GetInt("Settings: SSR"))
            {
                case 0:
                    camData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSR, false);
                    ssr.enabled.value = false;
                    ssr.tracing.value = RayCastingMode.RayMarching;
                    break;
                case 1:
                    ssr.enabled.value = true;
                    ssr.rayMaxIterations = 20;
                    ssr.fullResolution = false;
                    ssr.tracing.value = RayCastingMode.RayMarching;
                    break;
                case 2:
                    ssr.enabled.value = true;
                    ssr.rayMaxIterations = 32;
                    ssr.fullResolution = true;
                    ssr.tracing.value = RayCastingMode.RayMarching;
                    break;
                case 3:
                    ssr.enabled.value = true;
                    ssr.rayMaxIterations = 64;
                    ssr.fullResolution = true;
                    ssr.tracing.value = RayCastingMode.RayMarching;
                    break;
                case 4:
                    ssr.enabled.value = true;
                    ssr.fullResolution = true;
                    ssr.rayMaxIterations = 140;
                    ssr.tracing.value = RayCastingMode.RayMarching;
                    break;
                case 5:
                    ssr.enabled.value = true;
                    ssr.fullResolution = true;
                    ssr.rayMaxIterations = 140;
                    ssr.tracing.value = RayCastingMode.RayTracing;
                    ssr.mode.value = RayTracingMode.Performance;
                    break;
                case 6:
                    ssr.enabled.value = true;
                    ssr.fullResolution = true;
                    ssr.rayMaxIterations = 140;
                    ssr.tracing.value = RayCastingMode.RayTracing;
                    ssr.mode.value = RayTracingMode.Quality;
                    break;
                default:
                    break;
            }
        }
        AmbientOcclusion ssao = null;
        GlobalIllumination ssgi = null;
        player.GetComponentInChildren<Volume>().profile.TryGet<AmbientOcclusion>(out ssao);
        player.GetComponentInChildren<Volume>().profile.TryGet<GlobalIllumination>(out ssgi);
        if (ssao != null && ssgi != null)
        {
            switch (PlayerPrefs.GetInt("Settings: SSAO"))
            {
                case 0:
                    camData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSAO, false);
                    camData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSGI, false);
                    ssgi.enable.value = false;
                    ssao.active = false;
                    ssgi.tracing.value = RayCastingMode.RayMarching;
                    ssao.rayTracing.value = false;
                    break;
                case 1:
                    camData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSGI, false);
                    ssgi.enable.value = false;
                    ssao.active = true;
                    ssgi.tracing.value = RayCastingMode.RayMarching;
                    ssao.rayTracing.value = false;
                    break;
                case 2:
                    camData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSAO, false);
                    ssgi.enable.value = true;
                    ssao.active = false;
                    ssgi.tracing.value = RayCastingMode.RayMarching;
                    ssao.rayTracing.value = false;
                    break;
                case 3:
                    camData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSGI, false);
                    ssgi.enable.value = false;
                    ssao.active = true;
                    ssgi.tracing.value = RayCastingMode.RayMarching;
                    ssao.rayTracing.value = true;
                    break;
                case 4:
                    camData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSAO, false);
                    ssgi.enable.value = true;
                    ssao.active = false;
                    ssgi.tracing.value = RayCastingMode.RayTracing;
                    ssao.rayTracing.value = false;
                    ssgi.mode.value = RayTracingMode.Performance;
                    break;
                case 5:
                    camData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSAO, false);
                    ssgi.enable.value = true;
                    ssao.active = false;
                    ssgi.tracing.value = RayCastingMode.RayTracing;
                    ssgi.mode.value = RayTracingMode.Quality;
                    break;
                default:
                    break;
            }
        }
        //Res
        player.GetComponentInChildren<DynamicResCam>().currentScale = 100 - (PlayerPrefs.GetInt("Settings: Res Percent") * 5);
        player.GetComponentInChildren<DynamicResCam>().SetDynamicResolutionScale();

        //DLSS
        Debug.Log("DLSS " + PlayerPrefs.GetInt("Settings: DLSS"));
        switch (PlayerPrefs.GetInt("Settings: DLSS"))
        {
            case 0:
                camData.allowDeepLearningSuperSampling = false;
                break;
            case 1:
                camData.allowDeepLearningSuperSampling = true;
                camData.deepLearningSuperSamplingUseCustomQualitySettings = true;
                camData.deepLearningSuperSamplingQuality = 3;
                break;
            case 2:
                camData.allowDeepLearningSuperSampling = true;
                camData.deepLearningSuperSamplingUseCustomQualitySettings = true;
                camData.deepLearningSuperSamplingQuality = 0;
                break;
            case 3:
                camData.allowDeepLearningSuperSampling = true;
                camData.deepLearningSuperSamplingUseCustomQualitySettings = true;
                camData.deepLearningSuperSamplingQuality = 1;
                break;
            case 4:
                camData.allowDeepLearningSuperSampling = true;
                camData.deepLearningSuperSamplingUseCustomQualitySettings = true;
                camData.deepLearningSuperSamplingQuality = 2;
                break;
            default:
                break;
        }

        //Applying the frame setting mask back to the camera
        camData.renderingPathCustomFrameSettingsOverrideMask = frameSettingsOverrideMask;
    }

    public void AttemptAdvanceTutorial(string attemptString)
    {
        switch (attemptString)
        {
            case "TruckFade":

                break;
            case "TruckDone":
                player.transform.position = new Vector3(0.39633f, -0.53f, -2.731902f);
                player.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 362.0f, 0.0f));
                break;
            default:
                break;
        }
    }

}

[System.Serializable]
public class ShowObject
{
    public string sceneName;
    public GameObject objectShow;
}

[System.Serializable]
public class PrizeStrings
{
    public string name;
    public string description;
    public string price;
    public string[] skins;
    public string[] attributes;
}

[System.Serializable]
public class PrizeStringGroups
{
    public string groupName;
    public PrizeStrings[] prizeStrings;
}
[System.Serializable]
public class BuildBlockMats
{
    public string name;
    public Material mat;
}
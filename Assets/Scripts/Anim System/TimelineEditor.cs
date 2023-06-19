using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class TimelineEditor : MonoBehaviour
{
    public UI_ShowtapeManager uiShowtapeManager;
    public UI_SidePanel sidePanel;
    public WaveformVisualizer waveformVisualizer;
    public ShowtapeAnalyzer analyzer;
    public BitVisualization bitvis;
    public TimelineBitVis timelineBitVis;
    public Mack_Valves mackValves;
    public Dropdown file;
    public Dropdown edit;
    public Dropdown analysis;

    public AudioSource speaker;
    public VideoPlayer video;

    public RawImage[] views;

    public GameObject playbackMarker;

    public TLRecordGroup[] tlRecordGroup = new TLRecordGroup[300];

    public Text viewMinText;
    public Text viewMaxText;
    public Text currentTimeText;
    public Text timeScaleText;
    public Text fileName;
    public Text extraInfoText;

    public UI_WindowMaker windowMaker;
    public UI_PlayRecord playRecord;

    //Timeline Zoom View
    public float viewZoomMin = 0;
    public float viewZoomMax = 10;
    public float audioLengthMax = 0;
    float oldviewmin;
    float oldviewmax;
    public bool mouseDrag;
    float dragOldMin, dragOldMax, dragOldMouse;
    public int holderOffset;
    Vector2 screenSizeOld;
    int currentBitLineToEdit;
    int dropdownCooldown = 0;
    public GameObject CanvasExportBit;
    public Dropdown exportBitDropdown;

    //Camera Feeds
    GameObject cameraFeeds;
    public GameObject camFeedTemplate;
    public GameObject camFeedHolder;
    public GameObject editLineBox;
    public int currentCamFeed;


    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        screenSizeOld = new Vector2(Screen.width, Screen.height);
        for (int i = 0; i < tlRecordGroup.Length; i++)
        {
            tlRecordGroup[i].bit = i + 1;
        }
    }

    public void Update()
    {
        if(dropdownCooldown > 0)
        {
            dropdownCooldown--;
        }
        if (audioLengthMax != 0)
        {
            if (Input.GetMouseButtonUp(0))
            {
                StartCoroutine(removeEditBox());
            }
            //Keycodes
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PausePlay();
            }
            //DETELET DELETE DELETE THIS PLEASE DELETE THIS
            if (Input.GetKeyDown(KeyCode.A))
            {
                int arrayDestination = (int)(uiShowtapeManager.referenceSpeaker.time * uiShowtapeManager.dataStreamedFPS);

                Debug.Log(uiShowtapeManager.rshwData[arrayDestination].Get(0));
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
            {
                uiShowtapeManager.SaveRecording();
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
            {
                uiShowtapeManager.SaveRecordingAs();
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.O))
            {
                uiShowtapeManager.Load();
            }
            if (mouseDrag)
            {
                float dragPos = remap(0.5f + (dragOldMouse - (Input.mousePosition.x / Screen.width)), 0, 1, dragOldMin, dragOldMax);
                float length = (viewZoomMax - viewZoomMin) / 2.0f;
                if (dragPos - length >= 0 && dragPos + length <= audioLengthMax)
                {
                    viewZoomMin = dragPos - length;
                    viewZoomMax = dragPos + length;
                }
            }
            currentTimeText.text = System.TimeSpan.FromSeconds(speaker.time).ToString().TrimEnd(new Char[] { '0' }); ;
            viewZoomMax = Mathf.Clamp(viewZoomMax, viewZoomMin + 0.1f, audioLengthMax);
            viewZoomMin = Mathf.Clamp(viewZoomMin, 0, viewZoomMax - 0.1f);
            playbackMarker.transform.position = new Vector3((remap(speaker.time, viewZoomMin, viewZoomMax, 0, 1) * Screen.width), playbackMarker.transform.position.y, 0);
            if (uiShowtapeManager.playMovements && speaker.time >= (viewZoomMax + viewZoomMin) / 2.0f)
            {
                float length = (viewZoomMax - viewZoomMin) / 2.0f;
                if (speaker.time - length >= 0 && speaker.time + length <= audioLengthMax)
                {
                    viewZoomMin = speaker.time - length;
                    viewZoomMax = speaker.time + length;
                }
            }
            DoubleCheckViewBounds();
            if (!oldviewmax.Equals(viewZoomMax) || !viewZoomMin.Equals(oldviewmin))
            {
                RepaintTimeline();
            }
            oldviewmin = viewZoomMin;
            oldviewmax = viewZoomMax;

            //Check Screen Size change
            if(screenSizeOld != new Vector2(Screen.width,Screen.height))
            {
                RepaintTimeline();
                screenSizeOld = new Vector2(Screen.width, Screen.height);
            }
        }
    }

    IEnumerator removeEditBox()
    {
        yield return new WaitForSeconds(0.1f);
        editLineBox.SetActive(false);
    }

    public float remap(float val, float in1, float in2, float out1, float out2)
    {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }
    public void RepaintTimeline()
    {
        waveformVisualizer.PaintWaveformSpectrum(viewZoomMin, viewZoomMax, audioLengthMax);
        viewMinText.text = System.TimeSpan.FromSeconds(viewZoomMin).ToString().TrimEnd(new Char[] { '0' }); ;
        viewMaxText.text = System.TimeSpan.FromSeconds(viewZoomMax).ToString().TrimEnd(new Char[] { '0' }); ;
        timeScaleText.text = "[ " + (viewZoomMax - viewZoomMin).ToString() + "s ]";
        timelineBitVis.RepaintTimeline(viewZoomMin, viewZoomMax, audioLengthMax);
    }
    public void FileValueChanged()
    {
        if (dropdownCooldown == 0)
        {
            switch (file.value)
            {
                case 0:
                    uiShowtapeManager.AddWavSpecial();
                    uiShowtapeManager.SaveRecordingAs();
                    if(uiShowtapeManager.showtapeSegmentPaths != null)
                    {
                        if (uiShowtapeManager.showtapeSegmentPaths[0] != "")
                        {
                            if (uiShowtapeManager.referenceSpeaker != null)
                            {
                                uiShowtapeManager.referenceSpeaker.time = 0;
                            }
                            uiShowtapeManager.recordMovements = true;
                            uiShowtapeManager.currentShowtapeSegment = 0;
                            uiShowtapeManager.LoadFromURL(uiShowtapeManager.showtapeSegmentPaths[0]);
                        }
                    }
                    break;
                case 1:
                    uiShowtapeManager.Load();
                    break;
                case 2:
                    uiShowtapeManager.LoadMasterRandom();
                    break;
                case 3:
                    uiShowtapeManager.SaveRecording();
                    break;
                case 4:
                    uiShowtapeManager.SaveRecordingAs();
                    break;
                case 5:
                    SceneManager.LoadScene("Title Screen");
                    break;
                default:
                    break;
            }
        }
        dropdownCooldown = 30;
    }
    public void AnalyzeValueChanged()
    {
        if (dropdownCooldown == 0 && audioLengthMax != 0)
        {
            switch (analysis.value)
            {
                case 0:
                    analyzer.StartAnalysis("CompareTotalOn");
                    break;
                case 1:
                    analyzer.StartAnalysis("CompareLongestOn");
                    break;
                case 2:
                    analyzer.StartAnalysis("CompareTimesOn");
                    break;
                case 3:
                    analyzer.StartAnalysis("Extra Info");
                    break;
                case 4:
                    CanvasExportBit.SetActive(true);
                    List<Dropdown.OptionData> bits = new List<Dropdown.OptionData>();
                    for (int i = 1; i < 301; i++)
                    {
                        Dropdown.OptionData option = new Dropdown.OptionData();
                        option.text = windowMaker.SearchBitChartName(i);
                        bits.Add(option);
                    }
                    exportBitDropdown.options = bits;
                    break;
                case 5:
                    uiShowtapeManager.SetMasterFolder();
                    break;

                default:
                    break;
            }
        }
        dropdownCooldown = 30;
    }
    public void ExportBitWindowClose()
    {
        CanvasExportBit.SetActive(false);
    }

    public void ExportBitWindowValueChange()
    {
        analyzer.exportAllSignals = exportBitDropdown.value + 1;
        analyzer.StartAnalysis("ExportAllSignalsInBit");
        CanvasExportBit.SetActive(false);
    }

    public void DraggedTimeline(bool drag)
    {
        mouseDrag = drag;
        dragOldMax = viewZoomMax;
        dragOldMin = viewZoomMin;
        dragOldMouse = Input.mousePosition.x / Screen.width;
        DoubleCheckViewBounds();
    }

    public void EraseBitBox(int e)
    {
        e = e + holderOffset;
        int eraseLeft = (int)(remap(Input.mousePosition.x / Screen.width, 0, 1, viewZoomMin, viewZoomMax) * uiShowtapeManager.dataStreamedFPS);
        int eraseRight = eraseLeft + 1;
        while (eraseLeft > 0 && uiShowtapeManager.rshwData[eraseLeft].Get(e))
        {
            uiShowtapeManager.rshwData[eraseLeft].Set(e, false);
            eraseLeft--;
        }
        while (eraseRight < uiShowtapeManager.rshwData.Length && uiShowtapeManager.rshwData[eraseRight].Get(e))
        {
            uiShowtapeManager.rshwData[eraseRight].Set(e, false);
            eraseRight++;
        }
        RepaintTimeline();
    }

    public void ClickedTimeline()
    {
        speaker.time = remap(Input.mousePosition.x / Screen.width, 0, 1, viewZoomMin, viewZoomMax);
        Debug.Log("Clicked Timeline");
        syncTvsAndSpeakers();
        if (!uiShowtapeManager.playMovements)
        {
            StartCoroutine(AudioFeedback());
        }
    }

    IEnumerator AudioFeedback()
    {
        GameObject fake = new GameObject();
        fake.AddComponent<AudioSource>();
        AudioSource fakeSource = fake.GetComponent<AudioSource>();
        fakeSource.clip = speaker.clip;
        fakeSource.time = speaker.time;
        fakeSource.Play();
        yield return new WaitForSeconds(0.1f);
        Destroy(fake);
    }


    public void ZoomTimeline(float scroll)
    {
        float length = (viewZoomMax - viewZoomMin) / 2.0f;
        length = Mathf.Min(Mathf.Max(length - (scroll * 2), 1),10);
        if (((viewZoomMax + viewZoomMin) / 2.0f) - length >= 0)
        {
            viewZoomMin = ((viewZoomMax + viewZoomMin) / 2.0f) - length;
        }
        else
        {
            viewZoomMin = 0;
        }
        if (((viewZoomMax + viewZoomMin) / 2.0f) + length <= audioLengthMax)
        {
            viewZoomMax = ((viewZoomMax + viewZoomMin) / 2.0f) + length;
        }
        else
        {
            viewZoomMax = audioLengthMax;
        }
        DoubleCheckViewBounds();
    }

    void DoubleCheckViewBounds()
    {
        if(viewZoomMin < 0)
        {
            viewZoomMin = 0;
        }
        if(viewZoomMax > audioLengthMax)
        {
            viewZoomMax = audioLengthMax;
        }
    }
    public void ScrollBitGroups(float scroll)
    {
        if (audioLengthMax != 0)
        {
            if (scroll < 0)
            {
                holderOffset--;
            }
            else
            {
                holderOffset++;
            }
            holderOffset = Mathf.Clamp(holderOffset, 0, tlRecordGroup.Length);
            timelineBitVis.RepaintBitGroups();
            RepaintTimeline();
        }
    }

    public void BitLineRightClick(int which)
    {
        currentBitLineToEdit = which + holderOffset + 1;
        Debug.Log("Right Click Bit " + currentBitLineToEdit);
        editLineBox.SetActive(true);

        //Set fake mouse Cursor
        editLineBox.GetComponent<RectTransform>().position = Input.mousePosition;
    }

    public void BitLineErase(bool which)
    {
        Debug.Log("Bit Line Erase " + currentBitLineToEdit);
        uiShowtapeManager.DeleteMoveNoSaving(currentBitLineToEdit, which);
        RepaintTimeline();
    }

    public void BitLinePad(int howMuch)
    {
        Debug.Log("Bit Line Pad " + currentBitLineToEdit);
        uiShowtapeManager.PadMove(currentBitLineToEdit, howMuch);
        RepaintTimeline();
    }

    public void PitchForward()
    {
        if (!uiShowtapeManager.playMovements)
        {
            speaker.pitch = 0;
            uiShowtapeManager.Play(true, true);
        }
        switch (speaker.pitch)
        {
            case 0:
                speaker.pitch = 0.5f;
                break;
            case 0.5f:
                speaker.pitch = 1f;
                break;
            case 1:
                speaker.pitch = 2f;
                break;
            case 2:
                speaker.pitch = 5f;
                break;
            case 5:
                speaker.pitch = 10f;
                break;
            case 10:
                speaker.pitch = 100f;
                break;
            default:
                break;
        }
        video.playbackSpeed = speaker.pitch;
    }
    public void PausePlay()
    {
        speaker.pitch = 1;
        video.playbackSpeed = speaker.pitch;
        uiShowtapeManager.Play(false, false);
    }
    public void PitchBackward()
    {
        if (!uiShowtapeManager.playMovements)
        {
            speaker.pitch = 0;
            uiShowtapeManager.Play(true, true);
        }
        switch (speaker.pitch)
        {
            case 1:
                speaker.pitch = -1f;
                break;
            case 0:
                speaker.pitch = -0.5f;
                break;
            case -0.5f:
                speaker.pitch = -1f;
                break;
            case -1:
                speaker.pitch = -2f;
                break;
            case -2:
                speaker.pitch = -5f;
                break;
            case -5:
                speaker.pitch = -10f;
                break;
            case -10:
                speaker.pitch = -100f;
                break;
            default:
                break;
        }
        video.playbackSpeed = speaker.pitch;
    }

    public void AudioVideoGetData()
    {
        Debug.Log("Audio Video Get Data");
        audioLengthMax = uiShowtapeManager.speakerClip.length / uiShowtapeManager.speakerClip.channels;
        viewZoomMin = 0;
        viewZoomMax = 20;
        if (viewZoomMax > audioLengthMax)
        {
            viewZoomMax = audioLengthMax;
        }
        speaker.clip = uiShowtapeManager.speakerClip;
        speaker.Play();
        speaker.Pause();
        if (uiShowtapeManager.videoPath != "")
        {
            video.url = uiShowtapeManager.videoPath;
            video.Play();
            video.Pause();
        }
        waveformVisualizer.audioClip = uiShowtapeManager.speakerClip;
        waveformVisualizer.GetAudioSamples();
        waveformVisualizer.PaintWaveformSpectrum(viewZoomMin, viewZoomMax, audioLengthMax);
        fileName.text = Path.GetFileName(uiShowtapeManager.showtapeSegmentPaths[0]);
        extraInfoText.text = "Length: " + audioLengthMax + "s | 60fps Signal Density";
        StartCoroutine(CreateAndLinkScene(uiShowtapeManager.showtapeSegmentPaths[0].Substring(uiShowtapeManager.showtapeSegmentPaths[0].Length - 4, 4)));
    }

    public void setCurtains()
    {
        sidePanel.AutoCurtains(0);
    }

    public void setTechLights()
    {
        sidePanel.Upperlights(0);
    }

    IEnumerator CreateAndLinkScene(string url)
    {
        Debug.Log("Loading scene of " + url);

        Scene[] scenes = SceneManager.GetAllScenes();

        for (int i = 0; i < scenes.Length; i++)
        {
            if(scenes[i].name == "FNaF1")
            {
                SceneManager.UnloadSceneAsync("FNaF1");
            }
            if (scenes[i].name == "FNaF2")
            {
                SceneManager.UnloadSceneAsync("FNaF2");
            }
            if (scenes[i].name == "Studio C")
            {
                SceneManager.UnloadSceneAsync("Studio C");
            }
            if (scenes[i].name == "CYBERS")
            {
                SceneManager.UnloadSceneAsync("CYBERS");
            }
            if (scenes[i].name == "CRAE")
            {
                SceneManager.UnloadSceneAsync("CRAE");
            }
            if (scenes[i].name == "NRAE")
            {
                SceneManager.UnloadSceneAsync("NRAE");
            }
        }

        currentCamFeed = 0;

        if (GameVersion.gameName == "Faz-Anim")
        {
            switch (url)
            {
                case "fshw":
                    {
                        SceneManager.LoadScene("FNaF1", LoadSceneMode.Additive);
                        GameObject ui = null;
                        while (ui == null)
                        {
                            ui = GameObject.Find("FNaF1");
                            if (ui != null)
                            {
                                ui = ui.transform.Find("Show Selector").transform.Find("UI").gameObject;
                                windowMaker = ui.GetComponent<UI_WindowMaker>();
                                playRecord = ui.GetComponent<UI_PlayRecord>();
                                sidePanel = playRecord.sidePanel;
                                Mack_Valves mv = GameObject.Find("FNaF1").transform.Find("Mack Valves").gameObject.GetComponent<Mack_Valves>();
                                mackValves = mv;
                                uiShowtapeManager.mack = mv;
                                uiShowtapeManager.inputHandler = mv.gameObject.GetComponent<InputHandler>();
                                bitvis.mackvalves = mv;
                                cameraFeeds = GameObject.Find("FNaF1").transform.Find("Cameras").gameObject;
                                CamFeedCreate();
                            }
                            yield return null;
                        }
                        break;
                    }
                case "tshw":
                    {
                        SceneManager.LoadScene("FNaF2", LoadSceneMode.Additive);
                        GameObject ui = null;
                        while (ui == null)
                        {
                            ui = GameObject.Find("FNaF2");
                            if (ui != null)
                            {
                                ui = ui.transform.Find("Show Selector").transform.Find("UI").gameObject;
                                windowMaker = ui.GetComponent<UI_WindowMaker>();
                                playRecord = ui.GetComponent<UI_PlayRecord>();
                                sidePanel = playRecord.sidePanel;
                                Mack_Valves mv = GameObject.Find("FNaF2").transform.Find("Mack Valves").gameObject.GetComponent<Mack_Valves>();
                                mackValves = mv;
                                uiShowtapeManager.mack = mv;
                                uiShowtapeManager.inputHandler = mv.gameObject.GetComponent<InputHandler>();
                                bitvis.mackvalves = mv;
                                cameraFeeds = GameObject.Find("FNaF2").transform.Find("Cameras").gameObject;
                                CamFeedCreate();
                            }
                            yield return null;
                        }
                        break;
                    }
                case "mshw":
                    {
                        SceneManager.LoadScene("Toy Foxy", LoadSceneMode.Additive);
                        GameObject ui = null;
                        while (ui == null)
                        {
                            ui = GameObject.Find("MangleStage");
                            if (ui != null)
                            {
                                ui = ui.transform.Find("Show Selector").transform.Find("UI").gameObject;
                                windowMaker = ui.GetComponent<UI_WindowMaker>();
                                playRecord = ui.GetComponent<UI_PlayRecord>();
                                sidePanel = playRecord.sidePanel;
                                Mack_Valves mv = GameObject.Find("MangleStage").transform.Find("Mack Valves").gameObject.GetComponent<Mack_Valves>();
                                mackValves = mv;
                                uiShowtapeManager.mack = mv;
                                uiShowtapeManager.inputHandler = mv.gameObject.GetComponent<InputHandler>();
                                bitvis.mackvalves = mv;
                                cameraFeeds = GameObject.Find("MangleStage").transform.Find("Cameras").gameObject;
                                CamFeedCreate();
                            }
                            yield return null;
                        }
                        break;
                    }
                default:
                    break;
            }
        }
        else
        {
            switch (url)
            {
                case "sshw":
                    {
                        SceneManager.LoadScene("Studio C", LoadSceneMode.Additive); GameObject ui = null;
                        while (ui == null)
                        {
                            ui = GameObject.Find("Studio C");
                            if (ui != null)
                            {
                                ui = ui.transform.Find("Show Selector").transform.Find("UI").gameObject;
                                windowMaker = ui.GetComponent<UI_WindowMaker>();
                                playRecord = ui.GetComponent<UI_PlayRecord>();
                                sidePanel = playRecord.sidePanel;
                                Mack_Valves mv = GameObject.Find("Studio C").transform.Find("Mack Valves").gameObject.GetComponent<Mack_Valves>();
                                mackValves = mv;
                                uiShowtapeManager.mack = mv;
                                uiShowtapeManager.inputHandler = mv.gameObject.GetComponent<InputHandler>();
                                bitvis.mackvalves = mv;
                                cameraFeeds = GameObject.Find("Studio C").transform.Find("Cameras").gameObject;
                                CamFeedCreate();
                            }
                            yield return null;
                        }
                        break;
                    }
                case "rshw":
                    {
                        SceneManager.LoadScene("CRAE", LoadSceneMode.Additive);
                        GameObject ui = null;
                        while (ui == null)
                        {
                            ui = GameObject.Find("RFE");
                            if (ui != null)
                            {
                                ui = ui.transform.Find("Show Selector").transform.Find("UI").gameObject;
                                windowMaker = ui.GetComponent<UI_WindowMaker>();
                                playRecord = ui.GetComponent<UI_PlayRecord>();
                                sidePanel = playRecord.sidePanel;
                                Mack_Valves mv = GameObject.Find("RFE").transform.Find("Mack Valves").gameObject.GetComponent<Mack_Valves>();
                                mackValves = mv;
                                uiShowtapeManager.mack = mv;
                                uiShowtapeManager.inputHandler = mv.gameObject.GetComponent<InputHandler>();
                                bitvis.mackvalves = mv;
                                cameraFeeds = GameObject.Find("RFE").transform.Find("Cameras").gameObject;
                                CamFeedCreate();
                            }
                            yield return null;
                        }
                        break;
                    }
                case "cshw":
                    {
                        SceneManager.LoadScene("CYBERS", LoadSceneMode.Additive);
                        GameObject ui = null;
                        while (ui == null)
                        {
                            ui = GameObject.Find("Cyberamics");
                            if (ui != null)
                            {
                                ui = ui.transform.Find("Show Selector").transform.Find("UI").gameObject;
                                windowMaker = ui.GetComponent<UI_WindowMaker>();
                                playRecord = ui.GetComponent<UI_PlayRecord>();
                                sidePanel = playRecord.sidePanel;
                                Mack_Valves mv = GameObject.Find("Cyberamics").transform.Find("Mack Valves").gameObject.GetComponent<Mack_Valves>();
                                mackValves = mv;
                                uiShowtapeManager.mack = mv;
                                uiShowtapeManager.inputHandler = mv.gameObject.GetComponent<InputHandler>();
                                bitvis.mackvalves = mv;
                                cameraFeeds = GameObject.Find("Cyberamics").transform.Find("Cameras").gameObject;
                                CamFeedCreate();
                            }
                            yield return null;
                        }
                        break;
                    }
                case "nshw":
                    {
                        SceneManager.LoadScene("NRAE", LoadSceneMode.Additive);
                        GameObject ui = null;
                        while (ui == null)
                        {
                            ui = GameObject.Find("NRAE");
                            if(ui != null)
                            {
                                ui = ui.transform.Find("Show Selector").transform.Find("UI").gameObject;
                                windowMaker = ui.GetComponent<UI_WindowMaker>();
                                playRecord = ui.GetComponent<UI_PlayRecord>();
                                sidePanel = playRecord.sidePanel;
                                Mack_Valves mv = GameObject.Find("NRAE").transform.Find("Mack Valves").gameObject.GetComponent<Mack_Valves>();
                                mackValves = mv;
                                uiShowtapeManager.mack = mv;
                                uiShowtapeManager.inputHandler = mv.gameObject.GetComponent<InputHandler>();
                                bitvis.mackvalves = mv;
                                cameraFeeds = GameObject.Find("NRAE").transform.Find("Cameras").gameObject;
                                CamFeedCreate();
                            }
                            yield return null;
                        }
                        break;
                    }
                default:
                    break;
            }
        }
        RepaintTimeline();
        timelineBitVis.RepaintBitGroups();
        RepaintTimeline();
        uiShowtapeManager.inputHandler.valveMapping = -1;
        yield return null;
    }

    void CamFeedCreate()
    {
        foreach (Transform child in camFeedHolder.transform)
        {
            Destroy(child);
        }
        for (int i = 0; i < cameraFeeds.transform.childCount; i++)
        {
            GameObject temp = GameObject.Instantiate(camFeedTemplate,camFeedHolder.transform,true);
            RectTransform rect = temp.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + ((i % 2) * 50), rect.anchoredPosition.y - (40 * Mathf.FloorToInt(i/2.0f)));
            temp.name = i.ToString();
            temp.SetActive(true);
        }
    }
    public void audioVideoPlay()
    {
        Debug.Log("Audio Video Play");
        speaker.Play();
        if (uiShowtapeManager.videoPath != "")
        {
            video.Play();
            syncTvsAndSpeakers();
        }
    }
    public void audioVideoPause()
    {
        Debug.Log("Audio Video Pause");
        speaker.Pause();
        if (uiShowtapeManager.videoPath != "")
        {
            video.Pause();
            syncTvsAndSpeakers();
        }
    }

    public void syncTvsAndSpeakers()
    {
        if (uiShowtapeManager.videoPath != "")
        {
            video.time = speaker.time;
        }
    }

    public void SwapViews(int view)
    {
        for (int i = 0; i < views.Length; i++)
        {
            views[i].enabled = false;
        }
        views[view].enabled = true;
        if(view != 2 && cameraFeeds != null)
        {
            for (int i = 0; i < cameraFeeds.transform.childCount; i++)
            {
                cameraFeeds.transform.GetChild(i).gameObject.SetActive(false);
            }
        }    
    }

    public void ChangeCameraFeed(GameObject feedObj)
    {
        int feed = Int32.Parse(feedObj.name);
        if(cameraFeeds != null)
        {
            feed = Mathf.Min(feed, cameraFeeds.transform.childCount-1);
            for (int i = 0; i < cameraFeeds.transform.childCount; i++)
            {
                if(i == feed)
                {
                    cameraFeeds.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    cameraFeeds.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
}

[System.Serializable]
public class TLRecordGroup
{
    public int bit;

    //Editor
    [HideInInspector]
    public GameObject currentBit = null;
    [HideInInspector]
    public bool checkedObject = false;
}
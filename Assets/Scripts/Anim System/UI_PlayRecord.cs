using System.Collections.Generic;
using UnityEngine;
using SFB;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.Events;

public class UI_PlayRecord : MonoBehaviour
{
    //Stages
    [Header("Stage / Characters")]
    public StageSelector[] stages;
    [HideInInspector]
    public int currentStage = 0;

    //Characters
    public CharacterSelector[] characters;
    public FloatEvent characterEvent = new FloatEvent();
    public bool CharSwapCheck = false;
    [HideInInspector]
    public bool swap = false;
    [Space(20)]

    //Inspector Objects
    [Header("Inspector Objects")]
    public AudioSource[] speakerR;
    public AudioSource[] speakerL;
    public Sprite[] icons;
    public GameObject characterHolder;
    public GameObject mackValves;
    public GameObject playMultiText;
    public UI_SidePanel sidePanel;
    public Text ffSpeed;
    public Text AddSource;
    public Text Uncompress;
    public Text ticketText;
    [Space(20)]

    //Show Data
    [Header("Show Data")]
    Mack_Valves mack;
    InputHandler inputHandlercomp;
    [HideInInspector]
    public GameObject thePlayer;
    bool ticketCheck = false;
    bool ticketCheck2 = false;
    public SignalChange signalChange;
    public enum SignalChange
    {
        normal,
        PreCU,
        PrePTT,
    }

    public UI_ShowtapeManager manager;


    void Awake()
    {
        //Update Ticket Text
        UpdateTickets();

        //Initialize Objects
        thePlayer = GameObject.Find("Player");
        inputHandlercomp = mackValves.GetComponent<InputHandler>();
        mack = mackValves.GetComponent<Mack_Valves>();
        manager.inputHandler = inputHandlercomp;

        //Start up stages
        for (int i = 0; i < stages.Length; i++)
        {
            stages[i].Startup();
        }

        //Spawn in current Characters
        RecreateAllCharacters("");

        //Old code for special case in the original RR. Best kept for now.
        SwapCheck();
    }

    void Update()
    {
        //Advances the tutorial if it is active
        if (manager.recordMovements && manager.referenceSpeaker.clip != null)
        {
            if (manager.referenceSpeaker.time >= manager.referenceSpeaker.clip.length)
            {
                GameObject tt = GameObject.Find("Tutorial");
                if (tt != null)
                {
                    TutorialStart tut = tt.GetComponent<TutorialStart>();
                    tut.AttemptAdvanceTutorial("FinishShowtape");
                }

                SpecialSaveAs(11);
            }
        }

        //Run the Simulation
        UpdateAnims();
    }

    void UpdateAnims()
    {
        //A special case for swapping signals around in realtime through the Live Editor
        switch (signalChange)
        {
            case SignalChange.PreCU:
                bool g = mack.topDrawer[85];
                mack.topDrawer[85] = mack.topDrawer[80];
                mack.topDrawer[80] = false;
                mack.topDrawer[83] = g;
                mack.topDrawer[92] = mack.topDrawer[90];
                mack.topDrawer[93] = mack.topDrawer[91];
                mack.bottomDrawer[79] = mack.bottomDrawer[74];
                mack.bottomDrawer[90] = mack.bottomDrawer[74];
                mack.bottomDrawer[89] = false;
                mack.bottomDrawer[63] = true;
                break;
            case SignalChange.PrePTT:

                break;
            default:
                break;
        }

        //Update Portable Animatronics
        characterEvent.Invoke(Time.deltaTime * 60);

        //Update Lights
        for (int i = 0; i < stages[currentStage].lightValves.Length; i++)
        {
            stages[currentStage].lightValves[i].CreateMovements(Time.deltaTime * 60);
        }

        //Update Curtains
        if (stages[currentStage].curtainValves != null)
        {
            stages[currentStage].curtainValves.CreateMovements(Time.deltaTime * 60);
        }

        //Update Turntables
        for (int i = 0; i < stages[currentStage].tableValves.Length; i++)
        {
            stages[currentStage].tableValves[i].CreateMovements(Time.deltaTime * 60);
        }

        //Update AudioController
        if (stages[currentStage].texController != null)
        {
            stages[currentStage].texController.CreateTex();
        }

        //Update TV turn offs
        if (manager.videoPath != "")
        {
            for (int i = 0; i < stages[currentStage].tvs.Length; i++)
            {
                bool onoff = false;
                if (stages[currentStage].tvs[i].drawer)
                {
                    if (mack.bottomDrawer[stages[currentStage].tvs[i].bitOff])
                    {
                        onoff = true;
                    }
                }
                else
                {
                    if (mack.topDrawer[stages[currentStage].tvs[i].bitOff])
                    {
                        onoff = true;
                    }
                }


                for (int e = 0; e < stages[currentStage].tvs[i].tvs.Length; e++)
                {
                    if (onoff)
                    {
                        stages[currentStage].tvs[i].tvs[e].gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.black);
                    }
                    else
                    {
                        stages[currentStage].tvs[i].tvs[e].gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.white);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Switches which window is displayed on the main UI panel.
    /// </summary>
    /// <param name="thewindow"></param>
    public void SwitchWindow(int thewindow)
    {
        switch (thewindow)
        {
            case 0:
                //Title
                this.GetComponent<UI_WindowMaker>().MakeTitleWindow();
                manager.EraseShowtape();
                break;
            case 1:
                //Main Screen
                this.GetComponent<UI_WindowMaker>().MakeThreeWindow(icons[0], icons[1], icons[2], 0, 8, 2, 3, "Customize", "Play", "Record", 45);
                WindowSwitchDisable(true);
                manager.EraseShowtape();
                break;
            case 2:
                //Play Screen
                if (this.GetComponent<UI_WindowMaker>().allShowtapes.Length > 0)
                {
                    this.GetComponent<UI_WindowMaker>().MakeThreeWindow(icons[4], icons[1], icons[10], 1, 7, 6, 32, "Play Showtape", "Play Segment", "Show List", 37);
                }
                else
                {
                    this.GetComponent<UI_WindowMaker>().MakeTwoWindow(icons[4], icons[1], 1, 7, 6, "Play Showtape", "Play Segment", 37);
                }
                manager.EraseShowtape();
                WindowSwitchDisable(true);

                break;
            case 3:
                //Record Screen
                this.GetComponent<UI_WindowMaker>().MakeTwoWindow(icons[2], icons[3], 1, 5, 4, "New Recording", "Edit Recording", 37);
                WindowSwitchDisable(true);
                manager.EraseShowtape();
                break;
            case 4:
                //Edit Recording Screen
                this.GetComponent<UI_WindowMaker>().MakeTwoWindow(icons[3], icons[5], 3, 34, 21, "Edit Segment", "Add to Segment", 30);
                WindowSwitchDisable(true);
                manager.EraseShowtape();
                break;
            case 5:
                //New Recording Screen
                this.GetComponent<UI_WindowMaker>().MakeNewRecordWindow();
                manager.EraseShowtape();
                break;
            case 6:
                //Player Menu (Single)
                manager.Load();
                if (manager.rshwData != null)
                {
                    this.GetComponent<UI_WindowMaker>().MakePlayWindow(false);
                }
                break;
            case 7:
                //Player Menu (Folder)
                manager.LoadFolder();
                if (manager.rshwData != null)
                {
                    this.GetComponent<UI_WindowMaker>().MakePlayWindow(false);
                }
                break;
            case 8:
                //Customize Screen
                this.GetComponent<UI_WindowMaker>().MakeTwoWindow(icons[8], icons[9], 1, 16, 9, "Edit Stage", "Edit Characters", 35);
                break;
            case 9:
                //Edit Character Screen
                this.GetComponent<UI_WindowMaker>().MakeCharacterCustomizeIconsWindow(characters);
                break;
            case 11:
                //Recording Groups Screen (Or New Recording Screen)
                this.GetComponent<UI_WindowMaker>().MakeRecordIconsWindow();
                WindowSwitchDisable(false);
                manager.EraseShowtape();
                break;
            case 16:
                //Stage Customize Menu
                StageCustomMenu();
                break;
            case 17:
                if (manager.rshwData != null)
                {
                    this.GetComponent<UI_WindowMaker>().MakePlayWindow(false);
                }
                break;
            case 21:
                //Recording Groups Screen (Standalone)
                this.GetComponent<UI_WindowMaker>().MakeRecordIconsWindow();
                WindowSwitchDisable(false);
                manager.EraseShowtape();
                break;
            case 22:
                //Delete Movement Screen 1
                this.GetComponent<UI_WindowMaker>().MakeDeleteMoveMenu(0);
                break;
            case 23:
                //Delete Movement Back 1
                this.GetComponent<UI_WindowMaker>().MakeDeleteMoveMenu(-1);
                break;
            case 24:
                //Delete Movement Forward 1
                this.GetComponent<UI_WindowMaker>().MakeDeleteMoveMenu(1);
                break;
            case 28:
                //Showtape Window 0
                this.GetComponent<UI_WindowMaker>().MakeShowtapeWindow(0);
                break;
            case 29:
                //Segment Window 1
                this.GetComponent<UI_WindowMaker>().MakeSegmentWindow(1);
                break;
            case 30:
                //Segment Window -1
                this.GetComponent<UI_WindowMaker>().MakeSegmentWindow(-1);
                break;
            case 31:
                //Showtape Help Window
                this.GetComponent<UI_WindowMaker>().MakeShowtapeHelpWindow(0);
                break;
            case 32:
                //Showtape Window 1
                this.GetComponent<UI_WindowMaker>().MakeShowtapeWindow(1);
                break;
            case 33:
                //Showtape Window -1
                this.GetComponent<UI_WindowMaker>().MakeShowtapeWindow(-1);
                break;
            case 34:
                this.GetComponent<UI_WindowMaker>().MakeTwoWindow(icons[6], icons[5], 4, 22, 35, "Delete Bits", "Replace Audio", 30);
                break;
            case 35:
                manager.ReplaceShowAudio();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Starts new show.
    /// </summary>
    /// <param name="input"></param>
    public void StartNewShow(int input)
    {
        Debug.Log("Starting New Show");
        manager.Load();
        if (manager.speakerClip != null)
        {
            SwitchWindow(input);
        }
    }

    /// <summary>
    /// Load a customize window for a particular character.
    /// </summary>
    /// <param name="input"></param>
    public void CharacterCustomMenu(int input)
    {
        this.GetComponent<UI_WindowMaker>().MakeCharacterCustomizeWindow(characters, input);
    }

    /// <summary>
    /// Load a customize stage window for a particular stage.
    /// </summary>
    public void StageCustomMenu()
    {
        this.GetComponent<UI_WindowMaker>().MakeStageCustomizeWindow(stages, currentStage);
    }

    /// <summary>
    /// Index up the current costume of a character. Costume 0 is no character on stage.
    /// </summary>
    /// <param name="input"></param>
    public void CostumeUp(int input)
    {
        if (characters[input].currentCostume > -1)
        {
            characters[input].currentCostume--;
            RecreateAllCharacters(characters[input].characterName);
            this.GetComponent<UI_WindowMaker>().MakeCharacterCustomizeWindow(characters, input);
        }

    }

    /// <summary>
    /// Index down the current costume of a character.
    /// </summary>
    /// <param name="input"></param>
    public void CostumeDown(int input)
    {
        if (characters[input].currentCostume < characters[input].allCostumes.Length - 1)
        {
            characters[input].currentCostume++;
            RecreateAllCharacters(characters[input].characterName);
            this.GetComponent<UI_WindowMaker>().MakeCharacterCustomizeWindow(characters, input);
        }
    }

    /// <summary>
    /// Index up the current stage presented.
    /// </summary>
    /// <param name="input"></param>
    public void StageUp(int input)
    {
        if (input > 0)
        {
            currentStage--;
            for (int i = 0; i < stages.Length; i++)
            {
                if (i != currentStage)
                {
                    if (stages[i].stage.activeSelf)
                    {
                        stages[i].stage.SetActive(false);
                    }
                }
                else
                {
                    if (!stages[i].stage.activeSelf)
                    {
                        stages[i].stage.SetActive(true);
                    }
                }
            }
            RecreateAllCharacters("");
            SwapCheck();
            this.GetComponent<UI_WindowMaker>().MakeStageCustomizeWindow(stages, currentStage);
        }
    }

    /// <summary>
    /// Index down the current stage presented.
    /// </summary>
    /// <param name="input"></param>
    public void StageDown(int input)
    {
        if (input < stages.Length - 1)
        {
            currentStage++;
            for (int i = 0; i < stages.Length; i++)
            {
                if (i != currentStage)
                {
                    if (stages[i].stage.activeSelf)
                    {
                        stages[i].stage.SetActive(false);
                    }
                }
                else
                {
                    if (!stages[i].stage.activeSelf)
                    {
                        stages[i].stage.SetActive(true);
                    }
                }
            }
            RecreateAllCharacters("");
            SwapCheck();
            this.GetComponent<UI_WindowMaker>().MakeStageCustomizeWindow(stages, currentStage);
        }
    }

    public void RecreateAllCharacters(string singleCharacter)
    {
        if (singleCharacter == "")
        {
            //Destroy current Characters
            foreach (Transform child in characterHolder.transform)
            {
                Destroy(child.gameObject);
            }

            //Create current Characters
            int g = 0;
            for (int i = 0; i < stages[currentStage].stageCharacters.Length; i++)
            {
                for (int e = 0; e < characters.Length; e++)
                {
                    if (stages[currentStage].stageCharacters[i].characterName == characters[e].characterName)
                    {
                        if (characters[e].currentCostume != -1)
                        {
                            GameObject newChar = GameObject.Instantiate(characters[e].mainCharacter);
                            newChar.name = characters[e].characterName;
                            newChar.transform.parent = characterHolder.transform;
                            newChar.transform.localPosition = stages[currentStage].stageCharacters[i].characterPos + characters[e].allCostumes[characters[e].currentCostume].offsetPos;
                            newChar.transform.rotation = Quaternion.Euler(stages[currentStage].stageCharacters[i].characterRot);
                            newChar.transform.GetChild(0).GetComponent<Character_Valves>().mackValves = mackValves;
                            newChar.transform.GetChild(0).GetComponent<Character_Valves>().StartUp();
                            g++;

                            //Delete other costumes
                            foreach (Transform mesh in newChar.transform.GetChild(0).transform)
                            {
                                if (!(mesh.gameObject.name == characters[e].allCostumes[characters[e].currentCostume].costumeName) && mesh.gameObject.name != "Armature")
                                {
                                    Destroy(mesh.gameObject);
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            //Destroy Character
            foreach (Transform child in characterHolder.transform)
            {
                if (child.name == singleCharacter)
                {
                    Destroy(child.gameObject);
                }
            }

            //Create Character
            for (int e = 0; e < characters.Length; e++)
            {
                if (singleCharacter == characters[e].characterName)
                {
                    //Check for multiple of single character
                    bool[] count = new bool[stages[currentStage].stageCharacters.Length];
                    for (int i = 0; i < stages[currentStage].stageCharacters.Length; i++)
                    {
                        if (stages[currentStage].stageCharacters[i].characterName == singleCharacter)
                        {
                            count[i] = true;
                        }
                    }

                    for (int g = 0; g < count.Length; g++)
                    {
                        if (characters[e].currentCostume != -1 && count[g] == true)
                        {
                            GameObject newChar = GameObject.Instantiate(characters[e].mainCharacter);

                            newChar.name = characters[e].characterName;
                            newChar.transform.parent = characterHolder.transform;
                            newChar.transform.GetChild(0).GetComponent<Character_Valves>().mackValves = mackValves;
                            newChar.transform.localPosition = stages[currentStage].stageCharacters[g].characterPos;
                            newChar.transform.rotation = Quaternion.Euler(stages[currentStage].stageCharacters[g].characterRot);
                            newChar.transform.GetChild(0).GetComponent<Character_Valves>().StartUp();
                            //Delete other costumes
                            foreach (Transform mesh in newChar.transform.GetChild(0).transform)
                            {
                                if (!(mesh.gameObject.name == characters[e].allCostumes[characters[e].currentCostume].costumeName) && mesh.gameObject.name != "Armature")
                                {
                                    Destroy(mesh.gameObject);
                                }
                            }
                        }
                    }
                }
            }
        }
        sidePanel.FlowLoad(-1);
    }

    /// <summary>
    /// Old RR engine code that would automatically swap Rolfe and Klunk out.
    /// This was to prevent either character being on stage at the same time,
    /// or to have duplcate stages for each. The swapped character was still
    /// present, but would be below the ground by 100 meters.
    /// </summary>
    public void SwapCheck()
    {
        if (CharSwapCheck == true)
        {
            //Rolfe Klunk Swap
            if (swap)
            {
                for (int u = 0; u < characters.Length; u++)
                {
                    if (characters[u].characterName == "Rolfe & Earl")
                    {
                        characters[u].mainCharacter.transform.localPosition = new Vector3(0, -100, 0);
                    }
                    if (characters[u].characterName == "Klunk")
                    {
                        for (int i = 0; i < stages[currentStage].stageCharacters.Length; i++)
                        {
                            if (stages[currentStage].stageCharacters[i].characterName == characters[u].characterName)
                            {
                                characters[u].mainCharacter.transform.localPosition = stages[currentStage].stageCharacters[i].characterPos;
                                characters[u].mainCharacter.transform.rotation = Quaternion.Euler(stages[currentStage].stageCharacters[i].characterRot);
                            }
                        }
                    }
                }
            }
            else
            {
                for (int u = 0; u < characters.Length; u++)
                {
                    if (characters[u].characterName == "Rolfe & Earl")
                    {
                        for (int i = 0; i < stages[currentStage].stageCharacters.Length; i++)
                        {
                            if (stages[currentStage].stageCharacters[i].characterName == characters[u].characterName)
                            {
                                characters[u].mainCharacter.transform.localPosition = stages[currentStage].stageCharacters[i].characterPos;
                                characters[u].mainCharacter.transform.rotation = Quaternion.Euler(stages[currentStage].stageCharacters[i].characterRot);
                            }
                        }
                    }
                    if (characters[u].characterName == "Klunk")
                    {
                        characters[u].mainCharacter.transform.localPosition = new Vector3(0, -100, 0);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Open a window for the current movment group to be used.
    /// </summary>
    /// <param name="input"></param>
    public void RecordingGroupMenu(int input)
    {
        this.GetComponent<UI_WindowMaker>().MakeMoveTestWindow(input);
    }

    /// <summary>
    /// Stops the recording during a window switch.
    /// </summary>
    /// <param name="curtainStop"></param>
    void WindowSwitchDisable(bool curtainStop)
    {
        manager.recordMovements = false;
        inputHandlercomp.valveMapping = 0;
    }

    /// <summary>
    /// For the "OFF" button on the UI panel. Unloads the current stage and simulation.
    /// </summary>
    /// <param name="input"></param>
    public void UnloadScene(int input)
    {
        GameObject.Find("Global Controller").GetComponent<GlobalController>().LoadShowScene("");
    }

    /// <summary>
    /// Loads audio and video from the showtape manager into the stage speakers.
    /// </summary>
    public void loadAudio()
    {
        manager.referenceSpeaker.clip = manager.speakerClip;
        for (int i = 0; i < speakerL.Length; i++)
        {
            speakerL[i].clip = manager.speakerClip;
        }
        for (int i = 0; i < speakerR.Length; i++)
        {
            speakerR[i].clip = manager.speakerClip;
        }
        if (manager.videoPath != "")
        {
            for (int i = 0; i < stages[currentStage].tvs.Length; i++)
            {
                for (int e = 0; e < stages[currentStage].tvs[i].tvs.Length; e++)
                {
                    stages[currentStage].tvs[i].tvs[e].url = manager.videoPath;
                    stages[currentStage].tvs[i].tvs[e].Play();
                    stages[currentStage].tvs[i].tvs[e].Pause();
                }
            }
        }
        manager.Play(true, true);
        syncAudio();
        SwitchWindow(17);
        playMultiText.GetComponent<PlayMenuManager>().TextUpdate(false);
    }

    /// <summary>
    /// Ensures audio and video is synced when the showtape is playing.
    /// </summary>
    public void syncAudio()
    {
        if (manager.videoPath != "")
        {
            for (int i = 0; i < stages[currentStage].tvs.Length; i++)
            {
                for (int e = 0; e < stages[currentStage].tvs[i].tvs.Length; e++)
                {
                    stages[currentStage].tvs[i].tvs[e].time = manager.referenceSpeaker.time;
                }
            }
        }
        for (int i = 0; i < speakerL.Length; i++)
        {
            speakerL[i].time = manager.referenceSpeaker.time;
        }
        for (int i = 0; i < speakerR.Length; i++)
        {
            speakerR[i].time = manager.referenceSpeaker.time;
        }
    }

    /// <summary>
    /// Stops the showtape.
    /// </summary>
    public void Stop()
    {
        manager.referenceSpeaker.time = 0;
        manager.Play(true, false);
        SwitchWindow(2);
    }

    /// <summary>
    /// Pauses or unpauses the showtape.
    /// </summary>
    public void pauseSong()
    {
        if (manager.referenceSpeaker.isPlaying)
        {
            AVPause();
        }
        else
        {
            AVPlay();
        }
    }

    /// <summary>
    /// Saves a showtape while returning to the Create Recording menu.
    /// This is called when creating a new showtape.
    /// </summary>
    /// <param name="input"></param>
    public void SpecialSaveAs(int input)
    {
        if (manager.SaveRecordingAs())
        {
            SwitchWindow(input);
            TutorialStart tut = UnityEngine.Object.FindObjectOfType<TutorialStart>();
            if (tut != null)
            {
                Debug.Log("tut advance");
                tut.AttemptAdvanceTutorial("Create WAV");
            }
        }
        if (input == 11)
        {
            SwitchWindow(input);
        }
    }

    /// <summary>
    /// Pauses audio and video.
    /// </summary>
    public void AVPause()
    {
        Debug.Log("Audio Video Pause");
        if (manager.videoPath != "")
        {
            for (int i = 0; i < stages[currentStage].tvs.Length; i++)
            {
                for (int e = 0; e < stages[currentStage].tvs[i].tvs.Length; e++)
                {
                    stages[currentStage].tvs[i].tvs[e].Pause();
                }
            }
        }
        manager.referenceSpeaker.Pause();
        for (int i = 0; i < speakerL.Length; i++)
        {
            speakerL[i].Pause();
        }
        for (int i = 0; i < speakerR.Length; i++)
        {
            speakerR[i].Pause();
        }
        syncAudio();
    }

    /// <summary>
    /// Plays audio and video.
    /// </summary>
    public void AVPlay()
    {
        Debug.Log("Audio Video Pause");
        if (manager.videoPath != "")
        {
            for (int i = 0; i < stages[currentStage].tvs.Length; i++)
            {
                for (int e = 0; e < stages[currentStage].tvs[i].tvs.Length; e++)
                {
                    stages[currentStage].tvs[i].tvs[e].Play();
                }
            }
        }
        manager.referenceSpeaker.Play();
        for (int i = 0; i < speakerL.Length; i++)
        {
            speakerL[i].Play();
        }
        for (int i = 0; i < speakerR.Length; i++)
        {
            speakerR[i].Play();
        }
        syncAudio();
    }

    /// <summary>
    /// Increases the speed of audio and video.
    /// </summary>
    /// <param name="input"></param>
    public void FFSong(int input)
    {
        if (input == -1)
        {
            PitchBackward();
        }
        else if (input == 0)
        {
            manager.referenceSpeaker.pitch = 1;
        }
        else
        {
            PitchForward();
        }
        for (int i = 0; i < speakerL.Length; i++)
        {
            speakerL[i].pitch = manager.referenceSpeaker.pitch;
        }
        for (int i = 0; i < speakerR.Length; i++)
        {
            speakerR[i].pitch = manager.referenceSpeaker.pitch;
        }
        if (manager.videoPath != "")
        {
            for (int i = 0; i < stages[currentStage].tvs.Length; i++)
            {
                for (int e = 0; e < stages[currentStage].tvs[i].tvs.Length; e++)
                {
                    stages[currentStage].tvs[i].tvs[e].playbackSpeed = manager.referenceSpeaker.pitch;
                }
            }
        }
        syncAudio();
    }

    /// <summary>
    /// Pitches forward the audio and video by one setting.
    /// </summary>
    public void PitchForward()
    {
        if (!manager.playMovements)
        {
            manager.referenceSpeaker.pitch = 0;
            manager.Play(true, true);
        }
        switch (manager.referenceSpeaker.pitch)
        {
            case -10:
                manager.referenceSpeaker.pitch = 0.5f;
                break;
            case -5:
                manager.referenceSpeaker.pitch = 0.5f;
                break;
            case -2:
                manager.referenceSpeaker.pitch = 0.5f;
                break;
            case -1:
                manager.referenceSpeaker.pitch = 0.5f;
                break;
            case -0.5f:
                manager.referenceSpeaker.pitch = 0.5f;
                break;
            case 0:
                manager.referenceSpeaker.pitch = 0.5f;
                break;
            case 0.5f:
                manager.referenceSpeaker.pitch = 1f;
                break;
            case 1:
                manager.referenceSpeaker.pitch = 2f;
                break;
            case 2:
                manager.referenceSpeaker.pitch = 5f;
                break;
            case 5:
                manager.referenceSpeaker.pitch = 10f;
                break;
            case 10:
                manager.referenceSpeaker.pitch = 100f;
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// Pitches backward the audio and video by one setting.
    /// </summary>
    public void PitchBackward()
    {
        if (!manager.playMovements)
        {
            manager.referenceSpeaker.pitch = 0;
            manager.Play(true, true);
        }
        switch (manager.referenceSpeaker.pitch)
        {
            case 1:
                manager.referenceSpeaker.pitch = 0.5f;
                break;
            case 0.5f:
                manager.referenceSpeaker.pitch = -0.5f;
                break;
            case 0:
                manager.referenceSpeaker.pitch = -0.5f;
                break;
            case -0.5f:
                manager.referenceSpeaker.pitch = -1f;
                break;
            case -1:
                manager.referenceSpeaker.pitch = -2f;
                break;
            case -2:
                manager.referenceSpeaker.pitch = -5f;
                break;
            case -5:
                manager.referenceSpeaker.pitch = -10f;
                break;
            case -10:
                manager.referenceSpeaker.pitch = -100f;
                break;
            default:
                break;
        }
        if (manager.videoPath != "")
        {
            for (int i = 0; i < stages[currentStage].tvs.Length; i++)
            {
                for (int e = 0; e < stages[currentStage].tvs[i].tvs.Length; e++)
                {
                    stages[currentStage].tvs[i].tvs[e].playbackSpeed = manager.referenceSpeaker.pitch;
                }
            }
        }
    }

    /// <summary>
    /// Updates Ticket text with the currently saved ticket count.
    /// </summary>
    public void UpdateTickets()
    {
        ticketText.text = "x" + PlayerPrefs.GetInt("TicketCount").ToString();
    }
}
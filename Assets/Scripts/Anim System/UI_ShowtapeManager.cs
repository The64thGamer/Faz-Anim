using System.Collections.Generic;
using UnityEngine;
using SFB;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class FloatEvent : UnityEvent<float> { }

public class UI_ShowtapeManager : MonoBehaviour
{
    //Inspector Objects
    [Header("Inspector Objects")]
    public Mack_Valves mack;
    public InputHandler inputHandler;
    [Space(20)]

    //File Show MetaData
    [Header("File Show Metadata")]
    [HideInInspector] public BitArray[] rshwData;
    public AudioSource referenceSpeaker;
    public AudioClip speakerClip;
    public LoopVers songLoopSetting;
    public string wavPath;
    public string videoPath;
    public string fileExtention = "rshw";
    public string[] showtapeSegmentPaths = new string[1];
    public int currentShowtapeSegment = -1;
    public int dataStreamedFPS = 60;
    [Space(20)]

    //Events
    public UnityEvent audioVideoPlay;
    public UnityEvent audioVideoPause;
    public UnityEvent audioVideoGetData;
    public UnityEvent newDataRecorded;
    public UnityEvent curtainClose;
    public UnityEvent curtainOpen;
    public UnityEvent syncTvsAndSpeakers;
    public UnityEvent updateTickets;

    //Extra Variables
    bool ticketCheck;
    bool ticketCheck2;
    bool disableCharactersOnStart = true;

    //Sim States
    public bool recordMovements = false;
    public bool playMovements = false;
    public bool isRandomPlaybackOn;

    //New Simulation
    float timeSongStarted = 0;
    float timeSongOffset = 0;
    float timePauseStart = 0;
    float timeInputSpeedStart = 0;
    int previousFramePosition = 0;
    bool previousAnyButtonHeld = false;

    //Sync TV for Large Shows (Unity's Fault)
    float syncTimer;

    public enum LoopVers
    {
        noLoop,
        loopPlaylist,
        loopSong
    }

    void Update()
    {

        //Big Show Sync
        syncTimer += Time.deltaTime;
        if (syncTimer >= 30)
        {
            syncTimer = 0;
            syncTvsAndSpeakers.Invoke();
        }

        if(inputHandler != null)
        {
        InputDataObj inputDataObj = inputHandler.InputCheck();

        //Clear Drawers
        mack.topDrawer = inputDataObj.topDrawer;
        mack.bottomDrawer = inputDataObj.bottomDrawer;

            //Check for inputs and send to mack valves
            if (inputHandler != null && mack != null && referenceSpeaker.clip != null)
            {


                if (rshwData != null)
                {
                    //Show Code
                    //Being paused means the same frame of data will loop
                    //Being unpaused means deciding where to start next sim frame
                    int arrayDestination = (int)(referenceSpeaker.time * dataStreamedFPS);

                    //Check if new frames need to be created
                    if (arrayDestination >= rshwData.Length && rshwData.Length != 0)
                    {
                        if (recordMovements)
                        {
                            while (arrayDestination + 1 > rshwData.Length)
                            {
                                rshwData = rshwData.Append(new BitArray(300)).ToArray();
                            }
                        }
                        else
                        {
                            arrayDestination = rshwData.Length;
                        }
                    }

                    //Record
                    if (recordMovements)
                    {
                        //Record
                        if (inputDataObj.anyButtonHeld)
                        {
                            for (int i = 0; i < 150; i++)
                            {
                                if (inputDataObj.topDrawer[i])
                                {
                                    rshwData[arrayDestination].Set(i, true);
                                }
                                if (inputDataObj.bottomDrawer[i])
                                {
                                    rshwData[arrayDestination].Set(i + 150, true);
                                }
                            }
                            if (previousAnyButtonHeld)
                            {
                                //Record forward or backward
                                if (previousFramePosition <= arrayDestination)
                                {
                                    //Forward
                                    for (int i = 0; i < arrayDestination - previousFramePosition; i++)
                                    {
                                        for (int e = 0; e < 150; e++)
                                        {
                                            if (inputDataObj.topDrawer[e])
                                            {
                                                rshwData[previousFramePosition + i].Set(e, true);
                                            }
                                            if (inputDataObj.bottomDrawer[e])
                                            {
                                                rshwData[previousFramePosition + i].Set(e + 150, true);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //Backward
                                    for (int i = 0; i < previousFramePosition - arrayDestination; i++)
                                    {
                                        for (int e = 0; e < 150; e++)
                                        {
                                            if (inputDataObj.topDrawer[e])
                                            {
                                                rshwData[previousFramePosition - i].Set(e, true);
                                            }
                                            if (inputDataObj.bottomDrawer[e])
                                            {
                                                rshwData[previousFramePosition - i].Set(e + 150, true);
                                            }
                                        }
                                    }
                                }
                            }
                            newDataRecorded.Invoke();
                        }



                        //Ticket Earning Code
                        if (Input.anyKey)
                        {
                            ticketCheck = true;
                        }
                        if (referenceSpeaker.time % 5 < 1 && !ticketCheck2)
                        {
                            ticketCheck2 = true;
                            if (Input.anyKey)
                            {
                                ticketCheck = false;
                            }
                            if (ticketCheck)
                            {
                                PlayerPrefs.SetInt("TicketCount", PlayerPrefs.GetInt("TicketCount") + 1);
                                updateTickets.Invoke();
                            }
                            ticketCheck = false;
                        }
                        if (referenceSpeaker.time % 5 >= 1)
                        {
                            ticketCheck2 = false;
                        }
                    }


                    //Apply the current frame of simulation data to the Mack Valves
                    if (arrayDestination < rshwData.Length)
                    {
                        for (int i = 0; i < 150; i++)
                        {
                            if (rshwData[arrayDestination].Get(i))
                            {
                                mack.topDrawer[i] = true;
                            }
                            if (rshwData[arrayDestination].Get(i + 150))
                            {
                                mack.bottomDrawer[i] = true;
                            }
                        }
                    }

                    //Check if show is over
                    if (referenceSpeaker.time >= referenceSpeaker.clip.length)
                    {
                        if (isRandomPlaybackOn)
                        {
                            LoadMasterRandom();
                        }
                        else
                        {
                            if (!recordMovements)
                            {
                                Debug.Log("Song is over. Queuing next song / stopping.");
                                Play(true, false);
                                if (songLoopSetting == LoopVers.loopSong)
                                {
                                    if (currentShowtapeSegment == -1)
                                    {
                                        referenceSpeaker.time = 0;
                                    }
                                    else
                                    {
                                        LoadFromURL(showtapeSegmentPaths[currentShowtapeSegment]);
                                    }
                                }
                                else
                                {
                                    //Check if multi showtape or single
                                    if (currentShowtapeSegment == -1)
                                    {
                                        referenceSpeaker.time = 0;
                                        Unload();
                                    }
                                    else
                                    {
                                        currentShowtapeSegment++;
                                        //Check if end of multi showtape or not
                                        if (currentShowtapeSegment >= showtapeSegmentPaths.Length)
                                        {
                                            if (songLoopSetting == LoopVers.loopPlaylist)
                                            {
                                                currentShowtapeSegment = 0;
                                                LoadFromURL(showtapeSegmentPaths[currentShowtapeSegment]);
                                            }
                                            else
                                            {
                                                Unload();
                                            }
                                        }
                                        else
                                        {
                                            LoadFromURL(showtapeSegmentPaths[currentShowtapeSegment]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    previousFramePosition = arrayDestination;
                    previousAnyButtonHeld = inputDataObj.anyButtonHeld;
                }
            }
        }
    }

    public void Load()
    {
        CursorLockMode lockState = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Load");
        if (referenceSpeaker != null)
        {
            referenceSpeaker.time = 0;
        }

        //Call File Browser
        showtapeSegmentPaths = new string[1];
        string[] paths;
        if (fileExtention == "")
        {
            ExtensionFilter[] extensions;
            if (GameVersion.gameName == "Faz-Anim")
            {
                extensions = new ExtensionFilter[] { new ExtensionFilter("Show Files", "fshw", "tshw", "mshw") };
            }
            else
            {
                extensions = new ExtensionFilter[] { new ExtensionFilter("Show Files", "cshw", "sshw", "rshw", "nshw") };
            }

            paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape Audio", "", extensions, false);
        }
        else
        {
            paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape Audio", "", fileExtention, false);
        }
        if (paths.Length > 0)
        {
            showtapeSegmentPaths[0] = paths[0];
            currentShowtapeSegment = 0;
            LoadFromURL(paths[0]);
        }
        Cursor.lockState = lockState;
    }

    public void LoadFolder()
    {
        CursorLockMode lockState = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
        referenceSpeaker.time = 0;
        //Call File Browser
        var paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder of Showtapes", "", false);
        if (paths.Length > 0)
        {
            showtapeSegmentPaths = Directory.GetFiles(paths[0], "*." + fileExtention);
            currentShowtapeSegment = 0;
            LoadFromURL(showtapeSegmentPaths[currentShowtapeSegment]);
        }
        Cursor.lockState = lockState;
    }

    public void Play(bool force, bool onOff)
    {
        if (force)
        {
            playMovements = onOff;
        }
        else
        {
            playMovements = !playMovements;
        }
        syncTvsAndSpeakers.Invoke();
        if (playMovements)
        {
            timeSongOffset += Time.time - timePauseStart;
            timePauseStart = 0;
            audioVideoPlay.Invoke();
        }
        else
        {
            timePauseStart = Time.time;
            audioVideoPause.Invoke();
        }
    }

    public void SwapLoop()
    {
        if (songLoopSetting == LoopVers.loopPlaylist)
        {
            songLoopSetting = LoopVers.loopSong;
        }
        else if (songLoopSetting == LoopVers.loopSong)
        {
            songLoopSetting = LoopVers.noLoop;
        }
        else
        {
            songLoopSetting = LoopVers.loopPlaylist;
        }
    }

    public void SkipSong(int skip)
    {
        if (isRandomPlaybackOn)
        {
            LoadMasterRandom();
        }
        else
        {
            playMovements = false;
            referenceSpeaker.time = 0;
            if (songLoopSetting == LoopVers.noLoop || songLoopSetting == LoopVers.loopPlaylist)
            {
                currentShowtapeSegment += skip;
            }

            if (currentShowtapeSegment < 0)
            {
                currentShowtapeSegment = 0;
            }
            else if (currentShowtapeSegment >= showtapeSegmentPaths.Length)
            {
                if (songLoopSetting == LoopVers.loopPlaylist)
                {
                    currentShowtapeSegment = 0;
                }
                else
                {
                    currentShowtapeSegment = showtapeSegmentPaths.Length - 1;
                }
            }
            LoadFromURL(showtapeSegmentPaths[currentShowtapeSegment]);
        }
    }

    public void Unload()
    {
        isRandomPlaybackOn = false;
        videoPath = "";
        playMovements = false;
        recordMovements = false;
        referenceSpeaker.time = 0;
        currentShowtapeSegment = -1;
        showtapeSegmentPaths = new string[1];
        rshwData = new BitArray[0];
        audioVideoPause.Invoke();
        curtainClose.Invoke();
    }



    public void DeleteMove(int bitDelete)
    {
        int combinedNewInput = bitDelete + (24 * this.GetComponent<UI_WindowMaker>().deletePage);
        Debug.Log("Deleting Move: " + combinedNewInput);


        //Call File Browser
        showtapeSegmentPaths = new string[1];
        var paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape", "", fileExtention, false);
        if (paths.Length > 0)
        {
            showtapeSegmentPaths[0] = paths[0];
            currentShowtapeSegment = 0;
            playMovements = false;
            //Check if null
            if (showtapeSegmentPaths[0] != "")
            {
                rshwFormat thefile = rshwFormat.ReadFromFile(showtapeSegmentPaths[0]);
                speakerClip = OpenWavParser.ByteArrayToAudioClip(thefile.audioData);
                List<BitArray> newSignals = new List<BitArray>();
                int countlength = 0;
                if (thefile.signalData[0] != 0)
                {
                    countlength = 1;
                    BitArray bit = new BitArray(300);
                    newSignals.Add(bit);
                }
                for (int i = 0; i < thefile.signalData.Length; i++)
                {
                    if (thefile.signalData[i] == 0)
                    {
                        countlength += 1;
                        BitArray bit = new BitArray(300);
                        newSignals.Add(bit);
                    }
                    else
                    {
                        newSignals[countlength - 1].Set(thefile.signalData[i] - 1, true);
                    }
                }
                rshwData = newSignals.ToArray();

                //Actual Deletion Code
                for (int i = 0; i < rshwData.Length; i++)
                {
                    rshwData[i].Set(combinedNewInput - 1, false);
                }
                SaveRecording();
            }
        }
    }

    public void DeleteMoveNoSaving(int bitDelete, bool fill)
    {
        Debug.Log("Deleting Move (No Save): " + bitDelete);

        //Actual Deletion Code
        for (int i = 0; i < rshwData.Length; i++)
        {
            rshwData[i].Set(bitDelete - 1, fill);
        }
    }

    public void PadMove(int bitPad, int padding)
    {
        bitPad -= 1;
        if (padding > 0)
        {
            int oldLength = rshwData.Length;
            //Create new space
            for (int i = 0; i < padding; i++)
            {
                rshwData = rshwData.Append(new BitArray(300)).ToArray();
            }
            for (int e = 0; e < oldLength; e++)
            {
                rshwData[rshwData.Length - 1 - e].Set(bitPad, rshwData[oldLength - 1 - e].Get(bitPad));
            }
        }
        else
        {
            padding = Mathf.Abs(padding);
            for (int i = 0; i < rshwData.Length - padding; i++)
            {
                rshwData[i].Set(bitPad, rshwData[i + padding].Get(bitPad));
            }
        }
    }

    public enum addWavResult
    {
        none,
        noSource,
        uncompressed,
    }
    public addWavResult AddWav()
    {
        speakerClip = null;
        CursorLockMode lockState = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Adding Wav");
        //Call File Browser
        wavPath = "";
        showtapeSegmentPaths[0] = "";
        var paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape Audio", "", "wav", false);
        if (paths.Length > 0)
        {
            if (paths[0] != "")
            {
                wavPath = paths[0];
                speakerClip = OpenWavParser.ByteArrayToAudioClip(File.ReadAllBytes(paths[0]));
                audioVideoGetData.Invoke();
                CreateBitArray();
                if (speakerClip == null)
                {
                    return addWavResult.uncompressed;

                }
                else
                {
                    return addWavResult.noSource;
                }
            }
            return addWavResult.none;
        }
        return addWavResult.noSource;
        Cursor.lockState = lockState;
    }

    public void AddWavSpecial()
    {
        CursorLockMode lockState = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Adding Wav");
        //Call File Browser
        wavPath = "";
        showtapeSegmentPaths[0] = "";
        var paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape Audio", "", "wav", false);
        if (paths.Length > 0)
        {
            if (paths[0] != "")
            {
                wavPath = paths[0];
                speakerClip = OpenWavParser.ByteArrayToAudioClip(File.ReadAllBytes(paths[0]));
                CreateBitArray();
            }
        }
        Cursor.lockState = lockState;
    }

    public void StartNewShow()
    {
        Debug.Log("Starting New Show");
        disableCharactersOnStart = false;
        recordMovements = true;
        if (wavPath != "")
        {
            CreateBitArray();
        }
    }

    void CreateBitArray()
    {
        rshwData = new BitArray[100];
        for (int i = 0; i < rshwData.Length; i++)
        {
            rshwData[i] = new BitArray(300);
        }
    }

    public void SaveRecording()
    {
        //Stop Show
        if (rshwData != null)
        {
            audioVideoPause.Invoke();
            recordMovements = false;
            playMovements = false;
            var shw = new rshwFormat { audioData = OpenWavParser.AudioClipToByteArray(speakerClip) };
            List<int> converted = new List<int>();
            for (int i = 0; i < rshwData.Length; i++)
            {
                converted.Add(0);
                for (int e = 0; e < 300; e++)
                {
                    if (rshwData[i].Get(e) == true)
                    {
                        converted.Add(e + 1);
                    }
                }
            }
            shw.signalData = converted.ToArray();
            shw.Save(showtapeSegmentPaths[0]);
            Debug.Log("Showtape Saved");
        }
        else
        {
            Debug.Log("No Showtape. Did not save.");
        }
    }
    public bool SaveRecordingAs()
    {
        if (rshwData != null && speakerClip != null)
        {
            CursorLockMode lockState = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            //Stop Show
            audioVideoPause.Invoke();
            recordMovements = false;
            playMovements = false;
            if (speakerClip != null)
            {
                //Save to file
                var path = StandaloneFileBrowser.SaveFilePanel("Save Showtape", "", "MyShowtape", fileExtention);
                Debug.Log("Showtape Saved: " + path);
                if (!string.IsNullOrEmpty(path))
                {
                    showtapeSegmentPaths = new string[1];
                    showtapeSegmentPaths[0] = path;
                    var shw = new rshwFormat { audioData = OpenWavParser.AudioClipToByteArray(speakerClip) };
                    List<int> converted = new List<int>();
                    for (int i = 0; i < rshwData.Length; i++)
                    {
                        converted.Add(0);
                        for (int e = 0; e < 300; e++)
                        {
                            if (rshwData[i].Get(e) == true)
                            {
                                converted.Add(e + 1);
                            }
                        }
                    }
                    shw.signalData = converted.ToArray();
                    shw.Save(path);
                }
                else
                {
                    Debug.Log("No Showtape. Did not save.");
                    AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
                    sc.volume = 1;
                    sc.PlayOneShot((AudioClip)Resources.Load("Deny"));
                    Cursor.lockState = lockState;
                    return false;
                }
            }
            Cursor.lockState = lockState;
            return true;
        }
        else
        {
            Debug.Log("No Showtape. Did not save.");
            AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
            sc.volume = 1;
            sc.PlayOneShot((AudioClip)Resources.Load("Deny"));
            return false;
        }

    }

    public void LoadFromURL(string url)
    {
        StartCoroutine(LoadRoutineA(url));
    }

    IEnumerator LoadRoutineA(string url)
    {
        yield return StartCoroutine(LoadRoutineB(url));
    }

    IEnumerator LoadRoutineB(string url)
    {
        disableCharactersOnStart = false;
        playMovements = false;
        //Check if null
        if (url != "")
        {
            referenceSpeaker.time = 0;
            timeSongStarted = 0;
            timeSongOffset = 0;
            timePauseStart = 0;
            timeInputSpeedStart = 0;
            yield return null;
            //Add code for opening .rshw file
            curtainOpen.Invoke();
            yield return null;
            rshwFormat thefile = rshwFormat.ReadFromFile(url);
            yield return null;
            speakerClip = OpenWavParser.ByteArrayToAudioClip(thefile.audioData);
            yield return null;
            List<BitArray> newSignals = new List<BitArray>();
            int countlength = 0;
            if (thefile.signalData[0] != 0)
            {
                countlength = 1;
                BitArray bit = new BitArray(300);
                newSignals.Add(bit);
            }
            for (int i = 0; i < thefile.signalData.Length; i++)
            {
                if (thefile.signalData[i] == 0)
                {
                    countlength += 1;
                    BitArray bit = new BitArray(300);
                    newSignals.Add(bit);
                }
                else
                {
                    newSignals[countlength - 1].Set(thefile.signalData[i] - 1, true);
                }
            }
            rshwData = newSignals.ToArray();
            yield return null;
            if (File.Exists(url.Remove(url.Length - Mathf.Max(fileExtention.Length, 4)) + "mp4"))
            {
                Debug.Log("Video Found for Showtape.");
                videoPath = url.Remove(url.Length - Mathf.Max(fileExtention.Length, 4)) + "mp4";
            }
            else
            {
                videoPath = "";
            }
            audioVideoGetData.Invoke();
            yield return null;
            if (recordMovements)
            {
                Debug.Log("Recording Showtape: " + url + " (Length: " + ((float)countlength / 60.0f) + ")");
            }
            else
            {
                Debug.Log("Playing Showtape: " + url + " (Length: " + ((float)countlength / 60.0f) + ")");
            }
            yield return null;
            timeSongStarted = Time.time;
            syncTvsAndSpeakers.Invoke();
        }
    }

    public void SetMasterFolder()
    {
        CursorLockMode lockState = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
        var paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder of Showtapes", "", false);
        if (paths.Length > 0)
        {
            Debug.Log(paths[0]);
            PlayerPrefs.SetString("masterShowtapeList", paths[0]);
        }
        Cursor.lockState = lockState;
    }

    public void LoadMasterSegment()
    {
        UI_WindowMaker windowMaker = this.GetComponent<UI_WindowMaker>();
        string subfolder = PlayerPrefs.GetString("masterShowtapeList") + "\\" + windowMaker.allShowtapes[windowMaker.currentYear].groups[windowMaker.currentGroup].showtapeName;

        if (Directory.Exists(subfolder))
        {
            Debug.Log(subfolder);
            string[] temp = showtapeSegmentPaths;
            showtapeSegmentPaths = Directory.GetFiles(subfolder, "*." + fileExtention);
            string pathZero = showtapeSegmentPaths[windowMaker.currentSegment];
            showtapeSegmentPaths = temp;
            showtapeSegmentPaths[0] = pathZero;
            currentShowtapeSegment = 0;
            LoadFromURL(pathZero);
        }
        else
        {
            Debug.Log("Directory does not exist.");
        }
    }

    public void LoadMasterShowtape(int input)
    {
        string subfolder;
        if (input == 0)
        {
            UI_WindowMaker windowMaker = this.GetComponent<UI_WindowMaker>();
            subfolder = PlayerPrefs.GetString("masterShowtapeList") + "\\" + windowMaker.allShowtapes[windowMaker.currentYear].groups[windowMaker.currentGroup].showtapeName;
        }
        else
        {
            UI_WindowMaker windowMaker = this.GetComponent<UI_WindowMaker>();
            subfolder = PlayerPrefs.GetString("masterShowtapeList") + "\\" + windowMaker.allShowtapes[windowMaker.currentYear].groups[input - 1].showtapeName;
        }
        if (Directory.Exists(subfolder))
        {
            Debug.Log(subfolder);
            showtapeSegmentPaths = Directory.GetFiles(subfolder, "*." + fileExtention);
            currentShowtapeSegment = 0;
            LoadFromURL(showtapeSegmentPaths[currentShowtapeSegment]);
        }
        else
        {
            Debug.Log("Directory does not exist.");
        }
    }

    public void LoadMasterRandom()
    {
        isRandomPlaybackOn = true;
        var random = new System.Random();
        string[] fileNames = new string[] { };
        if (fileExtention == "")
        {
            string[] exts = new string[] { };
            if (GameVersion.gameName == "Faz-Anim")
            {
                exts = new string[] { "*.fshw", "*.tshw" , "*.mshw"};
            }
            else
            {
                exts = new string[] { "*.cshw", "*.sshw", "*.rshw", "*.nshw" };
            }
            fileNames = GetFilez(PlayerPrefs.GetString("masterShowtapeList"), SearchOption.AllDirectories, exts).ToArray();
        }
        else
        {
            fileNames = System.IO.Directory.GetFiles(PlayerPrefs.GetString("masterShowtapeList"), "*." + fileExtention, SearchOption.AllDirectories);
        }
        string randomFile = fileNames[random.Next(0, fileNames.Length)];
        Debug.Log(randomFile);
        showtapeSegmentPaths[0] = randomFile;
        currentShowtapeSegment = 0;
        LoadFromURL(randomFile);
    }
    public List<string> GetFilez(string path, System.IO.SearchOption opt, params string[] patterns)
    {
        List<string> filez = new List<string>();
        foreach (string pattern in patterns)
        {
            filez.AddRange(
                System.IO.Directory.GetFiles(path, pattern, opt)
            );
        }


        // filez.Sort(); // Optional
        return filez; // Optional: .ToArray()
    }



    public void ReplaceShowAudio()
    {
        //Call File Browser
        showtapeSegmentPaths = new string[1];
        var paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape", "", fileExtention, false);
        if (paths.Length > 0)
        {
            showtapeSegmentPaths[0] = paths[0];
            currentShowtapeSegment = 0;
            referenceSpeaker.time = 0;
            playMovements = false;
            //Check if null
            if (showtapeSegmentPaths[0] != "")
            {
                rshwFormat thefile = rshwFormat.ReadFromFile(showtapeSegmentPaths[0]);
                List<BitArray> newSignals = new List<BitArray>();
                int countlength = 0;
                if (thefile.signalData[0] != 0)
                {
                    countlength = 1;
                    BitArray bit = new BitArray(300);
                    newSignals.Add(bit);
                }
                for (int i = 0; i < thefile.signalData.Length; i++)
                {
                    if (thefile.signalData[i] == 0)
                    {
                        countlength += 1;
                        BitArray bit = new BitArray(300);
                        newSignals.Add(bit);
                    }
                    else
                    {
                        newSignals[countlength - 1].Set(thefile.signalData[i] - 1, true);
                    }
                }
                rshwData = newSignals.ToArray();

                paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape Audio", "", "wav", false);
                if (paths.Length > 0)
                {
                    if (paths[0] != "")
                    {
                        wavPath = paths[0];
                        speakerClip = OpenWavParser.ByteArrayToAudioClip(File.ReadAllBytes(paths[0]));
                        SaveRecording();
                    }
                }
            }
        }
    }

    public void EraseShowtape()
    {
        disableCharactersOnStart = false;
        playMovements = false;
        referenceSpeaker.time = 0;
        timeSongStarted = 0;
        timeSongOffset = 0;
        timePauseStart = 0;
        timeInputSpeedStart = 0;
        curtainOpen.Invoke();
        speakerClip = null;
        rshwData = null;
        videoPath = "";
        referenceSpeaker.clip = null;
    }
}
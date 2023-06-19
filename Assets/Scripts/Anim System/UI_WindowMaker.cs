using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_WindowMaker : MonoBehaviour
{
    public GameObject ThreeWindow;
    public GameObject TwoWindow;
    public GameObject TitleWindow;
    public GameObject PlayWindow;
    public GameObject NewRecordWindow;
    public GameObject RecordIconsWindow;
    public GameObject MoveTestWindow;
    public GameObject ShowtapeWindow;
    public GameObject SegmentWindow;
    public GameObject SegmentWindowHelp;
    public GameObject ShowtapeHelpWindow;
    public GameObject CharacterCustomizeWindow;
    public GameObject StageCustomizeWindow;
    public GameObject DeleteOne;

    public Sprite[] qualityIcons;

    public Static staticUI;

    public int deletePage;

    public int currentYear;
    public int currentGroup;
    public int currentSegment;

    public MovementRecordings[] recordingGroups;
    public ShowtapeYear[] allShowtapes;

    public void MakeThreeWindow(Sprite one, Sprite two, Sprite three, int switchBack, int switchOne, int switchTwo, int switchThree, string butOne, string butTwo, string butThree, int fontSize)
    {
        DisableWindows();
        ThreeWindow.SetActive(true);
        ThreeWindow.transform.Find("Icon1").GetComponent<Image>().sprite = one;
        ThreeWindow.transform.Find("Icon2").GetComponent<Image>().sprite = two;
        ThreeWindow.transform.Find("Icon3").GetComponent<Image>().sprite = three;
        ThreeWindow.transform.Find("Button1").GetComponent<Button3D>().funcWindow = switchOne;
        ThreeWindow.transform.Find("Button2").GetComponent<Button3D>().funcWindow = switchTwo;
        ThreeWindow.transform.Find("Button3").GetComponent<Button3D>().funcWindow = switchThree;
        ThreeWindow.transform.Find("Back Button").GetComponent<Button3D>().funcWindow = switchBack;
        ThreeWindow.transform.Find("Button1").GetChild(0).GetComponent<Text>().text = butOne;
        ThreeWindow.transform.Find("Button2").GetChild(0).GetComponent<Text>().text = butTwo;
        ThreeWindow.transform.Find("Button3").GetChild(0).GetComponent<Text>().text = butThree;
        ThreeWindow.transform.Find("Button1").GetChild(0).GetComponent<Text>().fontSize = fontSize;
        ThreeWindow.transform.Find("Button2").GetChild(0).GetComponent<Text>().fontSize = fontSize;
        ThreeWindow.transform.Find("Button3").GetChild(0).GetComponent<Text>().fontSize = fontSize;
    }

    public void MakeTwoWindow(Sprite one, Sprite two, int switchBack, int switchOne, int switchTwo, string butOne, string butTwo, int fontSize)
    {
        DisableWindows();
        TwoWindow.SetActive(true);
        TwoWindow.transform.Find("Icon1").GetComponent<Image>().sprite = one;
        TwoWindow.transform.Find("Icon2").GetComponent<Image>().sprite = two;
        TwoWindow.transform.Find("Button1").GetComponent<Button3D>().funcWindow = switchOne;
        TwoWindow.transform.Find("Button2").GetComponent<Button3D>().funcWindow = switchTwo;
        TwoWindow.transform.Find("Back Button").GetComponent<Button3D>().funcWindow = switchBack;
        TwoWindow.transform.Find("Button1").GetChild(0).GetComponent<Text>().text = butOne;
        TwoWindow.transform.Find("Button2").GetChild(0).GetComponent<Text>().text = butTwo;
        TwoWindow.transform.Find("Button1").GetChild(0).GetComponent<Text>().fontSize = fontSize;
        TwoWindow.transform.Find("Button2").GetChild(0).GetComponent<Text>().fontSize = fontSize;
    }

    public void MakeShowtapeWindow(int add)
    {
        DisableWindows();
        currentSegment = 0;
        ShowtapeWindow.SetActive(true);
        currentYear = Mathf.Min(Mathf.Max((currentYear + add),0),50);
        if(currentYear == 0)
        {
            ShowtapeWindow.transform.Find("Year Back").gameObject.SetActive(false);
        }
        else
        {
            ShowtapeWindow.transform.Find("Year Back").gameObject.SetActive(true);
        }
        if(currentYear == 50)
        {
            ShowtapeWindow.transform.Find("Year Forward").gameObject.SetActive(false);
        }
        else
        {
            ShowtapeWindow.transform.Find("Year Forward").gameObject.SetActive(true);
        }
        if (allShowtapes[currentYear].groups.Length == 0)
        {
            ShowtapeWindow.transform.Find("Error Text").gameObject.SetActive(true);
            for (int i = 1; i < 16; i++)
            {
                ShowtapeWindow.transform.Find("b" + i).gameObject.SetActive(false);
            }
        }
        else
        {
            ShowtapeWindow.transform.Find("Error Text").gameObject.SetActive(false);
            for (int i = 1; i < 17; i++)
            {
                if (i-1 < allShowtapes[currentYear].groups.Length)
                {
                    ShowtapeWindow.transform.Find("b" + i).gameObject.SetActive(true);
                    ShowtapeWindow.transform.Find("b" + i).transform.Find("Text").GetComponent<Text>().text = allShowtapes[currentYear].groups[i-1].showtapeName;
                    ShowtapeWindow.transform.Find("b" + i).transform.Find("Text").GetChild(0).GetComponent<Text>().text = allShowtapes[currentYear].groups[i-1].showtapeDate;
                    if(allShowtapes[currentYear].groups[i - 1].segments.Length == 0)
                    {
                        ShowtapeWindow.transform.Find("b" + i).transform.Find("Text").GetComponent<Text>().color = new Color32(188, 119, 0, 255);
                        ShowtapeWindow.transform.Find("b" + i).transform.Find("Text").GetChild(0).GetComponent<Text>().color = new Color32(198, 176, 0, 255);
                        ShowtapeWindow.transform.Find("b" + i).transform.Find("Length").GetComponent<Text>().color = Color.gray;
                    }
                    else
                    {
                        ShowtapeWindow.transform.Find("b" + i).transform.Find("Text").GetComponent<Text>().color = new Color32(255, 162, 0, 255);
                        ShowtapeWindow.transform.Find("b" + i).transform.Find("Text").GetChild(0).GetComponent<Text>().color = new Color32(255, 227, 0, 255);
                        ShowtapeWindow.transform.Find("b" + i).transform.Find("Length").GetComponent<Text>().color = Color.white;
                    }
                    //Length
                    if(allShowtapes[currentYear].groups[i - 1].segments.Length != 0)
                    {
                        int[] adding = new int[allShowtapes[currentYear].groups[i - 1].segments.Length*2];
                        int j = 0;
                        for (int e = 0; e < allShowtapes[currentYear].groups[i - 1].segments.Length*2; e++)
                        {
                            if(e%2 == 0)
                            {
                                adding[e] = Convert.ToInt32(allShowtapes[currentYear].groups[i - 1].segments[j].showLength.Split(':')[0]);
                                
                            }
                            else
                            {
                                adding[e] = Convert.ToInt32(allShowtapes[currentYear].groups[i - 1].segments[j].showLength.Split(':')[1]);
                                j++;
                            }
                        }
                        int hours = 0;
                        int minutes = 0;
                        int seconds = 0;
                        for (int e = 0; e < adding.Length; e++)
                        {
                            if (e % 2 == 0)
                            {
                                minutes += adding[e];
                            }
                            else
                            {
                                seconds += adding[e];
                            }

                        }
                        while(seconds >= 60)
                        {
                            seconds -= 60;
                            minutes++;
                        }
                        while (minutes >= 60)
                        {
                            minutes -= 60;
                            hours++;
                        }
                        if(hours > 0)
                        {
                            ShowtapeWindow.transform.Find("b" + i).transform.Find("Length").GetComponent<Text>().text = hours + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
                        }
                        else
                        {
                            ShowtapeWindow.transform.Find("b" + i).transform.Find("Length").GetComponent<Text>().text = minutes + ":" + seconds.ToString("00");
                        }
                        
                    }
                    else
                    {
                        ShowtapeWindow.transform.Find("b" + i).transform.Find("Length").GetComponent<Text>().text = "00:00";
                    }
                   
                }
                else
                {
                    ShowtapeWindow.transform.Find("b" + i).gameObject.SetActive(false);
                }
            }
        }
        ShowtapeWindow.transform.Find("Year Text").GetComponent<Text>().text = (1970 + currentYear).ToString();
    }
    public void MakeSegmentWindow(int current)
    {
        DisableWindows();
        if (current == -1)
        {
            currentSegment = Mathf.Min(currentSegment + 1, allShowtapes[currentYear].groups[currentGroup].segments.Length - 1);
        }
        else if (current == -2)
        {
            currentSegment = Mathf.Max(currentSegment - 1, 0);
        }
        else
        {
            currentGroup = current;
        }
        if (allShowtapes[currentYear].groups[currentGroup].segments.Length == 0)
        {
            SegmentWindowHelp.SetActive(true);
        }
        else
        {
            SegmentWindow.SetActive(true);
            if (currentSegment == 0)
            {
                SegmentWindow.transform.Find("Left").gameObject.SetActive(false);
            }
            else
            {
                SegmentWindow.transform.Find("Left").gameObject.SetActive(true);
            }
            if (currentSegment == allShowtapes[currentYear].groups[currentGroup].segments.Length - 1)
            {
                SegmentWindow.transform.Find("Right").gameObject.SetActive(false);
            }
            else
            {
                SegmentWindow.transform.Find("Right").gameObject.SetActive(true);
            }
            SegmentWindow.transform.Find("Show Name").GetComponent<Text>().text = allShowtapes[currentYear].groups[currentGroup].showtapeName;
            SegmentWindow.transform.Find("Segment Name").GetComponent<Text>().text = allShowtapes[currentYear].groups[currentGroup].segments[currentSegment].segmentNumber;
            SegmentWindow.transform.Find("Segment").GetComponent<Text>().text = allShowtapes[currentYear].groups[currentGroup].segments[currentSegment].showName.Replace("\\n", "\n");
            SegmentWindow.transform.Find("Length").GetComponent<Text>().text = "Length: (" + allShowtapes[currentYear].groups[currentGroup].segments[currentSegment].showLength + ")";
            switch (allShowtapes[currentYear].groups[currentGroup].segments[currentSegment].audioQuality)
            {
                case ShowtapeSegment.AudioQ.None:
                    SegmentWindow.transform.Find("Audio").GetComponent<Image>().sprite = qualityIcons[10];
                    break;
                case ShowtapeSegment.AudioQ.Unobtained:
                    SegmentWindow.transform.Find("Audio").GetComponent<Image>().sprite = qualityIcons[14];
                    break;
                case ShowtapeSegment.AudioQ.Incomplete:
                    SegmentWindow.transform.Find("Audio").GetComponent<Image>().sprite = qualityIcons[6];
                    break;
                case ShowtapeSegment.AudioQ.Corrupted:
                    SegmentWindow.transform.Find("Audio").GetComponent<Image>().sprite = qualityIcons[1];
                    break;
                case ShowtapeSegment.AudioQ.LowQuality:
                    SegmentWindow.transform.Find("Audio").GetComponent<Image>().sprite = qualityIcons[7];
                    break;
                case ShowtapeSegment.AudioQ.ReelQuality:
                    SegmentWindow.transform.Find("Audio").GetComponent<Image>().sprite = qualityIcons[12];
                    break;
                case ShowtapeSegment.AudioQ.MasteredAudio:
                    SegmentWindow.transform.Find("Audio").GetComponent<Image>().sprite = qualityIcons[8];
                    break;
                default:
                    break;
            }
            switch (allShowtapes[currentYear].groups[currentGroup].segments[currentSegment].videoQuality)
            {
                case ShowtapeSegment.VideoQ.None:
                    SegmentWindow.transform.Find("Video").GetComponent<Image>().sprite = qualityIcons[10];
                    break;
                case ShowtapeSegment.VideoQ.Unobtained:
                    SegmentWindow.transform.Find("Video").GetComponent<Image>().sprite = qualityIcons[14];
                    break;
                case ShowtapeSegment.VideoQ.Incomplete:
                    SegmentWindow.transform.Find("Video").GetComponent<Image>().sprite = qualityIcons[6];
                    break;
                case ShowtapeSegment.VideoQ.Corrupted:
                    SegmentWindow.transform.Find("Video").GetComponent<Image>().sprite = qualityIcons[1];
                    break;
                case ShowtapeSegment.VideoQ.LowQuality:
                    SegmentWindow.transform.Find("Video").GetComponent<Image>().sprite = qualityIcons[7];
                    break;
                case ShowtapeSegment.VideoQ.ReelQuality:
                    SegmentWindow.transform.Find("Video").GetComponent<Image>().sprite = qualityIcons[13];
                    break;
                case ShowtapeSegment.VideoQ.MasteredVideo:
                    SegmentWindow.transform.Find("Video").GetComponent<Image>().sprite = qualityIcons[8];
                    break;
                default:
                    break;
            }
            switch (allShowtapes[currentYear].groups[currentGroup].segments[currentSegment].signalQuality)
            {
                case ShowtapeSegment.SignalQ.None:
                    SegmentWindow.transform.Find("Signal").GetComponent<Image>().sprite = qualityIcons[10];
                    break;
                case ShowtapeSegment.SignalQ.Unobtained:
                    SegmentWindow.transform.Find("Signal").GetComponent<Image>().sprite = qualityIcons[14];
                    break;
                case ShowtapeSegment.SignalQ.Incomplete:
                    SegmentWindow.transform.Find("Signal").GetComponent<Image>().sprite = qualityIcons[6];
                    break;
                case ShowtapeSegment.SignalQ.Corrupted:
                    SegmentWindow.transform.Find("Signal").GetComponent<Image>().sprite = qualityIcons[1];
                    break;
                case ShowtapeSegment.SignalQ.Complete:
                    SegmentWindow.transform.Find("Signal").GetComponent<Image>().sprite = qualityIcons[0];
                    break;
                default:

                    break;
            }
            switch (allShowtapes[currentYear].groups[currentGroup].segments[currentSegment].showVersion)
            {
                case ShowtapeSegment.ShowQ.Original:
                    SegmentWindow.transform.Find("Version").GetComponent<Image>().sprite = qualityIcons[11];
                    break;
                case ShowtapeSegment.ShowQ.Edited:
                    SegmentWindow.transform.Find("Version").GetComponent<Image>().sprite = qualityIcons[5];
                    break;
                case ShowtapeSegment.ShowQ.Duplicate:
                    SegmentWindow.transform.Find("Version").GetComponent<Image>().sprite = qualityIcons[4];
                    break;
                case ShowtapeSegment.ShowQ.DuplicateEdited:
                    SegmentWindow.transform.Find("Version").GetComponent<Image>().sprite = qualityIcons[2];
                    break;
                case ShowtapeSegment.ShowQ.NewSignals:
                    SegmentWindow.transform.Find("Version").GetComponent<Image>().sprite = qualityIcons[9];
                    break;
                case ShowtapeSegment.ShowQ.DuplicateNewSignals:
                    SegmentWindow.transform.Find("Version").GetComponent<Image>().sprite = qualityIcons[3];
                    break;
                default:
                    break;
            }
        }
    }
    public void MakeShowtapeHelpWindow(int add)
    {
        DisableWindows();
        ShowtapeHelpWindow.SetActive(true);
    }

    public void MakeTitleWindow()
    {
        DisableWindows();
        TitleWindow.SetActive(true);
    }

    public void MakePlayWindow(bool recording)
    {
        DisableWindows();
        PlayWindow.SetActive(true);
    }

    public void MakeNewRecordWindow()
    {
        DisableWindows();
        NewRecordWindow.SetActive(true);
    }

    public void MakeRecordIconsWindow()
    {
        DisableWindows();
        RecordIconsWindow.SetActive(true);

        for (int i = 0; i < 24; i++)
        {
            if (i < recordingGroups.Length)
            {
                GameObject button = RecordIconsWindow.transform.Find("Button (" + i + ")").gameObject;
                button.SetActive(true);
                button.GetComponent<Image>().sprite = recordingGroups[i].groupIcon;
                button.transform.GetChild(0).GetComponent<Button3D>().funcName = "RecordingGroupMenu";
                button.transform.GetChild(0).GetComponent<Button3D>().funcWindow = i;
                button.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = recordingGroups[i].groupName;
            }
            else
            {
                RecordIconsWindow.transform.Find("Button (" + i + ")").gameObject.SetActive(false);
            }
        }
        RecordIconsWindow.transform.Find("Back Button").GetComponent<Button3D>().funcWindow = 4;
    }

    public void MakeCharacterCustomizeIconsWindow(CharacterSelector[] characters)
    {
        DisableWindows();
        RecordIconsWindow.SetActive(true);

        for (int i = 0; i < 24; i++)
        {
            if(i < characters.Length && characters[i].allCostumes.Length > 1)
            {
                GameObject button = RecordIconsWindow.transform.Find("Button (" + i + ")").gameObject;
                button.SetActive(true);
                button.GetComponent<Image>().sprite = characters[i].icon;
                button.transform.GetChild(0).GetComponent<Button3D>().funcName = "CharacterCustomMenu";
                button.transform.GetChild(0).GetComponent<Button3D>().funcWindow = i;
                button.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = characters[i].characterName;
            }
            else
            {
                RecordIconsWindow.transform.Find("Button (" + i + ")").gameObject.SetActive(false);
            }
        }
        RecordIconsWindow.transform.Find("Back Button").GetComponent<Button3D>().funcWindow = 8;
    }

    public void MakeMoveTestWindow(int currentGroup)
    {
        DisableWindows();
        MoveTestWindow.transform.Find("Ready").GetComponent<InputSetter>().mapping = currentGroup + 1;
        MoveTestWindow.SetActive(true);
        for (int i = 0; i < 33; i++)
        {
            if (i < recordingGroups[currentGroup].inputNames.Length)
            {
                GameObject button = MoveTestWindow.transform.Find("Button " + i).gameObject;
                
                button.SetActive(true);
                button.transform.transform.Find("Text").GetComponent<Text>().text = recordingGroups[currentGroup].inputNames[i].name;
            }
            else
            {
                MoveTestWindow.transform.Find("Button " + i).gameObject.SetActive(false);
            }
        }
        MoveTestWindow.transform.Find("Back Button").GetComponent<Button3D>().funcWindow = 21;
       
    }

    public void MakeCharacterCustomizeWindow(CharacterSelector[] characters, int current)
    {
        DisableWindows();
        CharacterCustomizeWindow.SetActive(true);
        if (characters[current].currentCostume == characters[current].allCostumes.Length - 1)
        {
            CharacterCustomizeWindow.transform.Find("Down").gameObject.SetActive(false);
        }
        else
        {
            CharacterCustomizeWindow.transform.Find("Down").gameObject.SetActive(true);
        }
        if (characters[current].currentCostume == -1)
        {
            CharacterCustomizeWindow.transform.Find("Up").gameObject.SetActive(false);
        }
        else
        {
            CharacterCustomizeWindow.transform.Find("Up").gameObject.SetActive(true);
        }
        if (characters[current].currentCostume != -1)
        {
            CharacterCustomizeWindow.transform.Find("Full Costume").gameObject.GetComponent<Text>().text = characters[current].allCostumes.Length.ToString();
            CharacterCustomizeWindow.transform.Find("Current Costume").gameObject.GetComponent<Text>().text = (1 + characters[current].currentCostume).ToString();
            CharacterCustomizeWindow.transform.Find("Name").gameObject.GetComponent<Text>().text = characters[current].allCostumes[characters[current].currentCostume].costumeName;
            CharacterCustomizeWindow.transform.Find("Type").gameObject.GetComponent<Text>().text = characters[current].allCostumes[characters[current].currentCostume].costumeType.ToString();
            CharacterCustomizeWindow.transform.Find("Description").gameObject.GetComponent<Text>().text = characters[current].allCostumes[characters[current].currentCostume].costumeDesc;
            CharacterCustomizeWindow.transform.Find("Year").gameObject.GetComponent<Text>().text = characters[current].allCostumes[characters[current].currentCostume].yearOfCostume;
            CharacterCustomizeWindow.transform.Find("Down").gameObject.GetComponent<Button3D>().funcWindow = current;
            CharacterCustomizeWindow.transform.Find("Up").gameObject.GetComponent<Button3D>().funcWindow = current;
            CharacterCustomizeWindow.transform.Find("Icon").gameObject.GetComponent<RawImage>().texture = characters[current].allCostumes[characters[current].currentCostume].costumeIcon.texture;
        }
        else
        {
            CharacterCustomizeWindow.transform.Find("Full Costume").gameObject.GetComponent<Text>().text = characters[current].allCostumes.Length.ToString();
            CharacterCustomizeWindow.transform.Find("Current Costume").gameObject.GetComponent<Text>().text = (1 + characters[current].currentCostume).ToString();
            CharacterCustomizeWindow.transform.Find("Name").gameObject.GetComponent<Text>().text = "None";
            CharacterCustomizeWindow.transform.Find("Type").gameObject.GetComponent<Text>().text = "";
            CharacterCustomizeWindow.transform.Find("Description").gameObject.GetComponent<Text>().text = "No character is present.";
            CharacterCustomizeWindow.transform.Find("Year").gameObject.GetComponent<Text>().text = "";
            CharacterCustomizeWindow.transform.Find("Down").gameObject.GetComponent<Button3D>().funcWindow = current;
            CharacterCustomizeWindow.transform.Find("Up").gameObject.GetComponent<Button3D>().funcWindow = current;
            CharacterCustomizeWindow.transform.Find("Icon").gameObject.GetComponent<RawImage>().texture = characters[current].allCostumes[characters[current].currentCostume+1].costumeIcon.texture;

        }
    }

    public void MakeStageCustomizeWindow(StageSelector[] stages, int current)
    {
        DisableWindows();
        StageCustomizeWindow.SetActive(true);
        StageCustomizeWindow.transform.Find("Full Costume").gameObject.GetComponent<Text>().text = stages.Length.ToString();
        StageCustomizeWindow.transform.Find("Current Costume").gameObject.GetComponent<Text>().text = (1 + current).ToString();
        StageCustomizeWindow.transform.Find("Name").gameObject.GetComponent<Text>().text = stages[current].stageName;
        StageCustomizeWindow.transform.Find("Type").gameObject.GetComponent<Text>().text = stages[current].stageType.ToString();
        StageCustomizeWindow.transform.Find("Description").gameObject.GetComponent<Text>().text = stages[current].stageDesc;
        StageCustomizeWindow.transform.Find("Year").gameObject.GetComponent<Text>().text = stages[current].stageDate;
        StageCustomizeWindow.transform.Find("Type").gameObject.GetComponent<Text>().text = stages[current].stageType.ToString();
        StageCustomizeWindow.transform.Find("Down").gameObject.GetComponent<Button3D>().funcWindow = current;
        StageCustomizeWindow.transform.Find("Up").gameObject.GetComponent<Button3D>().funcWindow = current;
        StageCustomizeWindow.transform.Find("Icon").gameObject.GetComponent<RawImage>().texture = stages[current].stageIcon.texture;
    }

    public void MakeDeleteMoveMenu(int page)
    {
        DisableWindows();
        DeleteOne.SetActive(true);
        deletePage += page;
        if(deletePage < 0)
        {
            deletePage = 0;
        }
        if(deletePage > 12)
        {
            deletePage = 12;
        }
        DeleteOne.transform.Find("pagenum").gameObject.GetComponent<Text>().text = deletePage.ToString() + " / 12";
        for (int i = 0; i < 24; i++)
        {
            DeleteOne.transform.Find("DLMV (" + i.ToString() + ")").gameObject.transform.GetChild(0).GetComponent<Text>().text = SearchBitChartName((i + 1) + (24 * deletePage));
        }
    }

    public String SearchBitChartName(int bit)
    {
        for (int i = 0; i < recordingGroups.Length; i++)
        {
            for (int e = 0; e < recordingGroups[i].inputNames.Length; e++)
            {
                int finalBitNum = 0;
                if(recordingGroups[i].inputNames[e].drawer)
                {
                    finalBitNum += 150;
                }
                if (recordingGroups[i].inputNames[e].index[0] + finalBitNum == bit)
                {
                    return bit.ToString() + "- " + recordingGroups[i].groupName + " - " + recordingGroups[i].inputNames[e].name;
                }
            }
            
        }
        return bit.ToString() + "- Nothing";
    }

    void DisableWindows()
    {
        staticUI.flash = true;
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if(this.transform.GetChild(i).gameObject.activeSelf)
            {
                this.transform.GetChild(i).gameObject.SetActive(false);
                break;
            }
        }
    }

    [System.Serializable]
    public class MovementRecordings
    {
        public string groupName;
        public Sprite groupIcon;
        public inputNames[] inputNames;
    }
    [System.Serializable]
    public class inputNames
    {
        public string name;
        public bool drawer;
        public int[] index;
    }
}

[System.Serializable]
public class ShowtapeYear
{
    public ShowTapeSelector[] groups;
}
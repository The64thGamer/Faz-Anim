using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UI_VideoPlayer : MonoBehaviour
{
    public UI_VideoYear[] videoFolders;
    int currentPage;
    int currentYear;
    int currentVideo;
    int currentType;

    public GameObject TV;
    VideoPlayer TVVid;

    public GameObject pause;
    public GameObject loop;
    public GameObject start;
    public GameObject end;
    Text startTT;
    Text endTT;

    public GameObject slider;
    Slider sliderUI;

    public Sprite playSP;
    public Sprite pauseSP;

    public Sprite loopOff;
    public Sprite loopOn;

    public GameObject windowYear;
    public GameObject windowType;
    public GameObject windowVideo;
    public GameObject windowPlayer;

    // Start is called before the first frame update
    private void Awake()
    {
        StartCoroutine(AwakeCoroutine());
    }

    IEnumerator AwakeCoroutine()
    {
        TVVid = TV.GetComponent<VideoPlayer>();
        yield return new WaitForSeconds(.5f);
        UpdatePage(0);
        yield return new WaitForSeconds(.5f);
        sliderUI = slider.GetComponent<Slider>();
        yield return new WaitForSeconds(.5f);
        startTT = start.GetComponent<Text>();
        yield return new WaitForSeconds(.5f);
        endTT = end.GetComponent<Text>();
    }

    private void Update()
    {
        if(TVVid.isPlaying)
        {
            sliderUI.value = (float)(TVVid.time / TVVid.clip.length);
            startTT.text = (Mathf.Floor((int)TVVid.time / 60f)).ToString("00") + ":" + ((int)TVVid.time % 60).ToString("00");
        }
    }

    public void UpdatePage(int add)
    {
        windowYear.SetActive(true);
        windowType.SetActive(false);
        windowVideo.SetActive(false);
        windowPlayer.SetActive(false);

        currentPage = Mathf.Min(Mathf.Max(currentPage + add, 0), 5);
        if (currentPage == 0)
        {
            windowYear.transform.Find("right").gameObject.SetActive(true);
            windowYear.transform.Find("left").gameObject.SetActive(false);
        }
        else if (currentPage == 5)
        {
            windowYear.transform.Find("right").gameObject.SetActive(false);
            windowYear.transform.Find("left").gameObject.SetActive(true);
        }
        else
        {
            windowYear.transform.Find("right").gameObject.SetActive(true);
            windowYear.transform.Find("left").gameObject.SetActive(true);
        }

        windowYear.transform.Find("yearText").GetComponent<Text>().text = (1970 + (currentPage * 10)).ToString() + "'s";

        for (int i = 0; i < 10; i++)
        {
            Color buttoncolor;
            UI_VideoYear vF = videoFolders[(currentPage * 10)+i];
            float totalVids = vF.Diagnostic.Length + vF.Promotional.Length + vF.Installation.Length + vF.Training.Length + vF.Commercial.Length + vF.News.Length + vF.Amatuer.Length + vF.Other.Length;

            if (totalVids == 0)
            {
                buttoncolor = Color.grey;
            }
            else
            {
                buttoncolor = Color.white;
            }
            windowYear.transform.Find(i.ToString()).GetComponent<Image>().color = buttoncolor;
            windowYear.transform.Find(i.ToString()).transform.Find("Text").GetComponent<Text>().color = buttoncolor;
            windowYear.transform.Find(i.ToString()).transform.Find("Text (1)").GetComponent<Text>().color = Color.black;

            windowYear.transform.Find(i.ToString()).transform.Find("Text").GetComponent<Text>().text = (1970 + (currentPage * 10) + i).ToString();
            windowYear.transform.Find(i.ToString()).transform.Find("Text (1)").GetComponent<Text>().text = totalVids.ToString();
        }
    }
    public void GoToYear(int add)
    {
        currentYear = add;
        currentVideo = 0;

        UI_VideoYear vF = videoFolders[(currentPage * 10) + add];
        float totalVids = vF.Diagnostic.Length + vF.Promotional.Length + vF.Installation.Length + vF.Training.Length + vF.Commercial.Length + vF.News.Length + vF.Amatuer.Length + vF.Other.Length;

        if (totalVids != 0)
        {
            windowYear.SetActive(false);
            windowType.SetActive(true);
            windowVideo.SetActive(false);
            windowPlayer.SetActive(false);
            windowType.transform.Find("yearText").GetComponent<Text>().text = (1970 + (currentPage * 10) + add).ToString();

            for (int i = 0; i < 8; i++)
            {
                int vidCount = 0;
                Color buttoncolor;
                switch (i)
                {
                    case 0:
                        vidCount = videoFolders[(currentPage * 10) + add].Diagnostic.Length;
                        break;
                    case 1:
                        vidCount = videoFolders[(currentPage * 10) + add].Promotional.Length;
                        break;
                    case 2:
                        vidCount = videoFolders[(currentPage * 10) + add].Installation.Length;
                        break;
                    case 3:
                        vidCount = videoFolders[(currentPage * 10) + add].Training.Length;
                        break;
                    case 4:
                        vidCount = videoFolders[(currentPage * 10) + add].Commercial.Length;
                        break;
                    case 5:
                        vidCount = videoFolders[(currentPage * 10) + add].News.Length;
                        break;
                    case 6:
                        vidCount = videoFolders[(currentPage * 10) + add].Amatuer.Length;
                        break;
                    case 7:
                        vidCount = videoFolders[(currentPage * 10) + add].Other.Length;
                        break;
                    default:
                        break;
                }
                Debug.Log(vidCount);
                if (vidCount == 0)
                {
                    buttoncolor = Color.grey;
                }
                else
                {
                    buttoncolor = Color.white;
                }
                windowType.transform.Find(i.ToString()).GetComponent<Image>().color = buttoncolor;
                windowType.transform.Find(i.ToString()).transform.Find("Text").GetComponent<Text>().color = buttoncolor;
                windowType.transform.Find(i.ToString()).transform.Find("Text (1)").GetComponent<Text>().color = Color.black;
                windowType.transform.Find(i.ToString()).transform.Find("Text (1)").GetComponent<Text>().text = vidCount.ToString();
            }
        }
    }

    public void MakeVideo(int type)
    {
        currentType = type;
        VideoSelector[] selection = null;
        switch (currentType)
        {
            case 0:
                selection = videoFolders[(currentPage * 10) + currentYear].Diagnostic;
                break;
            case 1:
                selection = videoFolders[(currentPage * 10) + currentYear].Promotional;
                break;
            case 2:
                selection = videoFolders[(currentPage * 10) + currentYear].Installation;
                break;
            case 3:
                selection = videoFolders[(currentPage * 10) + currentYear].Training;
                break;
            case 4:
                selection = videoFolders[(currentPage * 10) + currentYear].Commercial;
                break;
            case 5:
                selection = videoFolders[(currentPage * 10) + currentYear].News;
                break;
            case 6:
                selection = videoFolders[(currentPage * 10) + currentYear].Amatuer;
                break;
            case 7:
                selection = videoFolders[(currentPage * 10) + currentYear].Other;
                break;
            default:
                break;
        }
        if(selection.Length != 0)
        {
            TV.GetComponent<VideoPlayer>().Stop();
            windowYear.SetActive(false);
            windowType.SetActive(false);
            windowVideo.SetActive(true);
            windowPlayer.SetActive(false);


            windowVideo.transform.Find("title").GetComponent<Text>().text = selection[currentVideo].videoName;
            windowVideo.transform.Find("description").GetComponent<Text>().text = selection[currentVideo].videoDescription;
            windowVideo.transform.Find("yearText").GetComponent<Text>().text = selection[currentVideo].videoDate;
            windowVideo.transform.Find("icon").GetComponent<RawImage>().texture = selection[currentVideo].videoIcon;
            if (currentVideo == 0)
            {
                windowVideo.transform.Find("left").gameObject.SetActive(false);
            }
            else
            {
                windowVideo.transform.Find("left").gameObject.SetActive(true);
            }
            if (currentVideo == selection.Length - 1)
            {
                windowVideo.transform.Find("right").gameObject.SetActive(false);
            }
            else
            {
                windowVideo.transform.Find("right").gameObject.SetActive(true);
            }
        }
    }

    public void AddVideo(int add)
    {
        int vidCount = 0;
        switch (currentType)
        {
            case 0:
                vidCount = videoFolders[(currentPage * 10) + currentYear].Diagnostic.Length;
                break;
            case 1:
                vidCount = videoFolders[(currentPage * 10) + currentYear].Promotional.Length;
                break;
            case 2:
                vidCount = videoFolders[(currentPage * 10) + currentYear].Installation.Length;
                break;
            case 3:
                vidCount = videoFolders[(currentPage * 10) + currentYear].Training.Length;
                break;
            case 4:
                vidCount = videoFolders[(currentPage * 10) + currentYear].Commercial.Length;
                break;
            case 5:
                vidCount = videoFolders[(currentPage * 10) + currentYear].News.Length;
                break;
            case 6:
                vidCount = videoFolders[(currentPage * 10) + currentYear].Amatuer.Length;
                break;
            case 7:
                vidCount = videoFolders[(currentPage * 10) + currentYear].Other.Length;
                break;
            default:
                break;
        }
        currentVideo = Mathf.Min(Mathf.Max(currentVideo + add, 0), vidCount);
        MakeVideo(currentType);
    }

    public void PlayVideo(int input)
    {
        windowYear.SetActive(false);
        windowType.SetActive(false);
        windowVideo.SetActive(false);
        windowPlayer.SetActive(true);
        VideoSelector[] selection = null;
        switch (currentType)
        {
            case 0:
                selection = videoFolders[(currentPage * 10) + currentYear].Diagnostic;
                break;
            case 1:
                selection = videoFolders[(currentPage * 10) + currentYear].Promotional;
                break;
            case 2:
                selection = videoFolders[(currentPage * 10) + currentYear].Installation;
                break;
            case 3:
                selection = videoFolders[(currentPage * 10) + currentYear].Training;
                break;
            case 4:
                selection = videoFolders[(currentPage * 10) + currentYear].Commercial;
                break;
            case 5:
                selection = videoFolders[(currentPage * 10) + currentYear].News;
                break;
            case 6:
                selection = videoFolders[(currentPage * 10) + currentYear].Amatuer;
                break;
            case 7:
                selection = videoFolders[(currentPage * 10) + currentYear].Other;
                break;
            default:
                break;
        }
        TV.GetComponent<VideoPlayer>().clip = selection[currentVideo].video;
        TV.GetComponent<VideoPlayer>().Play();
        windowPlayer.transform.Find("title").GetComponent<Text>().text = selection[currentVideo].videoName;
        windowPlayer.transform.Find("icon").GetComponent<RawImage>().texture = selection[currentVideo].videoIcon;
        PauseVideo(-64);
        LoopVideo(-64);
        endTT.text = (Mathf.Floor((int)TVVid.clip.length / 60f)).ToString("00") + ":" + ((int)TVVid.clip.length % 60).ToString("00");
    }

    public void PauseVideo(int input)
    {  
        if (TVVid.isPaused)
        {
            TVVid.Play();
            pause.GetComponent<Image>().sprite = playSP;
        }
        else
        {
            TVVid.Pause();
            pause.GetComponent<Image>().sprite = pauseSP;
        }
        if (input == -64)
        {
            TVVid.Play();
            pause.GetComponent<Image>().sprite = playSP;
        }
    }

    public void LoopVideo(int input)
    {
        if (TVVid.isLooping)
        {
            TVVid.isLooping = false;
            loop.GetComponent<Image>().sprite = loopOff;
        }
        else
        {
            TVVid.isLooping = true;
            loop.GetComponent<Image>().sprite = loopOn;
        }
        if (input == -64)
        {
            TVVid.isLooping = false;
            loop.GetComponent<Image>().sprite = loopOff;
        }
    }

    public void ResetVideo(int input)
    {
        TVVid.time = 0;
    }
    public void SkipVideo(int input)
    {
        TVVid.time += input;
    }

}
    [System.Serializable] 
public class UI_VideoYear
{
    public VideoSelector[] Diagnostic;
    public VideoSelector[] Promotional;
    public VideoSelector[] Installation;
    public VideoSelector[] Training;
    public VideoSelector[] Commercial;
    public VideoSelector[] News;
    public VideoSelector[] Amatuer;
    public VideoSelector[] Other;
}
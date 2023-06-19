using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_MerchSelection : MonoBehaviour
{
    public GameObject cornerDisplay;
    public GlobalController gc;

    public string[] shelfPrizes;
    public bool resetPrizeDay;
    public bool resetAllPrizes;
    bool prizesLoaded;

    // Start is called before the first frame update
    void Awake()
    {
        shelfPrizes = new string[8];
        StartCoroutine(AwakeCoroutine());
    }

    IEnumerator AwakeCoroutine()
    {
        StartCoroutine(LoadSavedData());
        while (!prizesLoaded)
        {
            yield return null;
        }
        CheckAndCreatePrizes();
        DisplayPrizes();
    }

    private void Update()
    {
        if (resetPrizeDay)
        {
            PlayerPrefs.SetInt("TicketCount", PlayerPrefs.GetInt("TicketCount") + 1000);
            resetPrizeDay = false;
            PlayerPrefs.SetInt("CurrentPrizeDayTimeFA", -1);
            CheckAndCreatePrizes();
            DisplayPrizes();
        }
        if(resetAllPrizes)
        {
            resetAllPrizes = false;
            for (int e = 0; e < gc.prizeStrings.Length; e++)
            {
                for (int i = 0; i < gc.prizeStrings[e].prizeStrings.Length; i++)
                {
                    PlayerPrefs.SetInt(gc.prizeStrings[e].prizeStrings[i].name, 0);
                }
            }
        }
    }
    void BuyPrize(int prize)
    {
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        string price = GetItemPrice(PlayerPrefs.GetString("Shelf Prize: " + prize));
        if(price != "")
        {
            int realPrice = Convert.ToInt32(price);
            if (realPrice <= PlayerPrefs.GetInt("TicketCount"))
            {
                PlayerPrefs.SetInt(shelfPrizes[prize], 64);
                PlayerPrefs.SetInt("TicketCount", PlayerPrefs.GetInt("TicketCount") - realPrice);
                PlayerPrefs.SetString("Shelf Prize: " + prize, "");
                sc.clip = (AudioClip)Resources.Load("tap");
                StartCoroutine(LoadSavedData());
                CheckAndCreatePrizes();
                DisplayPrizes();
            }
            else
            {
                sc.clip = (AudioClip)Resources.Load("big tap");
            }
        }
    }
    void DisplayPrizes()
    {
        for (int i = 1; i < 9; i++)
        {
            string price = GetItemPrice(PlayerPrefs.GetString("Shelf Prize: " + (i-1).ToString()));
            if (price != "")
            {
                cornerDisplay.transform.Find("group" + i).gameObject.SetActive(true);
                cornerDisplay.transform.Find((i - 1).ToString()).transform.GetChild(0).GetComponent<Text>().text = "T " + price;
                cornerDisplay.transform.Find("group" + i).GetComponent<RawImage>().texture = Resources.Load<Texture>("Merch/Icons/" + PlayerPrefs.GetString("Shelf Prize: " + (i - 1).ToString()));
            }
            else
            {
                cornerDisplay.transform.Find("group" + i).gameObject.SetActive(false);
                cornerDisplay.transform.Find((i - 1).ToString()).transform.GetChild(0).GetComponent<Text>().text = "OUT";
            }
        }
    }

    string GetItemPrice(string name)
    {
        for (int e = 0; e < gc.prizeStrings.Length; e++)
        {
            for (int i = 0; i < gc.prizeStrings[e].prizeStrings.Length; i++)
            {
                if (gc.prizeStrings[e].prizeStrings[i].name == name)
                {
                    return gc.prizeStrings[e].prizeStrings[i].price;
                }
            }
        }
        return "";
    }
    void GatherNewPrizes(ref string[] strang)
    {
        strang = new string[8];
        List<string> list = new List<string>();
        for (int e = 0; e < gc.prizeStrings.Length; e++)
        {
            for (int i = 0; i < gc.prizeStrings[e].prizeStrings.Length; i++)
            {
                if (PlayerPrefs.GetInt(gc.prizeStrings[e].prizeStrings[i].name) != 64)
                {
                   list.Add(gc.prizeStrings[e].prizeStrings[i].name);
                }
            }
        }
        if(list.Count > 0)
        {
            List<int> picked = new List<int>();
            for (int i = 0; i < strang.Length; i++)
            {
                if (picked.Count < list.Count)
                {
                    int pick = 0;
                    while (true)
                    {
                        pick = UnityEngine.Random.Range(0, list.Count);
                        bool determine = false;
                        for (int e = 0; e < picked.Count; e++)
                        {
                            if (picked[e] == pick)
                            {
                                determine = true;
                            }
                        }
                        if (!determine)
                        {
                            break;
                        }
                    }
                    strang[i] = list[pick];
                    picked.Add(pick);
                }
            }
        }
    }

    void CheckAndCreatePrizes()
    {
        int currentDay = System.DateTime.Now.DayOfYear;
        bool createPrizes = false;
        if (!PlayerPrefs.HasKey("CurrentPrizeDayTimeFA"))
        {
            PlayerPrefs.SetInt("CurrentPrizeDayTimeFA", currentDay);
            Debug.Log("No current prize day. Setting to " + currentDay);
            createPrizes = true;
        }
        else if (PlayerPrefs.GetInt("CurrentPrizeDayTimeFA") != currentDay)
        {
            Debug.Log("New Prize day. Previous day was " + PlayerPrefs.GetInt("CurrentPrizeDayTimeFA") + ". Current day is now " + currentDay);
            PlayerPrefs.SetInt("CurrentPrizeDayTimeFA", currentDay);
            createPrizes = true;
        }
        else
        {
            Debug.Log("Same Prize day. Today is " + PlayerPrefs.GetInt("CurrentPrizeDayTimeFA") + ".");
        }

        if (createPrizes)
        {
            GatherNewPrizes(ref shelfPrizes);
            for (int i = 0; i < shelfPrizes.Length; i++)
            {
                Debug.Log("Prize #" + i + " - " + shelfPrizes[i]);
                PlayerPrefs.SetString("Shelf Prize: " + i, shelfPrizes[i]);
            }   
        }
    }
  
    public IEnumerator LoadSavedData()
    {
        for (int i = 0; i < shelfPrizes.Length; i++)
        {
           shelfPrizes[i] = PlayerPrefs.GetString("Shelf Prize: " + i);
        }
        prizesLoaded = true;
        yield return null;
    }
}
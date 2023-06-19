using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelineBitVis : MonoBehaviour
{
    public UI_ShowtapeManager uiShowtapeManager;
    public TimelineEditor timelineEditor;
    public GameObject bitPrefab;
    public GameObject holderPrefab;
    public GameObject[] holder;
    int maximumHolders = 10;

    public void RepaintBitGroups()
    {
        uiShowtapeManager.inputHandler.editorKeys = new UI_WindowMaker.MovementRecordings();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).gameObject.activeSelf)
            {
                Destroy(this.transform.GetChild(i).gameObject);
            }
        }
        holder = new GameObject[Mathf.Min(timelineEditor.tlRecordGroup.Length, maximumHolders)];
        List<UI_WindowMaker.inputNames> temp = new List<UI_WindowMaker.inputNames>();
        for (int i = 0; i < Mathf.Min(timelineEditor.tlRecordGroup.Length, maximumHolders); i++)
        {
            if(timelineEditor.holderOffset + i < timelineEditor.tlRecordGroup.Length)
            {
                temp.Add(new UI_WindowMaker.inputNames());
                temp[temp.Count - 1].index = new int[1];
                if (timelineEditor.tlRecordGroup[i + timelineEditor.holderOffset].bit > 150)
                {
                    temp[temp.Count - 1].drawer = true;
                    temp[temp.Count - 1].index[0] = i + timelineEditor.holderOffset - 149;
                }
                else
                {
                    temp[temp.Count - 1].index[0] = i + timelineEditor.holderOffset + 1;
                }

                holder[i] = GameObject.Instantiate(holderPrefab, this.transform);
                holder[i].SetActive(true);
                RectTransform rect = holder[i].transform as RectTransform;
                rect.anchoredPosition = new Vector3(rect.anchoredPosition.x, rect.anchoredPosition.y - (i * 40));
                int temper = i + 1;
                if(temper == 10)
                {
                    temper = 0;
                }
                holder[i].transform.Find("Bit Header").Find("Name").GetComponent<Text>().text = "(" + temper + ") " + timelineEditor.windowMaker.SearchBitChartName(timelineEditor.tlRecordGroup[i + timelineEditor.holderOffset].bit);
                holder[i].GetComponent<TimelineScreen>().rCBitBarNumber = i;
                holder[i] = holder[i].transform.Find("Holder").gameObject;
            }
        }
        uiShowtapeManager.inputHandler.editorKeys.inputNames = temp.ToArray();
    }

    public void RepaintTimeline(float viewzoomMin, float viewZoomMax, float audioLengthMax)
    {
        for (int i = 0; i < timelineEditor.tlRecordGroup.Length; i++)
        {
            timelineEditor.tlRecordGroup[i].checkedObject = false;
            timelineEditor.tlRecordGroup[i].currentBit = null;
        }
        for (int i = 0; i < holder.Length; i++)
        {
            if(holder[i] != null)
            {
                foreach (Transform child in holder[i].transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }
        int bitStart = Mathf.RoundToInt(viewzoomMin * 60.0f);
        int bitsRepresented = Mathf.RoundToInt((viewZoomMax - viewzoomMin) * 60.0f);

        int secondIndex = 0;
        //Loop all frames in view
        while (true)
        {
            //Function finished
            if (secondIndex >= bitsRepresented)
            {
                break;
            }
            if (bitStart + secondIndex < uiShowtapeManager.rshwData.Length)
            {
                for (int i = 0; i < holder.Length; i++)
                {
                    if (holder[i] != null && i + timelineEditor.holderOffset < timelineEditor.tlRecordGroup.Length)
                    {
                        //If bit is true on the current frame
                        if (uiShowtapeManager.rshwData[bitStart + secondIndex].Get(timelineEditor.tlRecordGroup[i + timelineEditor.holderOffset].bit-1))
                        {
                            if (timelineEditor.tlRecordGroup[i + timelineEditor.holderOffset].currentBit == null)
                            {
                                CreateButton(viewzoomMin, viewZoomMax, audioLengthMax, secondIndex, i);
                            }
                            else
                            {
                                UpdateButton(false, viewzoomMin, viewZoomMax, i);
                            }
                        }
                    }
                }
                for (int i = 0; i < holder.Length; i++)
                {
                    UpdateButton(true, viewzoomMin, viewZoomMax, i);
                }
            }
            secondIndex++;
        }
    }

    void CreateButton(float viewzoomMin, float viewZoomMax, float audioLengthMax, int secondIndex, int recGroupIndex)
    {
        if(holder[recGroupIndex] != null)
        {
            timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].currentBit = GameObject.Instantiate(bitPrefab, holder[recGroupIndex].transform);
            timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].currentBit.SetActive(true);
            RectTransform rect = timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].currentBit.transform as RectTransform;
            rect.position = new Vector3((remap(viewzoomMin + (secondIndex / 60.0f), viewzoomMin, viewZoomMax, 0, 1) * (Screen.width)), rect.position.y, 0);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x + ((Screen.width * (1080.0f / Screen.height)) / ((viewZoomMax - viewzoomMin) * uiShowtapeManager.dataStreamedFPS)), rect.sizeDelta.y);
            timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].checkedObject = true;
        }
    }
    public float remap(float val, float in1, float in2, float out1, float out2)
    {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }

    void UpdateButton(bool checkOnly, float viewzoomMin, float viewZoomMax, int recGroupIndex)
    {
        if (holder[recGroupIndex] != null)
        {
            if (checkOnly)
            {
                if (!timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].checkedObject)
                {
                    timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].currentBit = null;
                }
                timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].checkedObject = false;
            }
            else
            {
                RectTransform rect = timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].currentBit.transform as RectTransform;
                rect.sizeDelta = new Vector2(rect.sizeDelta.x + ((Screen.width * (1080.0f / Screen.height)) / ((viewZoomMax - viewzoomMin) * uiShowtapeManager.dataStreamedFPS)), rect.sizeDelta.y);
                timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].checkedObject = true;
            }
        }
    }
}

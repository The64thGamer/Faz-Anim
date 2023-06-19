using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public int valveMapping;
    public GameObject UI;
    UI_WindowMaker programmingGroups;
    InputDataObj frameInputs;

    [HideInInspector] public bool[] keypadInvert = new bool[33];
    int invertcooldown = 0;

    public UI_WindowMaker.MovementRecordings editorKeys = new UI_WindowMaker.MovementRecordings();

    // Update is called once per frame
    private void Awake()
    {
        editorKeys.inputNames = new UI_WindowMaker.inputNames[0];
        frameInputs = new InputDataObj();
        programmingGroups = UI.GetComponent<UI_WindowMaker>();
    }
    public InputDataObj InputCheck()
    {
        frameInputs.anyButtonHeld = false;
        frameInputs.topDrawer = new bool[150];
        frameInputs.bottomDrawer = new bool[150];
        if (valveMapping > 0)
        {
            for (int i = 0; i < programmingGroups.recordingGroups[valveMapping - 1].inputNames.Length; i++)
            {
                if (programmingGroups.recordingGroups[valveMapping - 1].inputNames[i].index[0] > 0)
                {
                    bool applyKey = false;

                    switch (i)
                    {
                        case 0:
                            if (Input.GetKey(KeyCode.Alpha4)) { applyKey = true; }
                            break;
                        case 1:
                            if (Input.GetKey(KeyCode.R)) { applyKey = true; }
                            break;
                        case 2:
                            if (Input.GetKey(KeyCode.F)) { applyKey = true; }
                            break;
                        case 3:
                            if (Input.GetKey(KeyCode.V)) { applyKey = true; }
                            break;
                        case 4:
                            if (Input.GetKey(KeyCode.Alpha5)) { applyKey = true; }
                            break;
                        case 5:
                            if (Input.GetKey(KeyCode.T)) { applyKey = true; }
                            break;
                        case 6:
                            if (Input.GetKey(KeyCode.G)) { applyKey = true; }
                            break;
                        case 7:
                            if (Input.GetKey(KeyCode.B)) { applyKey = true; }
                            break;
                        case 8:
                            if (Input.GetKey(KeyCode.Alpha6)) { applyKey = true; }
                            break;
                        case 9:
                            if (Input.GetKey(KeyCode.Y)) { applyKey = true; }
                            break;
                        case 10:
                            if (Input.GetKey(KeyCode.H)) { applyKey = true; }
                            break;
                        case 11:
                            if (Input.GetKey(KeyCode.N)) { applyKey = true; }
                            break;
                        case 12:
                            if (Input.GetKey(KeyCode.Alpha7)) { applyKey = true; }
                            break;
                        case 13:
                            if (Input.GetKey(KeyCode.U)) { applyKey = true; }
                            break;
                        case 14:
                            if (Input.GetKey(KeyCode.J)) { applyKey = true; }
                            break;
                        case 15:
                            if (Input.GetKey(KeyCode.M)) { applyKey = true; }
                            break;
                        case 16:
                            if (Input.GetKey(KeyCode.Alpha8)) { applyKey = true; }
                            break;
                        case 17:
                            if (Input.GetKey(KeyCode.I)) { applyKey = true; }
                            break;
                        case 18:
                            if (Input.GetKey(KeyCode.K)) { applyKey = true; }
                            break;
                        case 19:
                            if (Input.GetKey(KeyCode.Comma)) { applyKey = true; }
                            break;
                        case 20:
                            if (Input.GetKey(KeyCode.Alpha9)) { applyKey = true; }
                            break;
                        case 21:
                            if (Input.GetKey(KeyCode.O)) { applyKey = true; }
                            break;
                        case 22:
                            if (Input.GetKey(KeyCode.L)) { applyKey = true; }
                            break;
                        case 23:
                            if (Input.GetKey(KeyCode.Period)) { applyKey = true; }
                            break;
                        case 24:
                            if (Input.GetKey(KeyCode.Alpha0)) { applyKey = true; }
                            break;
                        case 25:
                            if (Input.GetKey(KeyCode.P)) { applyKey = true; }
                            break;
                        case 26:
                            if (Input.GetKey(KeyCode.Semicolon)) { applyKey = true; }
                            break;
                        case 27:
                            if (Input.GetKey(KeyCode.Slash)) { applyKey = true; }
                            break;
                        case 28:
                            if (Input.GetKey(KeyCode.Minus)) { applyKey = true; }
                            break;
                        case 29:
                            if (Input.GetKey(KeyCode.LeftBracket)) { applyKey = true; }
                            break;
                        case 30:
                            if (Input.GetKey(KeyCode.Quote)) { applyKey = true; }
                            break;
                        case 31:
                            if (Input.GetKey(KeyCode.Plus)) { applyKey = true; }
                            break;
                        case 32:
                            if (Input.GetKey(KeyCode.RightBracket)) { applyKey = true; }
                            break;
                    }
                    //Control Invert Check
                    if (invertcooldown > 0)
                    {
                        invertcooldown--;
                    }
                    if (Input.GetKey(KeyCode.LeftControl) && applyKey && invertcooldown == 0)
                    {
                        invertcooldown = 20 * programmingGroups.recordingGroups[valveMapping - 1].inputNames.Length;
                        keypadInvert[i] = !keypadInvert[i];
                    }
                    if (keypadInvert[i])
                    {
                        applyKey = !applyKey;
                    }
                    if (Input.GetKey(KeyCode.LeftControl) && invertcooldown != 0)
                    {
                        applyKey = false;
                    }


                    //Apply bool
                    if (!programmingGroups.recordingGroups[valveMapping - 1].inputNames[i].drawer)
                    {
                        //Top
                        if (applyKey)
                        {
                            frameInputs.anyButtonHeld = true;
                            for (int e = 0; e < programmingGroups.recordingGroups[valveMapping - 1].inputNames[i].index.Length; e++)
                            {
                                frameInputs.topDrawer[programmingGroups.recordingGroups[valveMapping - 1].inputNames[i].index[e] - 1] = true;
                            }
                        }
                    }
                    else
                    {
                        //Bottom
                        if (applyKey)
                        {
                            frameInputs.anyButtonHeld = true;
                            for (int e = 0; e < programmingGroups.recordingGroups[valveMapping - 1].inputNames[i].index.Length; e++)
                            {
                                frameInputs.bottomDrawer[programmingGroups.recordingGroups[valveMapping - 1].inputNames[i].index[e] - 1] = true;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < editorKeys.inputNames.Length; i++)
            {
                if (editorKeys.inputNames[i].index[0] > 0)
                {
                    bool applyKey = false;

                    switch (i)
                    {
                        case 0:
                            if (Input.GetKey(KeyCode.Alpha1)) { applyKey = true; }
                            break;
                        case 1:
                            if (Input.GetKey(KeyCode.Alpha2)) { applyKey = true; }
                            break;
                        case 2:
                            if (Input.GetKey(KeyCode.Alpha3)) { applyKey = true; }
                            break;
                        case 3:
                            if (Input.GetKey(KeyCode.Alpha4)) { applyKey = true; }
                            break;
                        case 4:
                            if (Input.GetKey(KeyCode.Alpha5)) { applyKey = true; }
                            break;
                        case 5:
                            if (Input.GetKey(KeyCode.Alpha6)) { applyKey = true; }
                            break;
                        case 6:
                            if (Input.GetKey(KeyCode.Alpha7)) { applyKey = true; }
                            break;
                        case 7:
                            if (Input.GetKey(KeyCode.Alpha8)) { applyKey = true; }
                            break;
                        case 8:
                            if (Input.GetKey(KeyCode.Alpha9)) { applyKey = true; }
                            break;
                        case 9:
                            if (Input.GetKey(KeyCode.Alpha0)) { applyKey = true; }
                            break;
                    }
                    //Control Invert Check
                    if (invertcooldown > 0)
                    {
                        invertcooldown--;
                    }
                    if (Input.GetKey(KeyCode.LeftControl) && applyKey && invertcooldown == 0)
                    {
                        invertcooldown = 20 * editorKeys.inputNames.Length;
                        keypadInvert[i] = !keypadInvert[i];
                    }
                    if (keypadInvert[i])
                    {
                        applyKey = !applyKey;
                    }
                    if (Input.GetKey(KeyCode.LeftControl) && invertcooldown != 0)
                    {
                        applyKey = false;
                    }


                    //Apply bool
                    if (!editorKeys.inputNames[i].drawer)
                    {
                        //Top
                        if (applyKey)
                        {
                            frameInputs.anyButtonHeld = true;
                            for (int e = 0; e < editorKeys.inputNames[i].index.Length; e++)
                            {
                                frameInputs.topDrawer[editorKeys.inputNames[i].index[e] - 1] = true;
                            }
                        }
                    }
                    else
                    {
                        //Bottom
                        if (applyKey)
                        {
                            frameInputs.anyButtonHeld = true;
                            for (int e = 0; e < editorKeys.inputNames[i].index.Length; e++)
                            {
                                frameInputs.bottomDrawer[editorKeys.inputNames[i].index[e] - 1] = true;
                            }
                        }
                    }
                }
            }
        }
        if(valveMapping == 0)
        {
            for (int i = 0; i < keypadInvert.Length; i++)
            {
                keypadInvert[i] = false;
            }
        }

        return frameInputs;
    }
}
[System.Serializable]
public class InputDataObj
{
    public bool[] topDrawer;
    public bool[] bottomDrawer;
    public bool anyButtonHeld;
}
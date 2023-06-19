using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharIconSelectButton : MonoBehaviour
{
    public int num;
    public ButtonType bt;
    public enum ButtonType
    {
        charIcon,
        edit,
        assignp1,
        assignp2,
    }
    public void SelectButton()
    {
        UI_ModelMaker[] components = GameObject.FindObjectsOfType<UI_ModelMaker>();
        switch (bt)
        {
            case ButtonType.charIcon:
                components[0].NewSelection(num);
                break;
            case ButtonType.edit:
                break;
            case ButtonType.assignp1:
                PlayerPrefs.SetInt("Player1: Preset", components[0].currentSelection);
                break;
            case ButtonType.assignp2:
                PlayerPrefs.SetInt("Player2: Preset", components[0].currentSelection);
                break;
            default:
                break;
        }
        
    }
}

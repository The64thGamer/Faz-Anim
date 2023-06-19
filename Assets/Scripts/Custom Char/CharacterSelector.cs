using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSelector
{
    public GameObject mainCharacter;
    public string characterName;
    public Sprite icon;
    public int currentCostume;
    public CharacterCostume[] allCostumes;

    public void SwapCharacter(int costumeIndex)
    {
        currentCostume = costumeIndex;
    }
}
[System.Serializable]
public class CharacterCostume
{
    public string costumeName;
    public string costumeDesc;
    public Sprite costumeIcon;
    public string yearOfCostume;
    public CostumeT costumeType;
    public Vector3 offsetPos;
    //This is if a costume has a different rig.
    public enum CostumeT
    {
        Costume,
        Retrofit,
        Innards,
    }
}

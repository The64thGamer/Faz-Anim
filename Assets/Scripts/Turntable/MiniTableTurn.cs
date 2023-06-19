using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniTableTurn : MonoBehaviour
{
    public GameObject holder;
    public string character;
    GameObject characterObj;
    public float offset;

    void LateUpdate()
    {
        if (characterObj != null)
        {
            characterObj.transform.eulerAngles = new Vector3(characterObj.transform.eulerAngles.x, this.transform.eulerAngles.y + offset, characterObj.transform.eulerAngles.z);
            characterObj.transform.position = new Vector3(this.transform.position.x, characterObj.transform.position.y, this.transform.position.z);
        }
        else
        {
            Transform gg = holder.transform.Find(character);
            if(gg != null)
            {
                characterObj = gg.gameObject;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCharacter : MonoBehaviour
{
    public GameObject holder;
    public string character;
    GameObject characterObj;
    public Vector3 offset;
    float damping = 2;


    void Update()
    {
        if (characterObj != null)
        {
            Vector3 lookPos = (characterObj.transform.position + offset) - transform.position;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        }
        else
        {
            Transform gg = holder.transform.Find(character);
            if (gg != null)
            {
                characterObj = gg.gameObject;
            }
        }
    }
}

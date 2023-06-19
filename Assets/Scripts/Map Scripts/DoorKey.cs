using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DoorKey : MonoBehaviour
{
    public string keyPref;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            PlayerPrefs.SetInt(keyPref, 1);
        }
    }
}

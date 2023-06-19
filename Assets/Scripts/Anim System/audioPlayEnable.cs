using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioPlayEnable : MonoBehaviour
{
    private void OnEnable()
    {
        this.GetComponent<AudioSource>().Play();
    }
}

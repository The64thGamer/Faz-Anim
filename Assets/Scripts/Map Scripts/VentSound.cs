using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentSound : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 11)
        {
            AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
            sc.clip = (AudioClip)Resources.Load("VentHit");
            sc.Play();
        }
    }
}

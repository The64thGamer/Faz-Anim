using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    Mack_Valves bitChart;
    public List<int> soundBits;
    public List<AudioClip> soundClips;
    public List<bool> boolChecks;
    AudioSource aud;

    // Start is called before the first frame update
    void Start()
    {
        bitChart = GameObject.Find("Mack Valves").GetComponent<Mack_Valves>();
        aud = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void Update()
    {
        for (int i = 0; i < soundBits.Count; i++)
        {
            if(bitChart.topDrawer[soundBits[i] - 1])
            {
                if (!boolChecks[i])
                {
                    aud.PlayOneShot(soundClips[i]);
                    boolChecks[i] = true;
                }
            }
            else
            {
                boolChecks[i] = false;
            }    
        }

    }
}

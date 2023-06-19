using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSetter : MonoBehaviour

{
    public GameObject thePlayer;
    public int mapping;

    // Start is called before the first frame update
    void OnEnable()
    {
        thePlayer.GetComponent<InputHandler>().valveMapping = mapping;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instaspawn : MonoBehaviour
{
    public GameObject[] spawn;

    void Awake()
    {
        for (int i = 0; i < spawn.Length; i++)
        {
            spawn[i].SetActive(true);
        }
    }
}

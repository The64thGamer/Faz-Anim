using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeChildObject : MonoBehaviour
{
    public GameObject tracker;
    void Update()
    {
        this.transform.position = tracker.transform.position;
    }
}

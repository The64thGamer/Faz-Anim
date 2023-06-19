using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckScript : MonoBehaviour
{
    Vector3 old;
    void Start()
    {
        old = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float newtime = Mathf.Min(Time.deltaTime, 0.03f);
        this.transform.position = new Vector3(Mathf.Clamp(this.transform.position.x + (Random.RandomRange(-0.01f, 0.01f)* newtime * 60), old.x - 0.5f, old.x + 0.5f), this.transform.position.y, Mathf.Clamp(this.transform.position.z + (Random.RandomRange(-0.01f, 0.01f) * newtime * 60), old.z - 0.5f, old.z + 0.5f));
    }
}

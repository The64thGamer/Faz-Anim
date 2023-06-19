using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSound : MonoBehaviour
{
    float timeOpen;
    public float zRotationAwake;
    bool stillShut = true;
    HingeJoint hj;

    private void Awake()
    {
        hj = this.GetComponent<HingeJoint>();
        zRotationAwake = hj.angle;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (this.GetComponent<Rigidbody>().constraints == RigidbodyConstraints.None && timeOpen < Time.time - 1 && collision.gameObject.layer == 11)
        {
            stillShut = false;
            timeOpen = Time.time;
            AudioSource source = this.GetComponent<AudioSource>();
            int sound = Random.Range(0, 4);
            AudioClip clip = Resources.Load("DoorOpen0" + sound) as AudioClip;
            source.clip = clip;
            source.pitch = Random.Range(0.98f, 1.02f);
            source.Play();
        }
    }

    private void FixedUpdate()
    {
        if(!stillShut && timeOpen < Time.time - 1 && hj.angle < zRotationAwake + 2 && hj.angle > zRotationAwake - 2)
        {
            stillShut = true;
            AudioSource source = this.GetComponent<AudioSource>();
            int sound = Random.Range(0, 4);
            AudioClip clip = Resources.Load("DoorClose0" + sound) as AudioClip;
            source.clip = clip;
            source.pitch = Random.Range(0.98f, 1.02f);
            source.Play();
        }
    }
}

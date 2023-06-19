using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLock : MonoBehaviour
{
    private Rigidbody rb;
    public string lockKeyPref;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        if(PlayerPrefs.GetInt(lockKeyPref) != 1)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 11)
        {
            
            if (PlayerPrefs.GetInt(lockKeyPref) != 1)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                rb.constraints = RigidbodyConstraints.None;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirthdayCheck : MonoBehaviour
{
    private void Awake()
    {
        if (System.DateTime.Now.DayOfYear != 40)
        {
            Destroy(this.gameObject);
        }
    }
}

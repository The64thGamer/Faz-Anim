using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBall : MonoBehaviour
{
    public PoolTable table;
    public bool firstHit;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Board")
        {
            firstHit = false;
        }
        else
        {
            if(firstHit)
            {
                firstHit = false;
                table.PlaySound(2);
            }
            else
            {
                table.PlaySound(1);
            }
        }
    }
}

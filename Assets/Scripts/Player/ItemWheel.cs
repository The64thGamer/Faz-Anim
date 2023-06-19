using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWheel : MonoBehaviour
{
    public Player player;

    public void GatherIcons()
    {
        player.GatherItemWheelIcons();
    }

    public void Disable()
    {
        this.GetComponent<Animator>().Play("New State");
        this.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SittingScript : MonoBehaviour
{
    public GameObject exitPoint;
    bool isSitting;
    private Player player;
    int playernum = 0;

    private void LateUpdate()
    {
        if(isSitting)
        {
            if (playernum == 1)
            {
                if (player.GPJoy != Vector2.zero)
                {
                    isSitting = false;
                    player.transform.position = exitPoint.transform.position;
                    player.GetComponent<CharacterController>().enabled = true;
                }
            }
            if (playernum == 0)
            {
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl))
                {
                    isSitting = false;
                    player.transform.position = exitPoint.transform.position;
                    player.GetComponent<CharacterController>().enabled = true;
                }
            }
        }
    }
    public void StartSit(int input)
    {
        if (!isSitting)
        {
            if (input == 0)
            {
                player = GameObject.Find("Player").GetComponent<Player>();
                playernum = 0;
            }
            else
            {
                player = GameObject.Find("Player 2").GetComponent<Player>();
                playernum = 1;
            }
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = this.transform.position;

            isSitting = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraItem : MonoBehaviour
{
    public Texture2D image;
    private void Awake()
    {
        if (PlayerPrefs.GetInt("Item: Camera") == 1)
        {
            Destroy(this.gameObject);
        }
    }

    public void Pickup(int input)
    {
        Player player = GameObject.Find("Player").GetComponent<Player>();
        Player player2;
        if (input == 0)
        {
            player.playerState = Player.PlayerState.frozenAllUnlock;
            player.DisplayMessage(image, (byte)150, (byte)5);
        }
        else
        {
            player2 = GameObject.Find("Player 2").GetComponent<Player>();
            player2.playerState = Player.PlayerState.frozenAllUnlock;
            player2.DisplayMessage(image, (byte)150, (byte)5);
        }
        PlayerPrefs.SetInt("Item: Camera", 1); 
        Destroy(this.gameObject);
    }
}

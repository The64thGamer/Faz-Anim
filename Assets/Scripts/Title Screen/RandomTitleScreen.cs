using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomTitleScreen : MonoBehaviour
{
    public Texture2D[] characters;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<RawImage>().texture = characters[Random.Range(0, characters.Length)];
        Destroy(this.GetComponent<RandomTitleScreen>());
        if (GameVersion.gameName != "Faz-Anim")
        {
            Destroy(this.gameObject);
        }
    }

}

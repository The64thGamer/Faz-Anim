using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider))]
public class SlidePuzzleButton : MonoBehaviour
{
    public GameObject puzzleMasterObj;
    public int puzzleNum;
    bool click;
    bool highlighted;
    AudioSource sc;

    public enum uisound
    {
        tap,
        bigTap,
        ting,
        none
    }
    public uisound uiSound = uisound.tap;
    void Start()
    {
        sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
    }
    public void Highlight(string name)
    {
        highlighted = true;
    }
    public void StartClick(string name)
    {
        click = true;
    }
    public void EndClick(string name)
    {
        if (click)
        {
            switch (uiSound)
            {
                case uisound.tap:
                    sc.clip = (AudioClip)Resources.Load("tap");
                    break;
                case uisound.bigTap:
                    sc.clip = (AudioClip)Resources.Load("big tap");
                    break;
                case uisound.ting:
                    sc.clip = (AudioClip)Resources.Load("ting");
                    break;
                case uisound.none:
                    sc.clip = null;
                    break;
                default:
                    break;
            }
            sc.pitch = Random.Range(0.95f, 1.05f);
            sc.Play();
            click = false;
            if(puzzleNum != -1)
            {
                puzzleMasterObj.GetComponent<SlidePuzzle>().Slide(puzzleNum);
            }
            else
            {
                puzzleMasterObj.GetComponent<SlidePuzzle>().Shuffle();
            }
        }
    }
}

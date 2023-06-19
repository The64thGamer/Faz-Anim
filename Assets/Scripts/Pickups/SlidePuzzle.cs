using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidePuzzle : MonoBehaviour
{
    public List<int> tiles;
    public List<GameObject> tileObjs;
    public GameObject currentSlide;
    public Vector3 slideDir;
    public Vector3 oldSlidePos;
    public float slideTime;
    public bool shuffleTile;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 16; i++)
        {
            tiles.Add(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(shuffleTile)
        {
            Slide(UnityEngine.Random.Range(0, 15));
        }
        if(currentSlide != null)
        {
            slideTime += Time.deltaTime;
            if(slideTime > .1f)
            {
                slideTime = .1f;
            }    
            currentSlide.transform.localPosition = Vector3.Lerp(oldSlidePos, slideDir, slideTime*10);
            if(slideTime == .1f)
            {
                currentSlide = null;
                slideTime = 0;
            }
        }
    }

    public void Slide(int puzzleiD)
    {
        if (slideTime == 0)
        {
            int puzzlePiece = -1;
            int i = -1;
            while (i != puzzleiD)
            {
                puzzlePiece++;
                i = tiles[puzzlePiece];
            }
            //Left Check
            if (puzzlePiece != 0 && puzzlePiece != 4 && puzzlePiece != 8 && puzzlePiece != 12)
            {
                if (tiles[puzzlePiece - 1] == 0)
                {
                    oldSlidePos = tileObjs[puzzleiD].transform.localPosition;
                    currentSlide = tileObjs[puzzleiD];
                    slideTime = 0;
                    slideDir = new Vector3(0.0001321f, 0) + oldSlidePos;
                    tiles[puzzlePiece - 1] = tiles[puzzlePiece];
                    tiles[puzzlePiece] = 0;
                }
            }
            //Right Check
            if (puzzlePiece != 3 && puzzlePiece != 7 && puzzlePiece != 11 && puzzlePiece != 15)
            {
                if (tiles[puzzlePiece + 1] == 0)
                {
                    oldSlidePos = tileObjs[puzzleiD].transform.localPosition;
                    currentSlide = tileObjs[puzzleiD];
                    slideTime = 0;
                    slideDir = new Vector3(-0.0001321f, 0) + oldSlidePos;
                    tiles[puzzlePiece + 1] = tiles[puzzlePiece];
                    tiles[puzzlePiece] = 0;
                }
            }
            //Up Check
            if (puzzlePiece != 0 && puzzlePiece != 1 && puzzlePiece != 2 && puzzlePiece != 3)
            {
                if (tiles[puzzlePiece - 4] == 0)
                {
                    oldSlidePos = tileObjs[puzzleiD].transform.localPosition;
                    currentSlide = tileObjs[puzzleiD];
                    slideTime = 0;
                    slideDir = new Vector3(0, 0.0001321f) + oldSlidePos;
                    tiles[puzzlePiece - 4] = tiles[puzzlePiece];
                    tiles[puzzlePiece] = 0;
                }
            }
            //Down Check
            if (puzzlePiece != 12 && puzzlePiece != 13 && puzzlePiece != 14 && puzzlePiece != 15)
            {
                if (tiles[puzzlePiece + 4] == 0)
                {
                    oldSlidePos = tileObjs[puzzleiD].transform.localPosition;
                    currentSlide = tileObjs[puzzleiD];
                    slideTime = 0;
                    slideDir = new Vector3(0, -0.0001321f) + oldSlidePos;
                    tiles[puzzlePiece + 4] = tiles[puzzlePiece];
                    tiles[puzzlePiece] = 0;
                }
            }
        }
    }

    public void Shuffle()
    {
        shuffleTile = !shuffleTile;
    }
}

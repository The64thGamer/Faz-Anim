using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarMove : MonoBehaviour
{
    public bool transition;
    bool transitionOld;
    public float startY = 0.5f;
    public float endY = 0f;
    public float randomSpeedMin;
    public float randomSpeedMax;
    float speed;
    RawImage rect;

    private void Start()
    {
        rect = this.GetComponent<RawImage>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(transitionOld != transition)
        {
            transitionOld = transition;
            speed = Random.Range(randomSpeedMin, randomSpeedMax);
        }

        if(transition && rect.uvRect.y > endY)
        { 
            rect.uvRect = new Rect(0, rect.uvRect.y - speed, rect.uvRect.width, rect.uvRect.height);
        }   
        else if (!transition && rect.uvRect.y < startY)
        {
            rect.uvRect = new Rect(0, rect.uvRect.y + speed, rect.uvRect.width, rect.uvRect.height);
        }
    }
}

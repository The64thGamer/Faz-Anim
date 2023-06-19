using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerButton : MonoBehaviour
{
    TutorialStart gc;
    public LayerMask hitMask;
    public string attemptString;
    bool waitwaitwait;

    void Awake()
    {
        gc = GameObject.Find("Tutorial").GetComponent<TutorialStart>();
    }

    private void FixedUpdate()
    {
        if (waitwaitwait)
        {
            if (gc.AttemptAdvanceTutorial(attemptString))
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitMask == (hitMask | (1 << other.gameObject.layer)))
        {
            if (gc.AttemptAdvanceTutorial(attemptString))
            {
                Destroy(this.gameObject);
            }
            else
            {
                waitwaitwait = true;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonListButton : MonoBehaviour
{
    [SerializeField]
    private ButtonListControl buttonControl;

    public void StartAwake()
    {
        StartCoroutine(CollectIcon());
    }

    public void Click()
    {
        buttonControl.CreateAttributePage(this.name);
    }

    IEnumerator CollectIcon()
    { 
        Debug.Log(this.name + "Icon Loading");
        ResourceRequest rr = Resources.LoadAsync<Sprite>("Merch/Icons/" + this.name);
        while (!rr.isDone)
        {
            yield return null;
        }
        Debug.Log(this.name + "Icon Loaded");
        this.transform.GetChild(0).GetComponent<Image>().sprite = rr.asset as Sprite;
    }
}

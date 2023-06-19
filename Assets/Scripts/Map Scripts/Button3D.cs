using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent (typeof(BoxCollider))]
public class Button3D : MonoBehaviour
{
    public GameObject ui;
    public string funcName;
    public int funcWindow;
    float highlightime;
    bool click;
    bool highlighted;
    AudioSource sc;
    RectTransform localrect;
    public bool sendPlayerNum = false;
    public float clickTime = 0;
    public string buttonText;

    public bool sendToTutorial;
    public string tutorialSend;
    TutorialStart tut;

    public enum uisound
    {
        tap,
        bigTap,
        ting,
        help,
        buy,
        none,
        systemOpen,
        systemClose,
        deny,
        create,
        unboxCrate,
        sitDown,
    }
    public uisound uiSound = uisound.tap;
    public bool ignoreCollider = false;

    void Awake()
    {
        if(sendToTutorial)
        {
            GameObject tt = GameObject.Find("Tutorial");
            if(tt != null)
            {
                tut = tt.GetComponent<TutorialStart>();
            }  
        }
        if (!ignoreCollider)
        {
            this.GetComponent<BoxCollider>().size = new Vector3(this.GetComponent<RectTransform>().sizeDelta.x, this.GetComponent<RectTransform>().sizeDelta.y, 0f);
            localrect = this.GetComponent<RectTransform>();
        }   
        sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(click)
        {
            clickTime += Time.deltaTime;
        }    
        if(clickTime > 0.2f)
        {
            clickTime = 0;
            click = false;
        }
        if(highlighted)
        {
            highlightime += 0.1f;
        }
        else
        {
            highlightime -= 0.1f;
        }
        highlighted = false;
        highlightime = Mathf.Clamp(highlightime, 0, 1);
        if (!ignoreCollider)
        {
            localrect.localScale = new Vector3(Mathf.Max(0,1.0f - (highlightime / 20f)), Mathf.Max(0, 1.0f - (highlightime / 20f)),1);
        }
    }

    private void OnDisable()
    {
        click = false;
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
        if(click)
        {
            if (funcName != "")
            {
                if (sc == null)
                {
                    sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
                }
                switch (uiSound)
                {
                    case uisound.tap:

                        sc.clip = (AudioClip)Resources.Load("tap");
                        sc.pitch = Random.Range(0.95f, 1.05f);
                        break;
                    case uisound.bigTap:
                        sc.clip = (AudioClip)Resources.Load("big tap");
                        sc.pitch = Random.Range(0.98f, 1.02f);
                        break;
                    case uisound.ting:
                        sc.clip = (AudioClip)Resources.Load("ting");
                        sc.pitch = Random.Range(0.95f, 1.05f);
                        break;
                    case uisound.help:
                        sc.clip = (AudioClip)Resources.Load("help");
                        sc.pitch = 1.0f;
                        break;
                    case uisound.buy:
                        sc.clip = (AudioClip)Resources.Load("Purchase");
                        sc.pitch = 1.0f;
                        break;
                    case uisound.none:
                        sc.clip = null;
                        break;
                    case uisound.systemOpen:
                        sc.clip = (AudioClip)Resources.Load("SystemOpen");
                        sc.pitch = 1.0f;
                        break;
                    case uisound.systemClose:
                        sc.clip = (AudioClip)Resources.Load("SystemClose");
                        sc.pitch = 1.0f;
                        break;
                    case uisound.deny:
                        sc.clip = (AudioClip)Resources.Load("Deny");
                        sc.pitch = 1.0f;
                        break;
                    case uisound.create:
                        sc.clip = (AudioClip)Resources.Load("Create");
                        sc.pitch = 1.0f;
                        break;
                    case uisound.unboxCrate:
                        sc.clip = (AudioClip)Resources.Load("Crate Unbox");
                        sc.pitch = 1.0f;
                        break;
                    case uisound.sitDown:
                        sc.clip = (AudioClip)Resources.Load("Sit Down");
                        sc.pitch = 1.0f;
                        break;
                    default:
                        break;
                }
                sc.Play();
                click = false;

                int finalsend = funcWindow;

                if(sendPlayerNum)
                {
                    if(name == "Player")
                    {
                        finalsend = 0;
                    }
                    else
                    {
                        finalsend = 1;
                    }
                }
                
                if(sendToTutorial && tut != null)
                {
                    if(tut.AttemptAdvanceTutorial(tutorialSend))
                    {
                        ui.SendMessage(funcName, finalsend);
                    }
                }
                else
                {
                    ui.SendMessage(funcName, finalsend);
                }
            }
        }
    }
}

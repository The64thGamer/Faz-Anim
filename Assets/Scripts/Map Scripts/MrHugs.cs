using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrHugs : MonoBehaviour
{
    public GameObject gameCam;

    //Movement
    public float sprintSpeed = 10;
    public float sprintFadeMultiplier = 5;
    public float doubleTapWindow = 0.5f;
    public float movementSpeed = 3;
    Vector3 camPos;
    float sprintAdditive;
    float doubleTapTimer;
    bool sprintDir;

    //Camera
    public Animator camFlip;
    public CamState camState = CamState.none;
    public HugsCams[] cameras;
    public float cameraMoveSpeed;
    public float cameraWaitTime;
    public GameObject camObj;
    public SpriteRenderer camObjRender;
    public int currentCam;
    public enum CamState
    {
        none,
        camUp,
        cams,
        camDown
    }

    void Start()
    {
        camPos = gameCam.transform.position;
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].position.x = Random.Range(-3.3f, 3.3f);
            cameras[i].direction = Random.value < 0.5;
            cameras[i].position.z = camObj.transform.localPosition.z;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Camera
        //Determine Cam State
        CamState old = camState;
        if (camFlip.GetCurrentAnimatorStateInfo(0).IsName("NoCams")) { camState = CamState.none; }
        if (camFlip.GetCurrentAnimatorStateInfo(0).IsName("CamUp")) { camState = CamState.camUp; }
        if (camFlip.GetCurrentAnimatorStateInfo(0).IsName("Cams")) { camState = CamState.cams; }
        if (camFlip.GetCurrentAnimatorStateInfo(0).IsName("CamDown")) { camState = CamState.camDown; }
        if (camState != old && camState == CamState.cams)
        {
            CamStart();
        }
        if (camState != old && camState == CamState.camDown)
        {
            CamEnd();
        }

        //Check cam flip
        if (Input.GetKeyDown("c"))
        {
            if (camState == CamState.none || camState == CamState.cams)
            {
                camFlip.SetBool("CamUp", !camFlip.GetBool("CamUp"));
            }
        }

        //Camera Movement
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].waitTimer < 0)
            {
                if (cameras[i].direction)
                {
                    cameras[i].position.x += cameraMoveSpeed * Time.deltaTime;
                }
                else
                {
                    cameras[i].position.x -= cameraMoveSpeed * Time.deltaTime;
                }
                if (Mathf.Abs(cameras[i].position.x) > 0.54f)
                {
                    cameras[i].position.x = Mathf.Min(Mathf.Max(cameras[i].position.x, -0.54f), 0.54f);
                    cameras[i].direction = !cameras[i].direction;
                    cameras[i].waitTimer = cameraWaitTime;
                }
            }
            else
            {
                cameras[i].waitTimer -= Time.deltaTime;
            }
        }
        //Current Cam
        camObj.transform.localScale = new Vector3((1 - ((Mathf.Abs(camObj.transform.localPosition.x)- 0.54f)/ 3.0f)) * 0.165f, 0.165f, 0.165f);
        camObj.transform.localPosition = cameras[currentCam].position;
        //Cam Change
        if(camState == CamState.cams)
        {
            if (Input.GetKeyDown("down"))
            {
                currentCam++;
                if (currentCam > cameras.Length - 1)
                {
                    currentCam--;
                }
                else
                {
                    ChangeCam();
                }
            }
            if (Input.GetKeyDown("up"))
            {
                currentCam--;
                if (currentCam < 0)
                {
                    currentCam++;
                }
                else
                {
                    ChangeCam();
                }
            }
        }
        

        if (camState != CamState.cams)
        {
            //Double tap dash
            //Check if double tap has started
            if (doubleTapTimer > 0)
            {
                doubleTapTimer -= Time.deltaTime;
                if ((sprintDir && Input.GetKeyDown("left")) || (!sprintDir && Input.GetKeyDown("right")))
                {
                    //Finish Double Tap
                    sprintAdditive = sprintSpeed;
                    doubleTapTimer = 0;
                }
                if ((!sprintDir && Input.GetKeyDown("left")) || (sprintDir && Input.GetKeyDown("right")))
                {
                    //Fail Double Tap
                    doubleTapTimer = 0;
                }
            }
            else if (Input.GetKeyUp("left") && sprintAdditive <= 0)
            {
                //Begin Double Tap
                sprintDir = true;
                doubleTapTimer = doubleTapWindow;
            }
            else if (Input.GetKeyUp("right") && sprintAdditive <= 0)
            {
                //Begin Double Tap
                sprintDir = false;
                doubleTapTimer = doubleTapWindow;
            }

            //Movement
            if (Input.GetKey("left"))
            {
                if (!sprintDir)
                {
                    sprintAdditive = 0;
                }
                camPos.x -= movementSpeed * Time.deltaTime;
            }
            if (Input.GetKey("right"))
            {
                if (sprintDir)
                {
                    sprintAdditive = 0;
                }
                camPos.x += movementSpeed * Time.deltaTime;
            }
            if (sprintAdditive > 0)
            {
                if (sprintDir)
                {
                    camPos.x -= sprintAdditive * Time.deltaTime;
                }
                else
                {
                    camPos.x += sprintAdditive * Time.deltaTime;
                }
                sprintAdditive = Mathf.Max(sprintAdditive - (sprintAdditive * sprintFadeMultiplier * Time.deltaTime), 0);
                if (sprintAdditive < 0.1f)
                {
                    sprintAdditive = 0;
                }
            }
            camPos.x = Mathf.Min(Mathf.Max(camPos.x, -5.1f), 5.1f);
            gameCam.transform.position = camPos;
        }
        else
        {
            sprintAdditive = 0;
        }
    }
    public void CamStart()
    {
        ChangeCam();
    }
    public void CamEnd()
    {
        camObj.SetActive(false);
    }
    public void ChangeCam()
    {
        camObj.SetActive(true);
        camObjRender.sprite = cameras[currentCam].background;
    }
}

[System.Serializable]
public class HugsCams
{
    public string name;
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public bool direction;
    [HideInInspector]
    public float waitTimer;
    public Sprite background;
}
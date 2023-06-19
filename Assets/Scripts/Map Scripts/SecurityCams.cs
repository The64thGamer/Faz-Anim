using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecurityCams : MonoBehaviour
{
    public Transform[] cams;
    public Camera cam;
    public Transform camParent;
    public Transform[] buttons;
    float currentAngle = 0;
    float waitTimer = 0;
    bool side;
    public Static statcc;
    float minute = 0;
    public Text time;
    public Text currentCam;
    public string[] camNames;
    public GameObject player;
    public bool on;

    //50x50
    private void Start()
    {
        UpdateCams();
        SelectCam(1);
        player = GameObject.Find("Player");
    }

    public void UpdateCams()
    {
        Vector2[] poses = new Vector2[cams.Length];
        for (int i = 0; i < cams.Length; i++)
        {
            poses[i] = new Vector2(cams[i].position.x, cams[i].position.z);
        }
        float minX = 0;
        float minY = 0;
        float maxX = 0;
        float maxY = 0;
        for (int i = 0; i < poses.Length; i++)
        {
            if(i == 0)
            {
                minX = poses[i].x;
                maxX = poses[i].x;
                minY = poses[i].y;
                maxY = poses[i].y;
            }
            if(poses[i].x < minX)
            {
                minX = poses[i].x;
            }
            if (poses[i].y < minY)
            {
                minY = poses[i].y;
            }
            if (poses[i].x > maxX)
            {
                maxX = poses[i].x;
            }
            if (poses[i].y > maxY)
            {
                maxY = poses[i].y;
            }
        }
        //50
        for (int i = 0; i < poses.Length; i++)
        {
            poses[i].x = -remap(poses[i].x, minX, maxX, -50, 50);
            poses[i].y = -remap(poses[i].y, minY, maxY, -50, 50);
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<RectTransform>().localPosition = poses[i];
        } 

    }

    public static float remap(float val, float in1, float in2, float out1, float out2)
    {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }

    private void Update()
    {
        if(Vector3.Distance(player.transform.position,this.transform.position) < 5)
        {
            if(!on)
            {
                cam.gameObject.SetActive(true);
            }
            on = true;
        }
        else
        {
            if (on)
            {
                cam.gameObject.SetActive(false);
            }
            on = false;
        }

        if(on)
        {
            if (minute == 0)
            {
                minute += 60.0f;
                DateTime dt = DateTime.Now;
                time.text = dt.ToString("h:mm tt"); // 7:00 AM // 12 hour clock
            }

            minute = Mathf.Max(0, minute - Time.deltaTime);
            waitTimer = Mathf.Max(0, waitTimer - Time.deltaTime);
            if (waitTimer == 0)
            {
                if (side)
                {
                    currentAngle += 10.0f * Time.deltaTime;
                }
                else
                {
                    currentAngle -= 10.0f * Time.deltaTime;
                }
            }
            if (currentAngle > 45 || currentAngle < -45)
            {
                currentAngle = 44.9f * Mathf.Sign(currentAngle);
                side = !side;
                waitTimer = 2.0f;
            }
            cam.transform.localEulerAngles = new Vector3(150, currentAngle, -180);
        }
    }


    public void SelectCam(int slot)
    {
        statcc.flash = true;
        slot--;
        camParent.transform.position = cams[slot].transform.position;
        camParent.transform.forward = cams[slot].up;
        currentCam.text = camNames[slot];
    }
}

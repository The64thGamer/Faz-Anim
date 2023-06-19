using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalandarNotif : MonoBehaviour
{
    private void Awake()
    {
        if (System.DateTime.Now.Month == 8
        && System.DateTime.Now.Day == 8)
        {
            this.GetComponent<Text>().text = "Happy Birthday FNaF!";
        }
        else if (System.DateTime.Now.Month == 11
        && System.DateTime.Now.Day == 11)
        {
            this.GetComponent<Text>().text = "Happy Birthday FNaF 2!";
        }
        else if (System.DateTime.Now.Month == 3
        && System.DateTime.Now.Day == 2)
        {
            this.GetComponent<Text>().text = "Happy Birthday FNaF 3!";
        }
        else if (System.DateTime.Now.Month == 7
        && System.DateTime.Now.Day == 23)
        {
            this.GetComponent<Text>().text = "Happy Birthday FNaF 4!";
        }
        else if (System.DateTime.Now.Month == 10
        && System.DateTime.Now.Day == 7)
        {
            this.GetComponent<Text>().text = "Happy Birthday Sister Location!";
        }
        else if (System.DateTime.Now.Month == 1
        && System.DateTime.Now.Day == 21)
        {
            this.GetComponent<Text>().text = "Happy Birthday FNaF World!";
        }
        else if (System.DateTime.Now.Month == 10
        && System.DateTime.Now.Day == 30)
        {
            this.GetComponent<Text>().text = "Happy Birthday FNaF 4 Halloween Edition!";
        }
        else if (System.DateTime.Now.Month == 12
        && System.DateTime.Now.Day == 4)
        {
            this.GetComponent<Text>().text = "Happy Birthday FFPS!";
        }
        else if (System.DateTime.Now.Month == 5
        && System.DateTime.Now.Day == 28)
        {
            this.GetComponent<Text>().text = "Happy Birthday FNaF VR: Help Wanted!";
        }
        else if (System.DateTime.Now.Month == 8
        && System.DateTime.Now.Day == 24)
        {
            this.GetComponent<Text>().text = "Happy Birthday RR!";
        }
        else if (System.DateTime.Now.Month == 1
        && System.DateTime.Now.Day == 22)
        {
            this.GetComponent<Text>().text = "Happy Birthday Faz-Anim!";
        }
        else
        {
            Destroy(this.gameObject.transform.parent.gameObject);
        }
    }
}

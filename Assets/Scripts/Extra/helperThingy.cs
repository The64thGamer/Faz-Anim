using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helperThingy : MonoBehaviour
{
    public TTS textSpeech;
    public GameObject monitor;
    public float timer;
    public questionObj[] questionlists;

    bool starting;
    int question = -1;
    int step = 0;
    bool countdown = false;

    private void Start()
    {
        monitor.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (starting)
        {
            timer = Mathf.Max(timer - Time.deltaTime, 0);
            if (timer == 0)
            {
                countdown = false;
                if (question == -1)
                {
                    textSpeech.inputText = "";
                    monitor.SetActive(true);
                }
                else
                {
                    if (step < questionlists[question].lines.Length)
                    {
                        textSpeech.inputText = questionlists[question].lines[step];
                        step++;
                    }
                    else
                    {
                        question = -1;
                        step = 0;
                        timer = 0;
                        textSpeech.inputText = "";
                    }
                    timer = 50.0f;
                }
            }
            if (textSpeech.inputText == textSpeech.outputText && textSpeech.inputText != "" && question != -1 && !countdown)
            {
                countdown = true;
                timer = 2.0f;
            }
        }
    }

    public void Startup(int input)
    {
        if (!monitor.activeSelf)
        {
            textSpeech.inputText = "You needing some more info?";
            timer = 3;
            countdown = true;
            starting = true;
        }
    }

    public void AskQuestion(int input)
    {
        question = input;
        step = 0;
        timer = 0;
        textSpeech.inputText = questionlists[question].lines[step];
    }


    [System.Serializable]
    public class questionObj
    {
        public string preview;
        public string[] lines;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTS : MonoBehaviour
{
    AudioSource speech;
    public TTSLetter[] ttsAlphabet;
    public string inputText;
    string internalText;
    public string outputText;
    float timer;
    int letterindex;
    Player player;
    public TTSName ttsName;
    public enum TTSName
    {
        Agit,
        Player,
    }

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        speech = this.GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        if (inputText != internalText)
        {
            internalText = inputText;
            outputText = "";
            letterindex = 0;
            switch (ttsName)
            {
                case TTSName.Agit:
                    player.agitText.text = outputText;
                    break;
                case TTSName.Player:
                    player.playerText.text = outputText;
                    break;
                default:
                    break;
            }
        }

        timer = Mathf.Max(0, timer - Time.deltaTime);

        if (internalText != outputText && timer == 0)
        {
            bool isthereLetter = false;
            outputText += internalText[letterindex];
            switch (ttsName)
            {
                case TTSName.Agit:
                    player.agitText.text = outputText;
                    break;
                case TTSName.Player:
                    player.playerText.text = outputText;
                    break;
                default:
                    break;
            }
            for (int i = 0; i < ttsAlphabet.Length; i++)
            {
                if (Char.ToLower(ttsAlphabet[i].letter) == Char.ToLower(internalText[letterindex]))
                {
                    isthereLetter = true;
                    PlayVoice(i, internalText[letterindex]);
                    letterindex++;
                    break;
                }
            }
            if (!isthereLetter)
            {
                PlayVoice(UnityEngine.Random.Range(0, ttsAlphabet.Length), internalText[letterindex]);
                letterindex++;
            }
        }
    }
    void PlayVoice(int theletter, char theactualletter)
    {
        if (ttsAlphabet[theletter].letter != ','
            && ttsAlphabet[theletter].letter != '.'
            && ttsAlphabet[theletter].letter != '!'
            && ttsAlphabet[theletter].letter != '?'
            && ttsAlphabet[theletter].letter != ' '
            && ttsAlphabet[theletter].letter != '\"'
            && ttsAlphabet[theletter].letter != '\''
            && ttsAlphabet[theletter].letter != ':'
            && ttsAlphabet[theletter].letter != ';'
            && ttsAlphabet[theletter].letter != '('
            && ttsAlphabet[theletter].letter != ')'
            && ttsAlphabet[theletter].letter != '-'
            && ttsAlphabet[theletter].letter != '_'
            && ttsAlphabet[theletter].letter != '+'
            && ttsAlphabet[theletter].letter != '='
            && ttsAlphabet[theletter].letter != '*'
            && ttsAlphabet[theletter].letter != '\'')
        {
            speech.volume = 0.9f;
            speech.PlayOneShot(ttsAlphabet[theletter].sound);
        }
        if (Char.ToLower(theactualletter) == 'a'
            && Char.ToLower(theactualletter) == 'e'
            && Char.ToLower(theactualletter) == 'i'
            && Char.ToLower(theactualletter) == 'o'
            && Char.ToLower(theactualletter) == 'u'
            && Char.ToLower(theactualletter) == 'y')
        {
            timer += 0.06f;
        }
        else if (theactualletter == ' ')
        {
            timer += 0.1f;
        }
        else if (theactualletter == ','
        || theactualletter == '.'
        || theactualletter == '!'
        || theactualletter == '?'
        || theactualletter == ':'
        || theactualletter == ';')
        {
            timer += 0.4f;
        }
        else
        {
            timer += 0.05f;
        }

    }
}




[System.Serializable]
public class TTSLetter
{
    public char letter;
    public AudioClip sound;
}
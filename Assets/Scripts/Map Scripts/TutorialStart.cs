using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStart : MonoBehaviour
{
    // Start is called before the first frame update
    public float timeScale = 1;
    public GameObject waypoint1;
    public GameObject waypoint2;
    public GameObject button;
    public GameObject fakeFnaF2;
    public GameObject fakeMangle;
    public SkinnedMeshRenderer door;
    public Animator robotohmygod;
    public TTS textToSpeechRobot;
    TTS textToSpeechPlayer;
    public float time = -2;
    public int state;
    public float walkSpeed;
    public float sideSpeed;
    GlobalController gc;
    GameObject player;
    Player playerCPT;
    AudioSource sc;
    Vector3 oldRobotTransform;
    bool unlock = false;
    bool finalWords;
    public GameObject finalBot;

    //Lines
    string[] lines = new string[]{
        "Ah, you are online.",
        "Seems you have turned on before we arrived.",
        "...",
        "My systems are functional.",
        "Indeed they are. You are brand new, just being shipped to the location.",
        "Where are we going?",
        "Fazbear Entertainment is opening a new restaurant soon. It is tying their previous animatronics into a single location, hoping to capture the nostalgia of long-time fans.",
        "…And why have they sent me?",
        "You are the Faz-Animation Bot #001,\ndesigned to bring the animatronics’ performances to life.",
        "Then why do I not have the knowledge to manage these tasks?",
        "The company has given you a complex AI to learn the task of making showtapes. They feel that letting you experiment will allow just as natural of decision making as any human, and hopefully even better.",
        "I am your Animation Director, Guide, & Info Trainer. You may call me: ADGIT.\n...Considering your product name, I shall call you: FAB.",
        "That name does not suit me.",
        "I see your AI chip is already making decisions.",
        "Here, I shall take on a more visual form to stimulate your ocular sensors.",
        "This form is acceptable.",
        "...",
        "Well, it may suit us if we begin before the owner arrives. We have a while to spare before they get here.",
        "Do you mind unlatching the crate?",
        "I'll head to the main showroom. Meet me when you're ready.",
        "Affirmative.",
        "I have arrived.",
        "Welcome to the classic Freddy's show you know, and are programmed, to love.",
        "I can't shake the feeling I can't say no.",
        "In front of you are your animation panels.\nThe left is your 'Live Editor'.\nThis lets you change volume, turn the lights on, and give your view neat VHS effects.",
        "So like some sort of DJ stand?",
        "Yes. For now we're gonna go into the 'Show' menu and force open the curtains.",
        "Aaaand there they are. Stunning work of 90's craftmanship. Press 'E' to turn your flashlight on to see them...",
        "The right is your main controls. This is where you will be producing your showtapes. Go ahead and open the menu.",
        "Here you can make your recorded shows, play them back on the animatronics, and customize the stage and costumes to fit your mood.",
        "Let's go and record.",
        "Here you're gonna be starting a new segment. Think of it as a single song in your album of a performance.",
        "First you'll need to insert the audio of your performance. Find a .wav file out of your database, take as long as you need.",
        "Here we're gonna be adding new movements to the segment.",
        "This is the catalogue of things you can control on the stage. For now we're gonna choose Freddy",
        "Here is Freddy's controls. You can test out his movements on this screen before starting.",
        "Use your keyboard to control him. Try pressing 4 to open his mouth, we'll be using this for our first recording.",
        "Ah~",
        "When you hit ready, your audio will begin playing, and the computer will record Freddy's movements. Try lip-syncing just his jaw to any words.",
        "The tape will finish when the audio ends, but you can hit the finish button at any time to save and exit.",
        "My showtape is finished. It is- glorious. Truly I'm ready for a raise.",
        "Not without some additions you aren't. The key to showtape animation is going back and adding more movements.",
        "This time, try using both your hands and focus on Freddy's arms and body. Really get him moving to that lip-sync you just made.",
        "Freddy's animations were truly hyper-realistic. I made his robot husk nearly alive.",
        "Your AI is clearly overpraising your ability, but you will learn in time.",
        "Next we're going to do lights. This is what truly makes your showtape segment pop.",
        "Turn Freddy's spotlight on when he speaks, and try making the colored lights dance to the beat.",
        "The audience will now be blinded to my awe-inspiring mastercraft.",
        "More likely they'll now just be able to see whats even going on.",
        "Well, I'm opening up all controls to you now. Feel free to force the curtains back closed, try to now open and close by animating them from the movement catalogue. It truly gives more spectacle.",
        "And feel free to continue working on your showtape. I'll be at the showroom entrance when you're ready to see more.",
        "Thank you, ADGIT.",
        "Glad you're enjoying being on the job. Anyways, here is the Prize Corner. Fazbear Entertainment is substituting a pay grade, as you have no human rights, in favor of gimmicky prizes for your hard work.",
        "You may have noticed on the Live Editor panel your Ticket count going up. Programming shows will earn you tickets to buy here.",
        "When you buy something, you can open up the Pause Menu to access it. Spawn in as many as you want.",
        "Pressing 'Z' will cycle through your Tool Wheel. This will let you pick up, freeze, and delete items, as well as taking screenshots.",
        "Use these prizes to decorate your stage. This will let you customize the feel of your tapes if you decide to record videos for them.",
        "I am grateful my overlords have wavered my right to pay in exchange for a right to creative freedom.",
        "Speaking of creativity, there are more animatronic stages currently set up in the building. They are more complex, but management hopes you'll have them all mastered by opening.",
        "Best of luck, FAB. If you want any more info and help, I'll be in the conference room.",
        "I shall come to you with any plights for information, as well as let you gawk at my creations.",
        "I live in your head. I can literally see everything you do. Let's just hope your stumblings won't be as bad as R-12's.",
    };


    void Start()
    {
        sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        if ((PlayerPrefs.GetInt("Tutorial Save 0") == 0 || !PlayerPrefs.HasKey("Tutorial Save 0")) && GameVersion.isVR != "true")
        {
            player = GameObject.Find("Player");
            fakeFnaF2.SetActive(false);
            fakeMangle.SetActive(false);
            robotohmygod.gameObject.SetActive(false);
            button.SetActive(false);
            gc = GameObject.Find("Global Controller").GetComponent<GlobalController>();
            playerCPT = player.GetComponent<Player>();
            playerCPT.SetFade(255, 1);
            playerCPT.canPause = false;
            textToSpeechPlayer = player.GetComponent<TTS>();
        }
        else
        {
            finalBot.SetActive(true);
            Destroy(this.gameObject);
        }
    }

    public void Update()
    {
        time += Mathf.Min(Time.deltaTime,0.2f) * timeScale;
        //Need states to prevent looping of commands
        if (time > 6 && state == 0)
        {
            state++;
            robotohmygod.gameObject.SetActive(true);
            textToSpeechRobot.inputText = lines[0];
        }
        if (time > 8 && state == 1)
        {
            state++;
            playerCPT.SetFade(0, 1);
        }

        if (time > 14 && state == 2)
        {
            state++;
            textToSpeechRobot.inputText = lines[1];
        }
        if (time > 18 && state == 3)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = lines[2];
        }
        if (time > 21 && state == 4)
        {
            state++;
            textToSpeechPlayer.inputText = lines[3];
        }
        if (time > 25 && state == 5)
        {
            state++;
            textToSpeechRobot.inputText = lines[4];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 32 && state == 6)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = lines[5];
        }
        if (time > 34.5 && state == 7)
        {
            state++;
            textToSpeechRobot.inputText = lines[6];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 48 && state == 8)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = lines[7];
        }
        if (time > 52 && state == 9)
        {
            state++;
            textToSpeechRobot.inputText = lines[8];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 60 && state == 10)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = lines[9];
        }
        if (time > 66 && state == 11)
        {
            state++;
            textToSpeechRobot.inputText = lines[10];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 82 && state == 12)
        {
            state++;
            textToSpeechRobot.inputText = lines[11];
        }
        if (time > 96 && state == 13)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = lines[12];
        }
        if (time > 99.5f && state == 14)
        {
            state++;
            textToSpeechRobot.inputText = lines[13];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 103.5 && state == 15)
        {
            state++;
            textToSpeechRobot.inputText = "";
            playerCPT.SetFade(255, 3);
        }
        if (time > 113 && time < 122 && state == 16)
        {
            playerCPT.keyboardLayout.gameObject.SetActive(true);
            playerCPT.keyboardLayout.color = new Color(1, 1, 1, Mathf.Min(playerCPT.keyboardLayout.color.a + (1 * Time.deltaTime), 1));
        }
        if (time > 122 && time < 130 && state == 16)
        {
            playerCPT.keyboardLayout.gameObject.SetActive(true);
            playerCPT.keyboardLayout.color = new Color(1, 1, 1, Mathf.Max(playerCPT.keyboardLayout.color.a - (1 * Time.deltaTime), 0));
        }
        if (time > 124 && state == 16)
        {
            robotohmygod.gameObject.transform.position += new Vector3(0, 10, 0);
            player.transform.position = new Vector3(0.39633f, -0.53f, -2.731902f);
            player.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 362.0f, 0.0f));
            state++;
        }
        if (time > 131 && state == 17)
        {
            state++;
            player.GetComponent<Player>().SetFade(0, 20);
        }
        if (time > 134 && state == 18)
        {
            state++;
            textToSpeechRobot.inputText = lines[14];
        }
        //LATER
        if (time > 140 && state == 19)
        {
            state++;
            robotohmygod.GetComponent<AudioSource>().spatialBlend = 1;
            robotohmygod.gameObject.transform.position -= new Vector3(0, 10, 0);
            sc.clip = (AudioClip)Resources.Load("SystemOpen");
            sc.pitch = 1.0f;
            sc.Play();
            robotohmygod.SetBool("Start", true);
        }
        if (time > 143.5f && state == 20)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = lines[15];
        }
        if (time > 146 && state == 21)
        {
            state++;
            textToSpeechRobot.inputText = lines[16];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 149 && state == 22)
        {
            state++;
            textToSpeechRobot.inputText = lines[17];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 157.5 && state == 23)
        {
            state++;
            textToSpeechRobot.inputText = lines[18];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 160.5 && state == 24)
        {
            textToSpeechRobot.inputText = "";
            state++;
            button.SetActive(true);
        }



        //DOOOR SECTION


        if (state >= 26 )
        {
            Vector3 vel = oldRobotTransform - robotohmygod.transform.position;
            Vector3 localVel = robotohmygod.transform.InverseTransformDirection(vel);
            robotohmygod.SetFloat("Velocity X", Mathf.Max(Mathf.Min(localVel.x * -sideSpeed, 1), -1), 0.1f, Time.deltaTime);
            robotohmygod.SetFloat("Velocity Z", Mathf.Max(Mathf.Min(localVel.z * -walkSpeed, 1), -1), 0.1f, Time.deltaTime);
            oldRobotTransform = robotohmygod.transform.position;
        }
        if (time > 3 && state == 26)
        {
            state++;
            textToSpeechRobot.inputText = lines[19];
            textToSpeechPlayer.inputText = "";
            robotohmygod.SetBool("Walk", true);
            robotohmygod.SetInteger("State", 1);
        }
        if (time > 10 && state == 27)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = lines[20];
            waypoint1.SetActive(true);
        }
        if (time > 12 && state == 28)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = "";
        }



        //TUTORIAL START

        if (time > 3 && state == 30)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = "";
        }

        if (time > 3 && state == 32)
        {
            state++;
            textToSpeechRobot.inputText = lines[22];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 10 && state == 33)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = lines[23];
        }
        if (time > 14 && state == 34)
        {
            state++;
            textToSpeechRobot.inputText = lines[24];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 28 && state == 35)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = lines[25];
        }
        if (time > 31 && state == 36)
        {
            state++;
            textToSpeechRobot.inputText = lines[26];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 34 && state == 37)
        {
            state++;
            textToSpeechPlayer.inputText = "";
            textToSpeechPlayer.inputText = "";
        }
        if (time > 4 && state == 40)
        {
            state++;
            textToSpeechRobot.inputText = lines[27];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 20 && state == 41)
        {
            state++;
            textToSpeechRobot.inputText = lines[28];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 2 && state == 43)
        {
            state++;
            textToSpeechRobot.inputText = lines[29];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 13 && state == 44)
        {
            state++;
            textToSpeechRobot.inputText = lines[30];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 1 && state == 46)
        {
            state++;
            textToSpeechRobot.inputText = lines[31];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 1 && state == 48)
        {
            state++;
            textToSpeechRobot.inputText = lines[32];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 1 && state == 50)
        {
            state++;
            textToSpeechRobot.inputText = lines[33];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 1 && state == 52)
        {
            state++;
            textToSpeechRobot.inputText = lines[34];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 1 && state == 54)
        {
            state++;
            textToSpeechRobot.inputText = lines[35];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 9 && state == 55)
        {
            state++;
            textToSpeechRobot.inputText = lines[36];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 17 && state == 56)
        {
            state++;
        }
        if (state == 57 && Input.GetKeyDown(KeyCode.Alpha4))
        {
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = lines[37];
        }
        if (state == 57 && Input.GetKeyUp(KeyCode.Alpha4))
        {
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = "";
        }
        if (time > 29 && state == 57)
        {
            state++;
            textToSpeechRobot.inputText = lines[38];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 41 && state == 58)
        {
            state++;
            textToSpeechRobot.inputText = lines[39];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 1 && state == 61)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = lines[40];
        }
        if (time > 9 && state == 62)
        {
            state++;
            textToSpeechRobot.inputText = lines[41];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 1 && state == 64)
        {
            state++;
            textToSpeechRobot.inputText = lines[42];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 1 && state == 67)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = lines[43];
        }
        if (time > 10f && state == 68)
        {
            state++;
            textToSpeechRobot.inputText = lines[44];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 18 && state == 69)
        {
            state++;
            textToSpeechRobot.inputText = lines[45];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 1 && state == 71)
        {
            state++;
            textToSpeechRobot.inputText = lines[46];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 1 && state == 74)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = lines[47];
        }
        if (time > 7 && state == 75)
        {
            state++;
            textToSpeechRobot.inputText = lines[48];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 13 && state == 76)
        {
            state++;
            unlock = true;
            fakeFnaF2.SetActive(true);
            fakeMangle.SetActive(true);
            textToSpeechRobot.inputText = lines[49];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 30 && state == 77)
        {
            state++;
            textToSpeechRobot.inputText = lines[50];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 40 && state == 78)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = lines[51];
            robotohmygod.SetBool("Walk", true);
            robotohmygod.SetInteger("State", 2);
            waypoint2.SetActive(true);
        }
        if (time > 43 && state == 79)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = "";
        }

        //===========================================

        if (time > 0 && state == 81)
        {
            state++;
            playerCPT.canPause = true;
            textToSpeechRobot.inputText = lines[52];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 18 && state == 82)
        {
            state++;
            textToSpeechRobot.inputText = lines[53];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 29 && state == 83)
        {
            state++;
            textToSpeechRobot.inputText = lines[54];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 38 && state == 84)
        {
            state++;
            textToSpeechRobot.inputText = lines[55];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 49.5 && state == 85)
        {
            state++;
            textToSpeechRobot.inputText = lines[56];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 61 && state == 86)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = lines[57];
        }
        if (time > 69.5 && state == 87)
        {
            state++;
            textToSpeechRobot.inputText = lines[58];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 84 && state == 88)
        {
            state++;
            textToSpeechRobot.inputText = lines[59];
            textToSpeechPlayer.inputText = "";
            robotohmygod.SetBool("Walk", true);
            robotohmygod.SetInteger("State", 3);
        }
        if (time > 93 && state == 89)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = lines[60];
        }
        if (time > 100.5 && state == 90)
        {
            state++;
            textToSpeechRobot.inputText = lines[61];
            textToSpeechPlayer.inputText = "";
        }
        if (time > 110 && state == 91)
        {
            state++;
            textToSpeechRobot.inputText = "";
            textToSpeechPlayer.inputText = "";
        }
        if (time > 111 && state == 92)
        {
            state++;
            finalBot.SetActive(true);
            PlayerPrefs.SetInt("Tutorial Save 0", 1);
            Destroy(robotohmygod.gameObject);
            Destroy(this);
        }
    }
    
/*
        "Glad you're enjoying being on the job. Anyways, here is the Prize Corner. Fazbear Entertainment is substituting a pay grade, as you have no human rights, in favor of gimmicky prizes for your hard work.",
        "You may have noticed on the Live Editor panel your Ticket count going up. Programming shows will earn you tickets to buy here.",
        "When you buy something, you can open up the Pause Menu to access it. Spawn in as many as you want.",
        "Pressing 'Z' will cycle through your Tool Wheel. This will let you pick up, freeze, and delete items, as well as taking screenshots.",
        "Use these prizes to decorate your stage. This will let you customize the feel of your tapes if you decide to record videos for them.",
        "I am grateful my overlords have wavered my right to pay in exchange for a right to creative freedom.",
        "Speaking of creativity, there is another animatronic stage currently set up in the building. It is more complex, but management hopes you'll have them both mastered by opening.",
        "Best of luck, FAB. If you want any more info and help, I'll be in the conference room.",
        "I shall come to you with any plights for information, as well as let you gawk at my creations",
        "I live in your head. I can literally see everything you do. Let's just hope your stumblings won't be as bad as R-12's.",
*/

    public void TipDoor(int nothing)
    {
        time = 0;
        state = 26;
        this.GetComponent<Animation>().Play();
        Destroy(this.GetComponent<AudioSource>());
        playerCPT.keyboardLayout.color = new Color(1, 1, 1, 0);
        door.useLightProbes = true;
        Destroy(button);
    }

    public bool AttemptAdvanceTutorial(string attempt)
    {
        if (state == 80 && attempt == "Reach Prize")
        {
            state++;
            time = 0;
            textToSpeechRobot.inputText = "";
            return true;
        }
        if (!unlock)
        {
            Debug.Log("Attempt: " + attempt);
            if (state == 29 && attempt == "Reach Showroom")
            {
                state++;
                time = 0;
                textToSpeechRobot.inputText = "";
                textToSpeechPlayer.inputText = lines[21];
                return true;
            }
            if (state == 31 && attempt == "OpenPanel")
            {
                state++;
                time = 0;
                return true;
            }
            if (state == 38 && attempt == "Access Show Live Panel")
            {
                state++;
                time = 0;
                return true;
            }
            if (state == 39 && attempt == "Open Curtain")
            {
                state++;
                time = 0;
                return true;
            }
            if (state == 42 && attempt == "Open Main Panel")
            {
                state++;
                time = 0;
                textToSpeechRobot.inputText = "";
                return true;
            }
            if (state == 45 && attempt == "3Window 3")
            {
                state++;
                time = 0;
                textToSpeechRobot.inputText = "";
                return true;
            }
            if (state == 47 && attempt == "2Window 1")
            {
                state++;
                time = 0;
                textToSpeechRobot.inputText = "";
                return true;
            }
            if (state == 49 && attempt == "Create WAV")
            {
                state++;
                time = 0;
                textToSpeechRobot.inputText = "";
                return true;
            }
            if (state == 51 && attempt == "2Window 2")
            {
                state++;
                time = 0;
                textToSpeechRobot.inputText = "";
                return true;
            }
            if (state == 53 && attempt == "Freddy Moves")
            {
                state++;
                time = 0;
                textToSpeechRobot.inputText = "";
                return true;
            }
            if ((state == 59 || state == 60) && attempt == "ReadyShowtape")
            {
                state = 60;
                time = 0;
                textToSpeechRobot.inputText = "";
                return true;
            }
            if (state == 60 && attempt == "FinishShowtape")
            {
                state++;
                time = 0;
                textToSpeechRobot.inputText = "";
                return true;
            }
            if (state == 63 && attempt == "Freddy Moves")
            {
                state++;
                time = 0;
                textToSpeechRobot.inputText = "";
                return true;
            }
            if ((state == 65 || state == 66) && attempt == "ReadyShowtape")
            {
                state = 66;
                time = 0;
                textToSpeechRobot.inputText = "";
                return true;
            }
            if (state == 66 && attempt == "FinishShowtape")
            {
                state++;
                time = 0;
                textToSpeechRobot.inputText = "";
                return true;
            }
            if (state == 70 && attempt == "Lights Moves")
            {
                state++;
                time = 0;
                textToSpeechRobot.inputText = "";
                return true;
            }
            if ((state == 72 || state == 73) && attempt == "ReadyShowtape")
            {
                state = 73;
                time = 0;
                textToSpeechRobot.inputText = "";
                return true;
            }
            if (state == 73 && attempt == "FinishShowtape")
            {
                state++;
                time = 0;
                textToSpeechRobot.inputText = "";
                return true;
            }
            return false;
        }
        else
        {
            return true;
        }    
    }
}

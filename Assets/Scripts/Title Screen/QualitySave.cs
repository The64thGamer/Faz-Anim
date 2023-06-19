using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QualitySave
{
    public static void ApplySavedQualitySettings()
    {
        int qLevel = PlayerPrefs.GetInt("Settings: Quality") + ((3 - PlayerPrefs.GetInt("Settings: Texture") ) * 3);
        switch (PlayerPrefs.GetInt("Settings: Windowed"))
        {

            case 0:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            default:
                break;
        }
        QualitySettings.SetQualityLevel(qLevel, true);
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("Settings: VSync");
    }

    public static void FirstTimeSave()
    {
        if (PlayerPrefs.GetInt("First Time Starting 1.14") != 1)
        {
            PlayerPrefs.SetInt("First Time Starting 1.14", 1);

            //Settings
            PlayerPrefs.SetInt("Settings: Quality", 2);
            PlayerPrefs.SetInt("Settings: VSync", 0);
            PlayerPrefs.SetInt("Settings: Texture", 3);
            PlayerPrefs.SetInt("Settings: Windowed", 0);
            PlayerPrefs.SetInt("Settings: Playback", 1);
        }
        if (PlayerPrefs.GetInt("First Time Starting 1.36") != 1)
        {
            PlayerPrefs.SetInt("First Time Starting 1.36", 1);

            //Settings
            PlayerPrefs.SetInt("Settings: Motion Blur", 0);
            PlayerPrefs.SetInt("Settings: Auto Exposure", 1);
        }
        if (PlayerPrefs.GetInt("First Time Starting 1.41") != 1)
        {
            PlayerPrefs.SetInt("First Time Starting 1.41", 1);

            //Settings
            PlayerPrefs.SetInt("Settings: SSR", 3);
            PlayerPrefs.SetInt("Settings: SSAO", 1);
        }
    }
}

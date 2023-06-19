using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalDoor : MonoBehaviour
{
    public string[] scenes;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player" || other.name == "Player VR")
            LoadScenesPortal();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player" || other.name == "Player VR")
            LoadScenesPortal();
    }

    void LoadScenesPortal()
    {
        LightProbes.TetrahedralizeAsync();
        int countLoaded = SceneManager.sceneCount;
        Scene[] loadedScenes = new Scene[countLoaded];

        for (int i = 0; i < countLoaded; i++)
        {
            loadedScenes[i] = SceneManager.GetSceneAt(i);
        }
        int e = 0;
        //Unload unnecisary scenes
        //Debug.Log("--Unloading Scenes--");
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            e = 0;
            while (true)
            {
                if (e >= scenes.Length)
                {
                    //Debug.Log("Unloaded " + loadedScenes[i].name);
                    SceneManager.UnloadSceneAsync(loadedScenes[i].name);
                    break;
                }
                if (loadedScenes[i].name == scenes[e])
                {
                    //Debug.Log(loadedScenes[i].name + " was already there");
                    break;
                }
                else
                {
                    e++;
                }
            }
        }
        //Unload Assets
        Resources.UnloadUnusedAssets();
        //Load new scenes
        countLoaded = SceneManager.sceneCount;
        loadedScenes = new Scene[countLoaded];
        //Debug.Log("--Loading Scenes--");
        for (int i = 0; i < countLoaded; i++)
        {
            loadedScenes[i] = SceneManager.GetSceneAt(i);
        }
        e = 0;
        for (int i = 0; i < scenes.Length; i++)
        {
            e = 0;
            while (true)
            {
                if (e >= loadedScenes.Length)
                {
                    this.StartCoroutine(this.LoadLevelAsync(scenes[i]));
                    //Debug.Log("Loaded " + scenes[i]);
                    break;
                }
                if (scenes[i] == loadedScenes[e].name)
                {
                    //Debug.Log(scenes[i] + " was already there");
                    break;
                }
                else
                {
                    e++;
                }
            }
        }
    }

    IEnumerator LoadLevelAsync(string scene)
    {
        AsyncOperation AO = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        AO.allowSceneActivation = false;
        while (AO.progress < 0.9f)
        {
            yield return null;
        }

        //Fade the loading screen out here
        AO.allowSceneActivation = true;
    }
}

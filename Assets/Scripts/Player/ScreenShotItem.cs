using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenShotItem : MonoBehaviour
{
    // Start is called before the first frame update
    public void ScreenshotCheck()
    {
        if (Input.GetMouseButtonDown(1))
        {
            string path = "/screenshots/x" + Mathf.Round(this.transform.position.x * 100f) / 100f + "y" + Mathf.Round(this.transform.position.y * 100f) / 100f + "z" + Mathf.Round(this.transform.position.z * 100f) / 100f;
            path = path.Replace("-", "N");
            path = path.Replace(".", "P");
            path = Application.streamingAssetsPath + path + ".png";
            if (!Directory.Exists(Application.streamingAssetsPath + "/screenshots"))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath + "/screenshots");
            }
            ScreenCapture.CaptureScreenshot(path, 4);
            AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
            sc.clip = (AudioClip)Resources.Load("Camera Shutter");
            sc.Play();
        }
    }

}

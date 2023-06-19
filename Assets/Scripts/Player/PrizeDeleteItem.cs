using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrizeDeleteItem : MonoBehaviour
{
    public GameObject iconUI;
    public GameObject playerCam;
    public LayerMask uiLayerMask;
    // Start is called before the first frame update
    public void DeleterCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 10f, uiLayerMask))
        {
            if (hit.collider.gameObject.layer == 9 || hit.collider.gameObject.layer == 16 || hit.collider.gameObject.layer == 17 || hit.collider.gameObject.layer == 18)
            {
                iconUI.SetActive(true);
                if (Input.GetMouseButtonDown(1))
                {
                    Destroy(hit.collider.transform.root.gameObject);
                    AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
                    sc.clip = (AudioClip)Resources.Load("Trashed");
                    sc.pitch = Random.Range(0.98f, 1.02f);
                    sc.volume = 0.7f;
                    sc.Play();
                }
            }
            else
            {
                iconUI.SetActive(false);
            }
        }
    }

    private void OnDisable()
    {
        iconUI.SetActive(false);
    }
}

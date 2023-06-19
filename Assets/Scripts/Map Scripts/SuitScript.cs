using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuitScript : MonoBehaviour
{
    public GameObject suit;
    public GameObject left;
    public GameObject right;
    public GlobalController globalController;
    public int oneOutofWhat = 100;

    private void OnTriggerEnter(Collider other)
    {
        //Make sure its the player
        if (other.gameObject.layer == 3 && !globalController.TwoJoined)
        {
            int rrrr = Random.Range(0, oneOutofWhat);

            //See if the chance will happen
            if (rrrr == 1)
            {
                Debug.Log("They were there.");
                GameObject suitReal = GameObject.Instantiate(suit);
                int random = Random.Range(0, 2);
                suitReal.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                if (random == 0)
                {
                    suitReal.transform.position = left.transform.position;
                    suitReal.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Left");
                }
                else
                {
                    suitReal.transform.position = right.transform.position;
                    suitReal.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Right");
                }
                StartCoroutine(FullDelete(suitReal));
            }
        }
    }
    IEnumerator FullDelete(GameObject realSuit)
    {
        while (true)
        {
            if(!realSuit.transform.GetChild(0).GetChild(1).gameObject.activeSelf)
            {
                Debug.Log("Gone.");
                Destroy(realSuit);
                break;
            }
            yield return null;
        }
    }
}

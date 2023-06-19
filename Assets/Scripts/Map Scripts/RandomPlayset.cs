using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlayset : MonoBehaviour
{
    public SkinnedMeshRenderer[] lods;
     Material[] bodies;
     Material[] heads;

    public Color[] colors;
    public Texture2D[] faces;
    public Texture2D[] hair;
    public Texture2D[] haircolor;
    public Texture2D[] shirts;
    public Texture2D[] shirtscolor;
    public Texture2D[] pants;
    public Texture2D[] pantscolor;
    Transform[] children;
    private Vector3[] _connectedAnchor;
    private Vector3[] _anchor;
    // Start is called before the first frame update
    void Start()
    {
        bodies = new Material[lods.Length];
        heads = new Material[lods.Length];

        //Setup
        for (int i = 0; i < lods.Length; i++)
        {
            foreach (Material matt in lods[i].materials)
            {
                if (matt.name == "PlayerBody (Instance)")
                {
                    bodies[i] = matt;
                }
                if (matt.name == "PlayerHead (Instance)")
                {
                    heads[i] = matt;
                }
            }
        }


        //Random Color
        int rColor = Random.Range(0, colors.Length);
        int rColor2 = Random.Range(0, colors.Length);
        int rColor3 = Random.Range(0, colors.Length);
        int rColor4 = Random.Range(0, colors.Length);
        int skinColor = Random.Range(0, colors.Length);
        int randomOne = Random.Range(0, 2);
        int randomTwo = Random.Range(0, 2);
        int randomthree = Random.Range(0, 4);
        int randomfour = Random.Range(0, 4);
        int rfaces = Random.Range(0, faces.Length);
        int rHairs = Random.Range(0, hair.Length);
        int rShirts = Random.Range(0, shirts.Length);
        int rPants = Random.Range(0, pants.Length);

        //Apply Body
        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i].SetColor("_Decal_1_Color", colors[rColor]);
            bodies[i].SetColor("_Decal_2_Color", colors[rColor2]);
            if (randomOne == 0)
            {
                bodies[i].SetColor("_Skin_Color", colors[skinColor]);
            }
            if (randomthree == 0)
            {
                bodies[i].SetTexture("_Decal_1", shirts[rShirts]);
                bodies[i].SetTexture("_Decal_1_C", shirtscolor[rShirts]);
            }
            if (randomfour == 0)
            {
                bodies[i].SetTexture("_Decal_2", pants[rPants]);
                bodies[i].SetTexture("_Decal_2_C", pantscolor[rPants]);
            }
        }

        //Apply Head
        for (int i = 0; i < heads.Length; i++)
        {
            heads[i].SetColor("_Decal_1_Color", colors[rColor3]);
            heads[i].SetColor("_Decal_2_Color", colors[rColor4]);
            if (randomOne == 0)
            {
                heads[i].SetColor("_Skin_Color", colors[skinColor]);
            }
            if (randomTwo == 0)
            {
                heads[i].SetTexture("_Decal_1", faces[rfaces]);
            }
            heads[i].SetTexture("_Decal_2", hair[rHairs]);
            heads[i].SetTexture("_Decal_2_C", haircolor[rHairs]);
        }

        children = transform.GetComponentsInChildren<Transform>();
        _connectedAnchor = new Vector3[children.Length];
        _anchor = new Vector3[children.Length];
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].GetComponent<Joint>() != null)
            {
                _connectedAnchor[i] = children[i].GetComponent<Joint>().connectedAnchor;
                _anchor[i] = children[i].GetComponent<Joint>().anchor;
            }
        }
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].GetComponent<Joint>() != null)
            {
                children[i].GetComponent<Joint>().connectedAnchor = _connectedAnchor[i];
                children[i].GetComponent<Joint>().anchor = _anchor[i];
            }
        }
    }

}

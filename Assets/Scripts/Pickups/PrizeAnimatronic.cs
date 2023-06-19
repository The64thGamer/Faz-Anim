using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PrizeAnimatronic : MonoBehaviour
{
    public float mass = 3.0f;
    public PrizeType prizeType;
    float uVSize = 0.5f;

    public enum PrizeType
    {
        basic,
        animatronic,
        buildingblock
    }

    public void ATTStartup()
    {
        switch (prizeType)
        {
            case PrizeType.basic:
                this.GetComponent<Rigidbody>().mass = mass;
                this.gameObject.layer = 9;
                break;
            case PrizeType.animatronic:
                this.gameObject.AddComponent<Rigidbody>();
                this.GetComponent<Rigidbody>().mass = mass;
                this.gameObject.layer = 9;
                this.GetComponent<BoxCollider>().enabled = true;
                this.GetComponentInChildren<Character_Valves>().StartUp();
                break;
            case PrizeType.buildingblock:
                this.GetComponent<Rigidbody>().mass = mass;
                this.gameObject.layer = 17;
                break;
            default:
                break;
        }
    }

    public void ATTSkin(string mesh)
    {
        switch (prizeType)
        {
            case PrizeType.basic:
                //Deletes all meshes under the first gameobject that aren't the string name or "Armature"
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    if (this.transform.GetChild(i).gameObject.name != mesh && this.transform.GetChild(i).gameObject.name != "Armature")
                    {
                        Destroy(this.transform.GetChild(i).gameObject);
                    }
                }
                break;
            case PrizeType.animatronic:
                //Deletes all meshes under the first gameobject that aren't the string name or "Armature"
                for (int i = 0; i < this.transform.GetChild(0).childCount; i++)
                {
                    if (this.transform.GetChild(0).GetChild(i).gameObject.name != mesh && this.transform.GetChild(0).GetChild(i).gameObject.name != "Armature")
                    {
                        Destroy(this.transform.GetChild(0).GetChild(i).gameObject);
                    }
                }
                break;
            case PrizeType.buildingblock:
                GlobalController gg = GameObject.Find("Global Controller").GetComponent<GlobalController>();
                for (int i = 0; i < gg.buildMaterials.Length; i++)
                {
                    if(gg.buildMaterials[i].name == mesh)
                    {
                        MeshRenderer[] mr = this.GetComponentsInChildren<MeshRenderer>();
                        Material[] mat;
                        for (int e = 0; e < mr.Length; e++)
                        {
                            mat = mr[e].materials;
                            for (int f = 0; f < mat.Length; f++)
                            {
                                Color col = mat[f].GetColor("_BaseColor");
                                mat[f] = Instantiate(gg.buildMaterials[i].mat);
                                mat[f].SetColor("_BaseColor", col);
                            }
                            mr[e].materials = mat;
                        }
                    }

                }
                break;
            default:
                break;
        }
        
    }

    public void ATTScale(float scale)
    {
        this.transform.localScale = new Vector3(this.transform.localScale.x * scale, this.transform.localScale.y * scale, this.transform.localScale.z * scale);
        this.GetComponent<Rigidbody>().mass *= scale;
    }

    public void ATTComplexScale(Vector3 scale)
    {
        this.transform.localScale = new Vector3(this.transform.localScale.x * scale.x, this.transform.localScale.y * scale.y, this.transform.localScale.z * scale.z);
    }

    public void ATTLightTemperature(int temp)
    {
        Light[] light = this.GetComponentsInChildren<Light>();
        for (int i = 0; i < light.Length; i++)
        {
            light[i].colorTemperature = temp;
        }
    }

    public void ATTLightIntensity(float intensity)
    {
        HDAdditionalLightData[] light = this.GetComponentsInChildren<HDAdditionalLightData>();
        for (int i = 0; i < light.Length; i++)
        {
            light[i].SetIntensity(intensity);
        }
    }

    public void ATTLightColor(Color color)
    {
        HDAdditionalLightData[] light = this.GetComponentsInChildren<HDAdditionalLightData>();
        for (int i = 0; i < light.Length; i++)
        {
            light[i].SetColor(color);
        }
    }

    public void ATTLightSpotSize(Vector2 both)
    {
        HDAdditionalLightData[] light = this.GetComponentsInChildren<HDAdditionalLightData>();
        for (int i = 0; i < light.Length; i++)
        {
            if (light[i].gameObject.name != "IGNORE")
            {
                light[i].SetSpotAngle(both.x, both.y);
            }
        }
    }

    public void ATTMaterialColor(Color color)
    {
        Debug.Log(this.name + " " + color);
        MeshRenderer[] mr = this.GetComponentsInChildren<MeshRenderer>();
        Material[] mat;
        if(prizeType == PrizeType.buildingblock)
        {
            for (int i = 0; i < mr.Length; i++)
            {
                mat = mr[i].materials;
                for (int e = 0; e < mat.Length; e++)
                {
                    mat[e].SetColor("_BaseColor", color);
                }
                mr[i].materials = mat;
            }
        }   
        else
        {
            for (int i = 0; i < mr.Length; i++)
            {
                mat = mr[i].materials;
                mat[0].SetColor("_BaseColor", color);
                mr[i].materials = mat;
            }
        }
    }
}

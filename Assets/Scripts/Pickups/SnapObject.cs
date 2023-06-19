using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapObject : MonoBehaviour
{
    public SnapType snapType;
    public enum SnapType
    {
        snapChild,
        snapParent
    }
    private void Update()
    {
        switch (snapType)
        {
            case SnapType.snapChild:
                transform.position = new Vector3(Mathf.Round(this.transform.root.position.x * 10) / 10, Mathf.Round((this.transform.root.position.y) * 10) / 10, Mathf.Round(this.transform.root.position.z * 10) / 10);
                break;
            case SnapType.snapParent:
                transform.eulerAngles = new Vector3((Mathf.Round(transform.root.eulerAngles.x / 45) * 45), (Mathf.Round(transform.root.eulerAngles.y / 45) * 45), (Mathf.Round(transform.root.eulerAngles.z / 45) * 45));
                break;
            default:
                break;
        }
    }
}

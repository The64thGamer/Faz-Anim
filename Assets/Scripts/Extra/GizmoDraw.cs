using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GizmoDraw : MonoBehaviour
{
    public static Dictionary<string, string> iconTypes = new Dictionary<string, string>
        {
            { "Seat Enter", "Seat Enter.png" },
            { "Seat Exit", "Seat Exit.png" },
        };
    private void OnDrawGizmos()
    {
        if (GizmoDraw.iconTypes.ContainsKey(gameObject.tag))
        {
            Gizmos.DrawIcon(transform.position, GizmoDraw.iconTypes[gameObject.tag], true);
        }
    }
}
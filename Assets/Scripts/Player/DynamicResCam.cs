using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.Rendering;

public class DynamicResCam : MonoBehaviour
{
    public float currentScale = 100;

    public float SetDynamicResolutionScale()
    {
        return currentScale;
    }

    void Start()
    {
        // Binds the dynamic resolution policy defined above.
        DynamicResolutionHandler.SetDynamicResScaler(SetDynamicResolutionScale, DynamicResScalePolicyType.ReturnsPercentage);
    }
}
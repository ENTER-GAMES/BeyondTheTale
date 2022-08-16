using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBoxManager : MonoBehaviour
{
    [SerializeField]
    private SoundBox soundBox;
    [SerializeField]
    private float minDetectSize;

    private void Update()
    {
        ShadowObject[] shadowObjects = GameObject.FindObjectsOfType<ShadowObject>();

        if (shadowObjects.Length == 0)
        {
            soundBox.SetTargetShadowObject(null);
            return;
        }

        ShadowObject target = null;
        float maxSize = -1;
        foreach (ShadowObject obj in shadowObjects)
        {
            float size = obj.bounds.size.x * obj.bounds.size.y;

            if (size > maxSize)
            {
                target = obj;
                maxSize = size;
            }
        }

        if (target != null && maxSize > minDetectSize)
            soundBox.SetTargetShadowObject(target);
        else
            soundBox.SetTargetShadowObject(null);
    }
}

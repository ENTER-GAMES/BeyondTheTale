using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllObjectsEraser : Eraser
{
    protected override ControllerBasedShadowObject[] Raycast(Vector2 startPosition)
    {
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(startPosition, int.MaxValue, targetLayerMask);

        List<ControllerBasedShadowObject> shadowObjects = new List<ControllerBasedShadowObject>();

        foreach (Collider2D col in collider2Ds)
            if (col.TryGetComponent<ControllerBasedShadowObject>(out ControllerBasedShadowObject shadowObject))
                shadowObjects.Add(shadowObject);

        return shadowObjects.ToArray();
    }
}

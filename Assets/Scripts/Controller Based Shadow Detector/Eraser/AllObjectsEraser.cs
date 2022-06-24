using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllObjectsEraser : Eraser
{
    protected override DrawedShadowObject[] Raycast(Vector2 startPosition)
    {
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(startPosition, int.MaxValue, targetLayerMask);

        List<DrawedShadowObject> drawedShadowObjects = new List<DrawedShadowObject>();

        foreach (Collider2D col in collider2Ds)
            if (col.TryGetComponent<DrawedShadowObject>(out DrawedShadowObject drawedShadowObj))
                drawedShadowObjects.Add(drawedShadowObj);

        return drawedShadowObjects.ToArray();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneObjectEraser : Eraser
{
    protected override DrawedShadowObject[] Raycast(Vector2 startPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPosition, Vector2.zero, int.MaxValue, targetLayerMask);

        if (hit.collider == null) return new DrawedShadowObject[0];

        if (hit.collider.TryGetComponent<DrawedShadowObject>(out DrawedShadowObject drawedShadowObject))
            return new DrawedShadowObject[1] { drawedShadowObject };
        else
            return new DrawedShadowObject[0];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneObjectEraser : Eraser
{
    protected override ControllerBasedShadowObject[] Raycast(Vector2 startPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPosition, Vector2.zero, int.MaxValue, targetLayerMask);

        if (hit.collider == null) return new ControllerBasedShadowObject[0];

        if (hit.collider.TryGetComponent<ControllerBasedShadowObject>(out ControllerBasedShadowObject shadowObject))
            return new ControllerBasedShadowObject[1] { shadowObject };
        else
            return new ControllerBasedShadowObject[0];
    }
}

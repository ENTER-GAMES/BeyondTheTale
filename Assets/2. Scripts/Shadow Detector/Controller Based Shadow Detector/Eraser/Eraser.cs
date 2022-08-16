using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Eraser : ControllerBasedShadowDetectorTool
{
    protected override void OnMouseUp(Vector3 mousePosition, int mouseIndex = 0)
    {
        base.OnMouseUp(mousePosition, mouseIndex);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);

        // 영역 내의 그림자 가져오기
        ControllerBasedShadowObject[] shadowObjects = Raycast(mouseWorldPos);

        // 그림자 삭제
        foreach (ControllerBasedShadowObject obj in shadowObjects)
        {
            detector?.RemoveShadow(obj.Shadow);
            Destroy(obj.gameObject);
        }
    }

    protected abstract ControllerBasedShadowObject[] Raycast(Vector2 startPosition);
}

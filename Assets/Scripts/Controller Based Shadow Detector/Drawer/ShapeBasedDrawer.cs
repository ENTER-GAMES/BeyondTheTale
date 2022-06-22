using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeBasedDrawer : Drawer
{
    [SerializeField]
    private GameObject drawedShadowObjectPrefab;        // 그려질 그림자 오브젝트 프리팹
    private GameObject currentDrawedShadowObject;       // 현재 그려지고 있는 그림자 오브젝트 

    private Vector3 drawStartPosition;          // 그리기 시작한 위치 (월드)

    protected override void OnMouseDown(Vector3 mousePosition, int mouseIndex = 0)
    {
        base.OnMouseDown(mousePosition, mouseIndex);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);

        // 그리기 시작한 위치 지정
        drawStartPosition = mouseWorldPos;
        drawStartPosition.z = 0;  // z축 0으로 (원래는 카메라 위치)

        // 그림자 오브젝트 생성
        currentDrawedShadowObject = Instantiate(drawedShadowObjectPrefab, drawStartPosition, Quaternion.identity);
    }

    protected override void OnMouse(Vector3 mousePosition, int mouseIndex = 0)
    {
        base.OnMouse(mousePosition, mouseIndex);

        if (currentDrawedShadowObject == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);

        // 스케일
        Vector2 scale = mouseWorldPos - drawStartPosition;
        scale.y *= -1;  // y축 반전

        // 그림자 오브젝트 스케일 변경
        currentDrawedShadowObject.transform.localScale = scale;
    }

    protected override void OnMouseUp(Vector3 mousePosition, int mouseIndex = 0)
    {
        base.OnMouseUp(mousePosition, mouseIndex);

        currentDrawedShadowObject = null;
    }
}

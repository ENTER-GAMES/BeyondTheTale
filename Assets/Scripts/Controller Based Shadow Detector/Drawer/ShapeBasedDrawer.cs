using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeBasedDrawer : Drawer
{
    [SerializeField]
    private GameObject rectPrefab;        // 그려질 그림자 오브젝트 프리팹
    private GameObject currentRect;       // 현재 그려지고 있는 그림자 오브젝트 

    private Vector3 startDrawPosition;          // 그리기 시작한 위치 (월드)
    private bool startDraw = false;

    public override void Select()
    {
        base.Select();

        startDraw = false;
    }

    public override void Deselect()
    {
        base.Deselect();

        DeleteRect();
    }

    protected override void OnMouseDown(Vector3 mousePosition, int mouseIndex = 0)
    {
        base.OnMouseDown(mousePosition, mouseIndex);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);

        // 그리기 시작한 위치 지정
        startDraw = true;
        startDrawPosition = mouseWorldPos;
        startDrawPosition.z = 0;  // z축 0으로 (원래는 카메라 위치)

        // 그림자 오브젝트 생성
        currentRect = Instantiate(rectPrefab, startDrawPosition, Quaternion.identity);
    }

    protected override void OnMouse(Vector3 mousePosition, int mouseIndex = 0)
    {
        base.OnMouse(mousePosition, mouseIndex);
        if (!startDraw) return;
        if (currentRect == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);

        // 스케일
        Vector2 scale = mouseWorldPos - startDrawPosition;
        scale.y *= -1;  // y축 반전

        // 그림자 오브젝트 스케일 변경
        currentRect.transform.localScale = scale;
    }

    protected override void OnMouseUp(Vector3 mousePosition, int mouseIndex = 0)
    {
        base.OnMouseUp(mousePosition, mouseIndex);
        if (!startDraw) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);

        DeleteRect();

        Shadow shadow = new Shadow(new List<Vector3>(){
            new Vector3(startDrawPosition.x, startDrawPosition.y, 0),
            new Vector3(startDrawPosition.x, mouseWorldPos.y, 0),
            new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0),
            new Vector3(mouseWorldPos.x, startDrawPosition.y, 0)
        });

        GameObject drawedShadowObjGameObj = Instantiate(drawedShadowObjectPrefab, Vector3.zero, Quaternion.identity);
        DrawedShadowObject drawedShadowObj = drawedShadowObjGameObj.GetComponent<DrawedShadowObject>();

        drawedShadowObj.Init(shadow);
    }

    private void DeleteRect()
    {
        Destroy(currentRect);
        currentRect = null;
    }
}

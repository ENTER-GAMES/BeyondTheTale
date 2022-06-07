using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ControllerBasedShadowDetector : ShadowDetector
{
    /* Enum */
    [Serializable]
    public enum ToolType { Selector, Drawer, Eraser }
    [Serializable]
    public enum DrawType { Point, Line, Rect, Circle }

    /* Property */
    [Header("[Controller Based Shadow Detector]")]

    [Header("Tools")]
    [SerializeField]
    private ToolType toolType = ToolType.Drawer;
    private ToolType preToolType = ToolType.Drawer;                 // 직전 도구 타입

    [Header("Drawer")]
    [SerializeField]
    private DrawType drawType = DrawType.Point;
    private DrawType preDrawType = DrawType.Point;                  // 직전 그리기 타입

    [Header("Point")]
    [SerializeField]
    private GameObject pointPrefab;
    private List<GameObject> pointObjectList = new List<GameObject>();

    /* Variable */
    private List<Shadow> shadowList = new List<Shadow>();           // 그림자 리스트

    [SerializeField]
    private List<Vector3> pointList = new List<Vector3>();          // 포인트 리스트
    private bool isDrawing = false;                                 // 그려지고 있는지 여부

    private void Awake()
    {
        // 직전 타입 저장
        preToolType = toolType;
        preDrawType = drawType;
    }

    private void Update()
    {
        CheckChangedToolType();     // 도구 타입이 변경되었는지 확인
        CheckChangedDrawType();     // 그리기 타입이 변경되었는지 확인


        // 선택된 도구에 맞게 Update
        if (toolType == ToolType.Selector)
            UpdateSelector();
        else if (toolType == ToolType.Drawer)
            UpdateDrawer();
        else if (toolType == ToolType.Eraser)
            UpdateEraser();
    }

    private void CheckChangedToolType()
    {
        if (toolType == preToolType) return;

        preToolType = toolType;
        isDrawing = false;
        pointList.Clear();
    }

    private void CheckChangedDrawType()
    {
        if (drawType == preDrawType) return;

        preDrawType = drawType;
        isDrawing = false;
        pointList.Clear();
    }

    private void UpdateSelector()
    {

    }

    private void UpdateDrawer()
    {
        if (drawType == DrawType.Point)
            UpdateDrawerPoint();
    }

    private void UpdateDrawerPoint()
    {
        // 마우스 버튼 클릭
        if (Input.GetMouseButtonDown(0))
        {
            // 클릭 위치
            Vector3 clickPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPoint.z = 0;
            pointList.Add(clickPoint);

            // 포인트 오브젝트 생성
            GameObject pointObj = Instantiate(pointPrefab, clickPoint, Quaternion.identity);
            pointObjectList.Add(pointObj);
        }

        // 엔터
        if (Input.GetKeyDown(KeyCode.Return) && pointList.Count > 0)
        {
            // 그림자 생성
            Shadow shadow = new Shadow(new List<Vector3>(pointList));
            shadowList.Add(shadow);
            MeshDrawer.Draw(shadow);

            // 포인트 및 포인트 오브젝트 삭제
            pointList.Clear();
            ClearPointObjects();
        }
    }

    private void ClearPointObjects()
    {
        foreach (var pointObj in pointObjectList)
        {
            GameObject.Destroy(pointObj);
        }
    }

    private void UpdateEraser()
    {

    }

    private void UpdateDrawMesh()
    {
    }
}

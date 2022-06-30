using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBasedDrawer : Drawer
{
    [SerializeField]
    private GameObject pointPrefab;                                     // 그림자 오브젝트 포인트 프리팹
    private List<GameObject> pointObjects = new List<GameObject>();     // 그림자 오브젝트 포인트 리스트
    private List<Vector3> points = new List<Vector3>();                 // 포인트 좌표 리스트

    [SerializeField]
    private float addPointDeltaTime;        // 몇 초 후에 다음 포인트 추가됨 (드래그 기능을 위해서 추가됨)
    private float lastAddPointTime;         // 마지막으로 포인트 추가된 시간

    [Header("Key Code")]
    [SerializeField]
    private KeyCode deleteAllPointsKey = KeyCode.Backspace;                 // 포인트 삭제 키

    public override void Select()
    {
        base.Select();
        DeleteAllPoints();
    }

    public override void Deselect()
    {
        base.Deselect();
        DeleteAllPoints();
    }

    protected override void Update()
    {
        base.Update();

        // 그리기 초기화
        if (Input.GetKeyDown(deleteAllPointsKey))
            DeleteAllPoints();
    }

    protected override void OnMouseDown(Vector3 mousePosition, int mouseIndex = 0)
    {
        base.OnMouseDown(mousePosition, mouseIndex);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        mouseWorldPos.z = 0;

        CreatePoint(mouseWorldPos);
    }

    protected override void OnMouse(Vector3 mousePosition, int mouseIndex = 0)
    {
        if (Time.time - lastAddPointTime < addPointDeltaTime) return;

        base.OnMouse(mousePosition, mouseIndex);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        mouseWorldPos.z = 0;

        CreatePoint(mouseWorldPos);
    }

    protected override void OnMouseUp(Vector3 mousePosition, int mouseIndex = 0)
    {
        base.OnMouseUp(mousePosition, mouseIndex);

        CreateShadow();
    }

    private void CreatePoint(Vector3 position)
    {
        points.Add(position);

        GameObject pointObject = Instantiate(pointPrefab, position, Quaternion.identity);
        pointObjects.Add(pointObject);

        lastAddPointTime = Time.time;
    }

    private void CreateShadow()
    {
        Shadow shadow = new Shadow(new List<Vector3>(points));

        detector?.AddShadow(shadow);

        DeleteAllPoints();
    }

    private void DeleteAllPoints()
    {
        points.Clear();

        foreach (GameObject pointObj in pointObjects)
            Destroy(pointObj);
        pointObjects.Clear();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawedShadowObject : MonoBehaviour
{
    [SerializeField]
    private Shadow shadow;
    [SerializeField]
    private MeshFilter meshFilter;
    [SerializeField]
    private PolygonCollider2D polygonCollider;
    public Bounds Bounds => polygonCollider.bounds;

    [Header("Render")]
    [SerializeField]
    private Material defaultMaterial;
    [SerializeField]
    private Material selectMaterial;
    [SerializeField]
    private MeshRenderer meshRenderer;

    public void Select()
    {
        if (meshRenderer != null)
            meshRenderer.material = selectMaterial;
    }

    public void Deselect()
    {
        if (meshRenderer != null)
            meshRenderer.material = defaultMaterial;
    }

    public void Init(Shadow shadow)
    {
        this.shadow = shadow;
        DrawShadow();
    }

    [ContextMenu("Draw Shadow")]
    private void DrawShadow()
    {
        // null 체크
        if (shadow == null || shadow.points == null || shadow.points.Count < 3)
        {
            Debug.Log("Define 2D polygon in 'poly' in the the Inspector");
            return;
        }


        /*---------- 콜라이더 ----------*/
        // 중앙 찾기
        Vector3 center = FindCenter(shadow.points.ToArray());

        // List<Vector3> -> Vector2[]
        Vector2[] points = new Vector2[shadow.points.Count];

        for (int i = 0; i < shadow.points.Count; i++)
            points[i] = shadow.points[i] - center;

        // 폴리곤 콜라이더 포인트 지정
        polygonCollider.points = points;


        /*---------- 메쉬 ----------*/
        // 메쉬 생성
        Mesh mesh = polygonCollider.CreateMesh(false, false);

        // 콜라이더와 메쉬 좌표 맞추기
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            mesh.vertices[i] -= center;
        }

        // 메쉬 지정
        meshFilter.mesh = mesh;

        // 메쉬 계산
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        transform.position = center;
    }

    private Vector3 FindCenter(Vector3[] poly)
    {
        Vector3 center = Vector3.zero;
        foreach (Vector3 v3 in poly)
        {
            center += v3;
        }
        return center / poly.Length;
    }
}

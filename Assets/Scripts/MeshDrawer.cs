using OpenCVForUnity.CoreModule;
using System.Collections.Generic;
using UnityEngine;

public class MeshDrawer : MonoBehaviour
{
    private List<GameObject> polys;
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private List<Vector3> ver;
    private List<Vector2> uv;
    private List<int> tri;

    public void Draw(Shadow shadow)
    {
        if (shadow.points.Count < 3)
            return;

        CreateObject();
        SettingsMesh(shadow.points);
        CreateMesh();
    }

    public void Clear()
    {
        if (polys == null)
            polys = new List<GameObject>();
        else
        {
            for (int i = 0; i < polys.Count; i++)
                Destroy(polys[i]);
            polys.Clear();
        }
    }

    private void CreateObject()
    {
        GameObject poly = new GameObject("Poly");
        meshFilter = poly.AddComponent<MeshFilter>();
        meshRenderer = poly.AddComponent<MeshRenderer>();
        polys.Add(poly);
    }

    private void SettingsMesh(List<Vector3> points)
    {
        InitMeshData();
        AddFirstTriangle();

        for (int i = 0; i < points.Count; i++)
        {
            AddVertex(points[i]);
            AddUV(Vector2.zero);
            if (i >= 3)
                AddTriangle();
            if (i == points.Count - 1)
                AddLastTriangle();
        }
    }

    private void InitMeshData()
    {
        ver = new List<Vector3>();
        uv = new List<Vector2>();
        tri = new List<int>();
    }

    private void AddVertex(Vector3 pt) { ver.Add(new Vector2(pt.x, pt.y)); }

    private void AddUV(Vector2 v) { uv.Add(v); }

    private void AddFirstTriangle() { tri.AddRange(new List<int> {0, 2, 1}); }

    private void AddTriangle() { tri.AddRange(new List<int> {0, ver.Count - 1, ver.Count - 2}); }

    private void AddLastTriangle() { tri.AddRange(new List<int> {0, 1, ver.Count - 1}); }


    private void CreateMesh()
    {
        mesh = new Mesh();
        mesh.vertices = ver.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = tri.ToArray();
        mesh.MarkDynamic();
        meshFilter.mesh = mesh;
    }
}
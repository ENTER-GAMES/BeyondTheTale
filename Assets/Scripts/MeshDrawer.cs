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

    public void Draw(List<Point> pts)
    {
        if (pts.Count < 3)
            return;

        CreateObject();
        SettingsMesh(pts);
        CreateMesh();
    }

    public void Clear()
    {
        if (polys == null)
        {
            polys = new List<GameObject>();
        }
        else
        {
            for (int i = 0; i < polys.Count; i++)
            {
                Destroy(polys[i]);
            }
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

    private void SettingsMesh(List<Point> pts)
    {
        InitMeshData();
        AddFirstTriangle();

        for (int i = 0; i < pts.Count; i++)
        {
            AddVertex(pts[i]);
            AddUV(Vector2.zero);
            if (i >= 3)
                AddTriangle();
            if (i == pts.Count - 1)
                AddLastTriangle();
        }
    }

    private void InitMeshData()
    {
        ver = new List<Vector3>();
        uv = new List<Vector2>();
        tri = new List<int>();
    }

    private void AddVertex(Point pt) { ver.Add(new Vector2((float)pt.x, (float)pt.y)); }

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
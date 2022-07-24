using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowObject : MonoBehaviour
{
    protected Shadow shadow;
    public Shadow Shadow => shadow;
    [SerializeField]
    protected MeshFilter meshFilter;
    [SerializeField]
    protected PolygonCollider2D polygonCollider2D;

    public void Init(Shadow shadow)
    {
        this.shadow = shadow;
        DrawMesh();
    }

    private void DrawMesh()
    {
        polygonCollider2D.points = shadow.points;
        Mesh mesh = polygonCollider2D.CreateMesh(false, false);
        meshFilter.mesh = mesh;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}

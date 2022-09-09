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
    public Bounds bounds => polygonCollider2D.bounds;
    [SerializeField]
    protected MeshRenderer meshRenderer;

    public void Init(Shadow shadow)
    {
        this.shadow = shadow;
        Activate();
        DrawMesh();
    }

    private void DrawMesh()
    {
        polygonCollider2D.points = shadow.points;
        Mesh mesh = polygonCollider2D.CreateMesh(false, false);
        if (mesh == null)
            return;
        meshFilter.mesh = mesh;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    public void Deactivate()
    {
        meshRenderer.enabled = false;
        polygonCollider2D.enabled = false;
    }

    public void Activate()
    {
        meshRenderer.enabled = true;
        polygonCollider2D.enabled = true;
    }
}

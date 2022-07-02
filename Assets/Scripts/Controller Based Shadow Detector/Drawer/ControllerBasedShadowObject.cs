using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBasedShadowObject : ShadowObject
{
    public Bounds Bounds => polygonCollider2D.bounds;

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
}

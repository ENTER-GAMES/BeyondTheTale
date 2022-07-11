using System.Collections.Generic;
using UnityEngine;

public class MeshDrawer : MonoBehaviour
{
    [SerializeField]
    private ShadowObject shadowObjectPrefab;
    private List<ShadowObject> shadowObjects = new List<ShadowObject>();

    public void Draw(List<Shadow> shadows)
    {
        foreach (Shadow shadow in shadows)
            CreateObject(shadow);
    }

    public void Clear()
    {
        foreach (ShadowObject so in shadowObjects)
            Destroy(so.gameObject);
        shadowObjects.Clear();
    }

    private void CreateObject(Shadow shadow)
    {
        if (shadow.points.Length < 3)
            return;

        ShadowObject clone = Instantiate(shadowObjectPrefab);
        clone.Init(shadow);
        shadowObjects.Add(clone);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDrawerPool : MeshDrawer
{
    private List<ShadowObject> shadowPool = new List<ShadowObject>();

    public override void Draw(List<Shadow> shadows)
    {
        foreach (Shadow shadow in shadows)
        {
            if (shadowPool.Count > 0)
            {
                InitObject(shadowPool[0], shadow);
                shadowPool.RemoveAt(0);
            }
            else
            {
                CreateObject(shadow);
            }
        }
    }

    public override void Clear()
    {
        foreach (ShadowObject so in shadowObjects)
        {
            shadowPool.Add(so);
            so.Deactivate();
        }
        shadowObjects.Clear();
    }

    private void InitObject(ShadowObject shadowObject, Shadow shadow)
    {
        if (shadow.points.Length < 3)
            return;

        shadowObject.Init(shadow);
        shadowObjects.Add(shadowObject);
    }
}

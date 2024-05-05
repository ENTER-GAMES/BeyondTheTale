using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDrawerPool : MeshDrawer
{
    private List<ShadowObject> shadowPool = new List<ShadowObject>();

    public override void Draw(List<Shadow> shadows)
    {
        int newShadowCount = 0;

        foreach (Shadow shadow in shadows)
        {
            if (shadowPool.Count > 0)
            {
                if (InitObject(shadowPool[0], shadow))
                {
                    shadowPool.RemoveAt(0);
                    newShadowCount++;
                }
            }
            else
            {
                if (CreateObject(shadow))
                    newShadowCount++;
            }
        }

        shadowCount = newShadowCount;
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

    private bool InitObject(ShadowObject shadowObject, Shadow shadow)
    {
        if (shadow.points.Length < 3)
            return false;

        shadowObject.Init(shadow);
        shadowObjects.Add(shadowObject);
        return true;
    }
}

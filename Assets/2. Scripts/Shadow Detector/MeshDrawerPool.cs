using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDrawerPool : MeshDrawer
{
    public override void Draw(List<Shadow> shadows)
    {
        int newShadowCount = 0;

        foreach (Shadow shadow in shadows)
        {
            ShadowObject shadowObject = null;
            for (int i = 0; i < shadowObjects.Count; i++)
            {
                if (!shadowObjects[i].IsActive())
                {
                    shadowObject = shadowObjects[i];
                    break;
                }    
            }

            if (shadowObject != null)
                InitObject(shadowObject, shadow);
            else
                CreateObject(shadow);

            newShadowCount++;
        }

        shadowCount = newShadowCount;
    }

    public override void Clear()
    {
        foreach (ShadowObject so in shadowObjects)
        {
            so.Deactivate();
        }
    }

    private bool InitObject(ShadowObject shadowObject, Shadow shadow)
    {
        if (shadow.points.Length < 3)
            return false;

        shadowObject.Init(shadow);
        return true;
    }
}

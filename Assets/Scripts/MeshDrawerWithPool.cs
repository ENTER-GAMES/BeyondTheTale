using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDrawerWithPool : MeshDrawer
{
    [SerializeField]
    private int poolSize = 10;
    private static readonly Shadow privateShadow = new Shadow(
        new Vector2[] {
            new Vector2(100, 0),
            new Vector2(100, 1),
            new Vector2(101, 2),
        }
    );

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            ShadowObject obj = Instantiate(shadowObjectPrefab);
            obj.Init(privateShadow);
            shadowObjects.Add(obj);
        }
    }

    public override void Draw(List<Shadow> shadows)
    {
        for (int i = 0; i < poolSize; i++)
        {
            ShadowObject obj = shadowObjects[i];

            if (i < shadows.Count)
            {
                Shadow shadow = shadows[i];
                obj.Init(shadow);
            }
            else
            {
                if (obj.Shadow == privateShadow) return;
                obj.Init(privateShadow);
            }
        }
    }

    public override void Clear()
    {
        for (int i = 0; i < poolSize; i++)
        {
            ShadowObject obj = shadowObjects[i];
            if (obj.Shadow == privateShadow) return;
            obj.Init(privateShadow);
        }
    }
}

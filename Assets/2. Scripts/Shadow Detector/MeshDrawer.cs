using System.Collections.Generic;
using UnityEngine;

public class MeshDrawer : MonoBehaviour
{
    protected static MeshDrawer instance;

    protected int shadowCount = 0;
    public static int ShadowCount => instance ? instance.shadowCount : 0;

    [SerializeField]
    protected ShadowObject shadowObjectPrefab;
    protected List<ShadowObject> shadowObjects = new List<ShadowObject>();

    protected virtual void Awake() => instance = this;

    public virtual void Draw(List<Shadow> shadows)
    {
        int newShadowCount = 0;

        // 그림자 생성
        foreach (Shadow shadow in shadows)
        {
            if (CreateObject(shadow))
                newShadowCount++;
        }

        shadowCount = newShadowCount;
    }

    public virtual void Clear()
    {
        foreach (ShadowObject so in shadowObjects)
            Destroy(so.gameObject);
        shadowObjects.Clear();
    }

    protected bool CreateObject(Shadow shadow)
    {
        if (shadow.points.Length < 3)
            return false;

        ShadowObject clone = Instantiate(shadowObjectPrefab);
        clone.Init(shadow);
        shadowObjects.Add(clone);
        return true;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatManager : MonoBehaviour
{
    private int hitCount = 0;
    private int screamCount = 0;

    [SerializeField]
    private List<Cat> cats;
    private List<Cat> activeCats;
    private List<Cat> deactiveCats;
    [SerializeField]
    private int maxActiveNum = 1;

    private void Start()
    {
        foreach (Cat cat in cats)
            cat.Init(this);

        activeCats = new List<Cat>();
        deactiveCats = new List<Cat>();
    }

    private void Activate()
    {
        if (activeCats.Count != 0)
            return;

        for (int i = activeCats.Count; i < maxActiveNum; i++)
        {
            Cat cat = deactiveCats[Random.Range(0, deactiveCats.Count)];
            deactiveCats.Remove(cat);
            activeCats.Add(cat);
            cat.Activate();
        }
    }

    public void OnHit()
    {
        hitCount++;
    }

    public void OnScream()
    {
        screamCount++;
    }

    public void OnClose(Cat cat)
    {
        activeCats.Remove(cat);
        deactiveCats.Add(cat);
        Activate();
    }
}

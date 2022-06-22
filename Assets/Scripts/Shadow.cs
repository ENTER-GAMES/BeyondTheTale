using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Shadow
{
    public List<Vector3> points;

    public Shadow(List<Vector3> points)
    {
        this.points = points;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Shadow
{
    public Vector2[] points;

    public Shadow(Vector2[] points)
    {
        this.points = points;
    }

    public Shadow(List<Vector3> points)
    {
        Vector2[] pointsArr = new Vector2[points.Count];

        for (int i = 0; i < pointsArr.Length; i++)
            pointsArr[i] = points[i];

        this.points = pointsArr;
    }
}

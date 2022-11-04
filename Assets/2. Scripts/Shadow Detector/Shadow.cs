using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Shadow
{
    public Vector2[] points;
    public double area;

    public Shadow(Vector2[] points) : this(points, 0)
    {
    }

    public Shadow(List<Vector3> points) : this(points, 0)
    {
    }

    public Shadow(Vector2[] points, double area)
    {
        this.points = points;
        this.area = area;
    }

    public Shadow(List<Vector3> points, double area)
    {
        Vector2[] pointsArr = new Vector2[points.Count];

        for (int i = 0; i < pointsArr.Length; i++)
            pointsArr[i] = points[i];

        this.points = pointsArr;
        this.area = area;
    }
}

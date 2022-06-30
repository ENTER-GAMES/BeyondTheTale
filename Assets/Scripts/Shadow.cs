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
}

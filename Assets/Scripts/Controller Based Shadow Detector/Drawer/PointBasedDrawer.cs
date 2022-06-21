using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBasedDrawer : Drawer
{
    protected override void OnMouseDown(Vector3 mousePosition, int mouseIndex = 0)
    {
        base.OnMouseDown(mousePosition, mouseIndex);

        print(mousePosition);
    }
}

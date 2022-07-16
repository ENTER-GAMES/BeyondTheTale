using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Alice_5_1 : Character_5_1, ISpotLight2DTarget
{
    public void OnHitBySpotLight2D()
    {
        Restart();
    }

    private void Restart()
    {
        transform.position = spawnPosition;
        moveDirection = Vector3.right;
    }
}

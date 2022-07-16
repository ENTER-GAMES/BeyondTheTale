using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPoint : Point<IJumpable>
{
    [Header("Value")]
    [SerializeField]
    protected bool useValue = false;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float moveSpeed;

    protected override void Hit(IJumpable target)
    {
        if (useValue)
            target.Jump(jumpForce, moveSpeed);
        else
            target.Jump();
    }
}

public interface IJumpable : IAbleForPoint
{
    public void Jump();
    public void Jump(float jumpForce);
    public void Jump(float jumpForce, float moveSpeedDuringJump);
}

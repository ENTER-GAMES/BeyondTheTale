using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPoint : Point<IJumpable>
{
    [Header("Value")]
    [SerializeField]
    protected bool useJumpForce = false;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    protected bool useMoveSpeed = false;
    [SerializeField]
    private float moveSpeed;

    protected override void Hit(IJumpable target)
    {
        if (useJumpForce && useMoveSpeed)
            target.Jump(jumpForce, moveSpeed);
        else if (useJumpForce)
            target.Jump(jumpForce);
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

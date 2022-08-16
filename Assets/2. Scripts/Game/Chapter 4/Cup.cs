using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cup : MonoBehaviour
{
    [SerializeField]
    private CupChecker cupChecker;

    [SerializeField]
    private bool isCollisionTable = false;

    private void Awake()
    {
        if (cupChecker == null)
            cupChecker = GetComponentInChildren<CupChecker>();
    }

    public bool IsSuccess() => cupChecker.IsSuccess() && isCollisionTable;

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Table"))
            isCollisionTable = true;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Table"))
            isCollisionTable = false;
    }
}

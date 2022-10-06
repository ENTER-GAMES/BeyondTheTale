using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cup : MonoBehaviour
{
    private CupChecker cupChecker;

    private void Awake()
    {
        if (cupChecker == null)
            cupChecker = GetComponentInChildren<CupChecker>();
    }

    public bool IsSuccess() => cupChecker.IsSuccess();
}

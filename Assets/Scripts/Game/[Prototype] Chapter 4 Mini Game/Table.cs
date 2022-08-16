using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Table : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onComplete = new UnityEvent();

    [SerializeField]
    private Cup[] cups;

    [SerializeField]
    private bool isSuccess = false;

    private void Update()
    {
        if (isSuccess) return;


        foreach (Cup cup in cups)
            if (!cup.IsSuccess()) return;

        isSuccess = true;
        onComplete.Invoke();
        print("Table: Success!");
    }
}

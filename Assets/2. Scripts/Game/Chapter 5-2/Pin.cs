using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pin : MonoBehaviour
{
    [SerializeField]
    private int[] targetIndexs;         // key: 다이얼 인덱스, value: 다이얼 내의 타겟 인덱스
    private List<bool> results = new List<bool>();

    public UnityEvent onComplete = new UnityEvent();

    private void Awake()
    {
        for (int i = 0; i < targetIndexs.Length; i++)
            results.Add(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<DialCode>(out DialCode dialCode)) return;

        int dialIndex = dialCode.DialIndex;
        if (dialIndex >= targetIndexs.Length) return;

        results[dialIndex] = targetIndexs[dialIndex] == dialCode.DialCodeIndex;

        if (CheckComplete())
        {
            onComplete.Invoke();
            print("Complete");
        }
    }

    private bool CheckComplete()
    {
        foreach (bool result in results)
            if (!result) return false;

        return true;
    }
}

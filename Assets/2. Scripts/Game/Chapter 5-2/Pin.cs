using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pin : MonoBehaviour
{
    private int[] targetIndexs;         // key: 다이얼 인덱스, value: 다이얼 내의 타겟 인덱스
    private List<bool> results = new List<bool>();

    public UnityEvent onComplete = new UnityEvent();
    private bool hasInitDone = false;

    public void Init(int[] targetIndexs)
    {
        this.targetIndexs = targetIndexs;
        for (int i = 0; i < this.targetIndexs.Length; i++)
            results.Add(false);

        hasInitDone = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasInitDone) return;

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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!hasInitDone) return;

        if (!other.TryGetComponent<DialCode>(out DialCode dialCode)) return;

        int dialIndex = dialCode.DialIndex;
        if (dialIndex >= targetIndexs.Length) return;

        results[dialIndex] = false;
    }

    private bool CheckComplete()
    {
        foreach (bool result in results)
            if (!result) return false;

        return true;
    }
}

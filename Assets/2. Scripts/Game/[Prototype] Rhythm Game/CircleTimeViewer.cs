using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CircleTimeViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void SetTime(float time)
    {
        text.text = time.ToString("0.0");
    }
}

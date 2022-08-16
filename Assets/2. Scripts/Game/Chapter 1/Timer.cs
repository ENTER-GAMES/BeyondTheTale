using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    private TextMeshProUGUI text;
    private float timer = 0;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        text.text = string.Format ("{0:N2}", timer);
    }
}

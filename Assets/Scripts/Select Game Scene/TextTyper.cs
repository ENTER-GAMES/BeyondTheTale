using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class TextTyper : MonoBehaviour
{
    private TextMeshPro textMeshPro;
    [SerializeField]
    private float charDelayTime = 0.1f;
    [SerializeField]
    private float spaceDelayTime = 0.2f;

    private string message;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        message = textMeshPro.text;
    }

    private void OnEnable()
    {
        textMeshPro.text = "";
    }

    public void Type()
    {
        StopAllCoroutines();
        StartCoroutine(ExplainRoutine());
    }

    private IEnumerator ExplainRoutine()
    {
        textMeshPro.text = "";

        for (int i = 0; i < message.Length; i++)
        {
            if (message[i].Equals('<'))
            {
                string tag = "";
                int j = i;
                while (true)
                {
                    char c = message[j];
                    tag += c;
                    if (c.Equals('>')) break;
                    j++;
                }
                textMeshPro.text += tag;
                i = j;
                continue;
            }

            textMeshPro.text += message[i];

            if (message[i].Equals(' ')) yield return new WaitForSeconds(spaceDelayTime);
            else yield return new WaitForSeconds(charDelayTime);
        }
    }
}

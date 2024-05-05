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

    [Header("Narration")]
    [SerializeField]
    private AudioClip narrationAudioClip;
    private AudioSource audioSource;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        audioSource = GetComponent<AudioSource>();
        message = textMeshPro.text;
    }

    public void Type()
    {
        audioSource.Stop();
        audioSource.clip = narrationAudioClip;
        audioSource.Play();

        StopAllCoroutines();
        StartCoroutine(ExplainRoutine());
    }

    public void RemoveAllText()
    {
        StopAllCoroutines();
        textMeshPro.text = "";
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

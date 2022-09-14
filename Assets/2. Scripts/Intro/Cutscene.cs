using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer[] spriteRenderers;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void SetOpacityOfAllRenderers(float a)
    {
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            Color newColor = renderer.color;
            newColor.a = a;
            renderer.color = newColor;
        }
    }

    public void PlayAnimation()
    {
        animator.Play("Animation");
    }
}

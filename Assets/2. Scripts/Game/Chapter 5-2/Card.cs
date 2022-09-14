using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private bool isTarget = false;

    private bool isCollision = false;

    private BoxCollider2D boxCollider2D;
    private AudioSource audioSource;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!isTarget) return;

        Collider2D collider = Physics2D.OverlapBox(transform.position, boxCollider2D.size, transform.eulerAngles.z, LayerMask.GetMask("Shadow Object"));

        if (collider == null)
            isCollision = false;

        else if (isCollision == false)
        {
            if (audioSource.isPlaying == false)
                audioSource.Play();

            isCollision = true;
        }
    }

    public void SetIsTarget(bool value) => isTarget = value;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PageTriggerText : MonoBehaviour
{
    private int triggerShadowCount = 0;
    private bool isTriggered = false;
    public bool IsTriggered => isTriggered;

    [SerializeField]
    private LayerMask targetLayerMask;

    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        UpdateTriggerShadow();
    }

    private void UpdateTriggerShadow()
    {
        Bounds bounds = boxCollider2D.bounds;
        Collider2D hit = Physics2D.OverlapBox(bounds.center, bounds.size, 0, targetLayerMask);

        isTriggered = hit != null;
    }

    public void IsSelected(bool isSelected)
    {
        if (isSelected)
            transform.localScale = Vector3.one * 1.5f;
        else
            transform.localScale = Vector3.one;
    }
}

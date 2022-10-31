using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PageTriggerText : MonoBehaviour
{
    private int triggerShadowCount = 0;
    private bool isTriggered = false;
    public bool IsTriggered => isTriggered;

    private bool isSelected = false;            // 선택되었는지 여부

    [SerializeField]
    private LayerMask targetLayerMask;

    [SerializeField]
    private Transform bookmarkTransform;

    [Header("Move Up")]
    [SerializeField]
    private float moveUpTime;
    [SerializeField]
    private Vector3 moveUpTargetLocalPosition;
    [SerializeField]
    private AnimationCurve moveUpAnimationCurve;

    [Header("Move Down")]
    [SerializeField]
    private float moveDownTime;
    [SerializeField]
    private Vector3 moveDownTargetLocalPosition;
    [SerializeField]
    private AnimationCurve moveDownAnimationCurve;

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
        if (this.isSelected == isSelected) return;

        this.isSelected = isSelected;
        StopAllCoroutines();

        if (isSelected)
            StartCoroutine(MoveBookmark(moveDownTime, moveDownTargetLocalPosition, moveDownAnimationCurve));
        else
            StartCoroutine(MoveBookmark(moveUpTime, moveUpTargetLocalPosition, moveUpAnimationCurve));
    }

    private IEnumerator MoveBookmark(float moveTime, Vector3 to, AnimationCurve curve)
    {
        float timer = 0, percent = 0;

        Vector3 startPos = bookmarkTransform.localPosition;
        while (percent < 1)
        {
            timer += Time.deltaTime;
            percent = timer / moveTime;

            Vector3 newPos = Vector3.Lerp(startPos, to, curve.Evaluate(percent));
            bookmarkTransform.localPosition = newPos;

            yield return null;
        }
    }

    [ContextMenu("Set Move Up Target Local Position")]
    private void SetMoveUpTargetLocalPosition()
    {
        moveUpTargetLocalPosition = bookmarkTransform.localPosition;
    }

    [ContextMenu("Set Move Down Target Local Position")]
    private void SetMoveDownTargetLocalPosition()
    {
        moveDownTargetLocalPosition = bookmarkTransform.localPosition;
    }
}

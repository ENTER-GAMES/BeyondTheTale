using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Point<T> : MonoBehaviour where T : IAbleForPoint
{
    [Header("@Gizmos: Color")]
    [SerializeField]
    private Color gizmoColor = Color.black;

    [Header("Target")]
    [SerializeField]
    private List<string> targetTags;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<T>(out T target)) return;        // 컴포넌트 존재 여부 확인
        if (!targetTags.Contains(other.tag)) return;                // 타겟 태그 확인

        Hit(target);
    }

    protected abstract void Hit(T target);

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
}

public interface IAbleForPoint { }
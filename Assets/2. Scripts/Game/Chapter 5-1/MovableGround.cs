using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableGround : MonoBehaviour
{
    [SerializeField]
    private MovableGroundProperty[] properties;

    public void MoveToLastPosition()
    {
        // transform.position = originPosition;
        StopAllCoroutines();
        StartCoroutine(MoveOnceRoutine(properties[properties.Length - 1]));
    }

    [ContextMenu("Move")]
    public void Move()
    {
        StopAllCoroutines();
        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        foreach (MovableGroundProperty property in properties)
        {
            yield return StartCoroutine(MoveOnceRoutine(property));
            yield return new WaitForSeconds(property.nextDelayTime);
        }
    }

    private IEnumerator MoveOnceRoutine(MovableGroundProperty property)
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = property.targetTransform.position;

        float time = Vector3.Distance(startPosition, endPosition) / property.moveSpeed;
        float timer = 0;
        float percent = 0;
        while (percent < 1)
        {
            timer += Time.deltaTime;
            percent = timer / time;

            transform.position = Vector3.Lerp(startPosition, endPosition, percent);

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Transform prevTransform = transform;
        foreach (MovableGroundProperty property in properties)
        {
            Gizmos.DrawLine(prevTransform.position, property.targetTransform.position);
            prevTransform = property.targetTransform;
        }
    }

    [Serializable]
    public struct MovableGroundProperty
    {
        public Transform targetTransform;
        public float moveSpeed;
        public float nextDelayTime;
    }
}

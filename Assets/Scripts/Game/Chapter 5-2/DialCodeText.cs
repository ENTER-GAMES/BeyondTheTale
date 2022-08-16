using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialCodeText : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform;

    private void LateUpdate()
    {
        transform.SetPositionAndRotation(
            Camera.main.WorldToScreenPoint(targetTransform.position),
            targetTransform.rotation
        );
    }
}

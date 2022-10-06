using UnityEngine;

public class DialCodeText : MonoBehaviour
{
    private Transform targetTransform;

    public void Init(Transform target)
    {
        targetTransform = target;
        gameObject.SetActive(true);
    }

    private void LateUpdate()
    {
        if (!targetTransform)
            return;

        transform.SetPositionAndRotation(
            targetTransform.position,
            targetTransform.rotation
        );
    }
}

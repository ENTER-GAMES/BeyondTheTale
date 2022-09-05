using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageTriggerStarter : MonoBehaviour
{
    [SerializeField]
    private GameObject pageTrigger;
    [SerializeField]
    private Vector3 targetEulerRotation = Vector3.zero;
    [SerializeField]
    private LayerMask targetShadowLayerMask;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetShadowLayerMask) == 0) return;

        pageTrigger.transform.rotation = Quaternion.Euler(targetEulerRotation);
        pageTrigger.SetActive(true);

        transform.parent.gameObject.SetActive(false);
    }
}

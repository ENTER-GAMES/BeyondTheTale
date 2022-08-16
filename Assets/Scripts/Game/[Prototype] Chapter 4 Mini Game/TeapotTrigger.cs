using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeapotTrigger : MonoBehaviour
{
    [SerializeField]
    private Teapot teapot;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("ShadowObject"))
            teapot.StartPour();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ShadowObject"))
            teapot.StopPour();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidParticle : MonoBehaviour
{
    private void Update()
    {
        if (transform.position.x < -9.5 || transform.position.x > 9.5 || transform.position.y < -6 || transform.position.y > 6)
            gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);
    }
}

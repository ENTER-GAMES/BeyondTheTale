using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidParticle : MonoBehaviour
{
    [SerializeField]
    private float waitCollisionTimeAtEnable = 0.5f;      // OnEnable일 때 충돌을 안하는 시간(초)

    private CircleCollider2D circleCollider2D;

    private void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (transform.position.x < -9.5 || transform.position.x > 9.5 || transform.position.y < -6 || transform.position.y > 6)
            gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(nameof(CollisionTurnOnRoutine));
    }

    private IEnumerator CollisionTurnOnRoutine()
    {
        yield return new WaitForSeconds(waitCollisionTimeAtEnable);
        circleCollider2D.enabled = true;
    }

    private void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);
        StopAllCoroutines();
        circleCollider2D.enabled = false;
    }
}

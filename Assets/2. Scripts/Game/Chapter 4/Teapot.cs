using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teapot : MonoBehaviour
{
    [SerializeField]
    private GameObject teaParticlePrefab;
    [SerializeField]
    private float deltaTime;
    [SerializeField]
    private int countAtOnce;
    [SerializeField]
    private float force;

    private bool isPouring = false;

    public void StartPour()
    {
        if (isPouring) return;
        isPouring = true;

        StartCoroutine(nameof(PourRoutine));
    }

    public void StopPour()
    {
        isPouring = false;

        StopAllCoroutines();
    }

    private IEnumerator PourRoutine()
    {
        while (true)
        {
            for (int i = 0; i < countAtOnce; i++)
            {
                GameObject teaParticle = ObjectPooler.SpawnFromPool(teaParticlePrefab.name, transform.position, transform.rotation);

                if (!teaParticle.TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody2D)) continue;

                rigidbody2D.AddForce(transform.up * force, ForceMode2D.Impulse);
            }

            yield return new WaitForSeconds(deltaTime);
        }
    }
}

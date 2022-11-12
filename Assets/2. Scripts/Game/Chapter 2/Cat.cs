using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WindowState
{
    Cat,
    Rabbit
}

public class Cat : MonoBehaviour
{
    private CatManager catManager;
    private Animator animator;
    [SerializeField]
    private float startTimeMin = 0;
    [SerializeField]
    private float startTimeMax = 1;
    [SerializeField]
    private Vector2 colliderSize;

    [Header("Effect")]
    [SerializeField]
    private Transform effectPosition;
    [SerializeField]
    private GameObject effectPrefab;

    private WindowState currentState = WindowState.Cat;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Init(CatManager catManager)
    {
        this.catManager = catManager;
    }

    private void Update()
    {
        CheckShadow();
    }

    private void CheckShadow()
    {
        Collider2D collider = Physics2D.OverlapBox(transform.position, colliderSize, LayerMask.GetMask("Shadow Object"));

        if (collider)
            animator.SetBool("hit", true);
    }

    public void OnDoorOpen()
    {
        catManager.OnDoorOpen();
    }

    public void OnHitCat()
    {
        catManager.OnHitCat();
        Instantiate(effectPrefab, transform.position, Quaternion.identity);
    }

    public void OnHitRabbit()
    {
        catManager.OnHitRabbit();
    }

    public void OnScream()
    {
        catManager.OnScream();
    }

    public void OnClose()
    {
        animator.SetBool("hit", false);
        catManager.OnClose(this);
    }

    public void Activate(WindowState state)
    {
        currentState = state;
        StopAllCoroutines();
        StartCoroutine(ActivateRoutine());
    }

    private IEnumerator ActivateRoutine()
    {
        float time = Random.Range(startTimeMin, startTimeMax);
        yield return new WaitForSeconds(time);
        animator.SetBool("hit", false);
        if (currentState == WindowState.Cat)
            animator.SetTrigger("cat");
        else if (currentState == WindowState.Rabbit)
            animator.SetTrigger("rabbit");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, colliderSize);
    }
}

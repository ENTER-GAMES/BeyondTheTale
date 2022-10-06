using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CircleGenerator : MonoBehaviour
{
    [SerializeField] private Circle circlePrefab;

    [SerializeField] private int maxActiveNum;
    [SerializeField] private float delayMin;
    [SerializeField] private float delayMax;

    [SerializeField] private Vector3 offset;

    private List<Circle> deactiveCircleList = new List<Circle>();
    private List<Circle> activeCircleList = new List<Circle>();

    public UnityEvent<Circle> onCircleFilled = new UnityEvent<Circle>();

    private void Start()
    {
        onCircleFilled.AddListener(OnCircleFilled);
        Generate();
    }

    private void Generate()
    {
        for (int i = activeCircleList.Count; i < maxActiveNum; i++)
            StartCoroutine(GenerateRoutine());
    }

    private IEnumerator GenerateRoutine()
    {
        if (!CheckCircleExist())
            CreateCircle();

        Circle circle = deactiveCircleList[0];
        deactiveCircleList.RemoveAt(0);
        activeCircleList.Add(circle);

        yield return new WaitForSeconds(Random.Range(delayMin, delayMax));

        circle.transform.position = GetRandomPosition();
        circle.gameObject.SetActive(true);
        circle.Play();
    }

    private void OnCircleFilled(Circle circle)
    {
        activeCircleList.Remove(circle);
        deactiveCircleList.Add(circle);
        circle.gameObject.SetActive(false);
        Generate();
    }

    private bool CheckCircleExist()
    {
        if (deactiveCircleList.Count == 0)
            return false;
        else
            return true;
    }

    private void CreateCircle()
    {
        Circle circle = Instantiate(circlePrefab, Vector3.zero, Quaternion.identity);
        circle.gameObject.SetActive(false);
        deactiveCircleList.Add(circle);
        circle.Init(this);
    }

    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(0 + offset.x, Screen.width - offset.x);
        float y = Random.Range(0 + offset.y, Screen.height - offset.y);
        return Camera.main.ScreenToWorldPoint(new Vector3(x, y, Camera.main.transform.position.z * -1));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerFlash : MonoBehaviour
{
    [SerializeField]
    private int startAngle;
    [SerializeField]
    private int endAngle;
    [SerializeField]
    private int rotateDirection;
    [SerializeField]
    private float rotateSpeed;
    private float currentAngle;
    private int targetAngle;
    private int currentDirection;

    private void Awake()
    {
        rotateDirection = rotateDirection >= 0 ? 1 : -1;
        currentDirection = rotateDirection;
        currentAngle = transform.rotation.eulerAngles.z;
        targetAngle = endAngle;
    }

    private void Update()
    {
        currentAngle += currentDirection * rotateSpeed * Time.deltaTime;

        if ((currentDirection == 1 && currentAngle > targetAngle)
         || (currentDirection == -1 && currentAngle < targetAngle))
        {
            currentDirection *= -1;
            targetAngle = targetAngle == startAngle ? endAngle : startAngle;
        }

        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;

public class CameraBasedShadowDetectorSetting : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CameraBasedShadowDetector detector;

    [Header("Camera Point Settings")]
    [SerializeField] private CameraBasedPoint[] cameraBasedPoints = new CameraBasedPoint[4];
    private CameraBasedPoint cameraPointTemp = new CameraBasedPoint();
    private int cameraPointSettingCount = 0;
    public int CameraPointSettingCount => cameraPointSettingCount;

    private bool isSettingMode = false;
    public bool IsSettingMode => isSettingMode;
    private bool isPointActive = false;

    private int screenWidthRatio;
    private int screenHeightRatio;

    public void Init()
    {
        screenWidthRatio = Screen.width / detector.width;
        screenHeightRatio = Screen.height / detector.height;
    }

    private void Update()
    {
        if (!detector.HasInitDone)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            SetSettingMode();

        if (isSettingMode)
        {
            Vector2 mousePos = GetMousePosition();

            if (Input.GetMouseButtonDown(0))
                ActivatePoint();

            if (Input.GetMouseButton(0) && isPointActive)
                UpdatePointPosition(mousePos);

            if (Input.GetMouseButtonUp(1))
                DeactivatePoint();

            if (Input.GetMouseButtonUp(0) && isPointActive)
            {
                ApplyPointPosition();
                CheckNextPoint();
                SetCameraPointTemp();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                cameraPointSettingCount = 0;
                SetCameraPointTemp();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                cameraPointSettingCount = 1;
                SetCameraPointTemp();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                cameraPointSettingCount = 2;
                SetCameraPointTemp();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                cameraPointSettingCount = 3;
                SetCameraPointTemp();
            }

            detector.DrawPerspectivePoint();
        }
    }

    private void SetSettingMode()
    {
        isSettingMode = !isSettingMode;
        detector.rendererFrame.gameObject.SetActive(isSettingMode);
        cameraPointSettingCount = 0;
    }

    private Vector2 GetMousePosition()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x / screenWidthRatio, detector.height - Input.mousePosition.y / screenHeightRatio);
        mousePos.x = Mathf.Clamp(mousePos.x, 0, detector.width);
        mousePos.y = Mathf.Clamp(mousePos.y, 0, detector.height);
        return mousePos;
    }

    private void ActivatePoint()
    {
        isPointActive = true;
    }

    private void UpdatePointPosition(Vector2 mousePos)
    {
        cameraPointTemp.x = mousePos.x;
        cameraPointTemp.y = mousePos.y;
    }

    private void DeactivatePoint()
    {
        SetCameraPointTemp();
        isPointActive = false;
    }

    private void ApplyPointPosition()
    {
        cameraBasedPoints[cameraPointSettingCount].x = cameraPointTemp.x;
        cameraBasedPoints[cameraPointSettingCount].y = cameraPointTemp.y;

    }

    private void CheckNextPoint()
    {
        cameraPointSettingCount++;
        if (cameraPointSettingCount >= cameraBasedPoints.Length)
            cameraPointSettingCount = 0;
    }

    private void SetCameraPointTemp()
    {
        cameraPointTemp.x = cameraBasedPoints[cameraPointSettingCount].x;
        cameraPointTemp.y = cameraBasedPoints[cameraPointSettingCount].y;
    }

    public Point GetCameraBasedPoint(int index)
    {
        if (isSettingMode && index == cameraPointSettingCount)
            return cameraPointTemp.Get();
        else
            return cameraBasedPoints[index].Get();
    }

    public int GetCameraBasedPointLength()
    {
        return cameraBasedPoints.Length;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using TMPro;

[System.Serializable]
public class CameraSettings
{
    [Header("Detector Settings")]
    [Range(-255, 255)]
    public int r = 0;
    [Range(-255, 255)]
    public int g = 0;
    [Range(-255, 255)]
    public int b = 0;
    [Range(0, 255)]
    public int threshold = 20;
    [Range(0.001f, 0.1f)]
    public double epsilon = 0.001f;
    [Range(1, 21)]
    public int gaussian = 1;
    [Range(0, 100000)]
    public int contourMinArea = 0;
    public bool useApprox = false;
    public bool viewShadow = false;
    [Range(1, 10)]
    public int pointRadius = 1;

    public void CopyTo(CameraSettings settings)
    {
        settings.r = r;
        settings.g = g;
        settings.b = b;
        settings.threshold = threshold;
        settings.epsilon = epsilon;
        settings.gaussian = gaussian;
        settings.contourMinArea = contourMinArea;
        settings.useApprox = useApprox;
        settings.viewShadow = viewShadow;
        settings.pointRadius = pointRadius;
    }
}

public class CameraBasedShadowDetectorSetting : MonoBehaviour
{
    private CameraBasedShadowDetector detector;
    private UIManager uiManager;

    [Header("Camera Point Settings")]
    [SerializeField] private CameraBasedPoint[] cameraBasedPoints = new CameraBasedPoint[4];
    private CameraBasedPoint cameraPointTemp = new CameraBasedPoint();
    private int cameraPointSettingCount = 0;
    public int CameraPointSettingCount => cameraPointSettingCount;

    [SerializeField]
    private CameraSettings settings;
    private CameraSettings settingsTemp = new CameraSettings();

    [Header("UI")]
    [SerializeField] private RawImage rawImage;
    [SerializeField] private Slider sliderR;
    [SerializeField] private Slider sliderG;
    [SerializeField] private Slider sliderB;
    [SerializeField] private Slider sliderThreshold;
    [SerializeField] private Slider sliderEpsilon;
    [SerializeField] private Slider sliderGaussian;
    [SerializeField] private Slider sliderContourMinArea;
    [SerializeField] private Toggle toggleUseApprox;
    [SerializeField] private Toggle toggleScreen;
    [SerializeField] private Toggle toggleViewShadow;
    [SerializeField] private Slider sliderPointRadius;
    [SerializeField] private TextMeshProUGUI textR;
    [SerializeField] private TextMeshProUGUI textG;
    [SerializeField] private TextMeshProUGUI textB;
    [SerializeField] private TextMeshProUGUI textThreshold;
    [SerializeField] private TextMeshProUGUI textEpsilon;
    [SerializeField] private TextMeshProUGUI textGaussian;
    [SerializeField] private TextMeshProUGUI textContourMinArea;
    [SerializeField] private TextMeshProUGUI textPointRadius;

    [Header("Mesh")]
    [SerializeField] private Material meshMaterial;
    [SerializeField] private Color shadowSettingsColor;
    [SerializeField] private Color shadowViewColor;
    [SerializeField] private Color shadowHideColor;

    private bool isSettingMode = false;
    public bool IsSettingMode => isSettingMode;
    private bool isPointActive = false;
    private bool hasInitDone = false;

    private int screenWidthRatio;
    private int screenHeightRatio;

    private void Awake()
    {
        detector = FindObjectOfType<CameraBasedShadowDetector>();
        detector.onInitDone.AddListener(Init);
        uiManager = FindObjectOfType<UIManager>();
        uiManager.onCancelPressed.AddListener(Cancel);
        uiManager.onAcceptPressed.AddListener(Accept);
        uiManager.onUIOpened.AddListener(OpenUI);
        uiManager.onUIClosed.AddListener(CloseUI);
    }

    public void Init()
    {
        screenWidthRatio = Screen.width / detector.width;
        screenHeightRatio = Screen.height / detector.height;

        Load();
        settings.CopyTo(settingsTemp);
        SetMeshColor();

        UpdateUI();

        hasInitDone = true;
    }

    private void UpdateUI()
    {
        sliderR.value = settings.r;
        sliderG.value = settings.g;
        sliderB.value = settings.b;
        sliderThreshold.value = settings.threshold;
        sliderEpsilon.value = (float)settings.epsilon;
        sliderGaussian.value = settings.gaussian;
        sliderContourMinArea.value = settings.contourMinArea;
        toggleUseApprox.isOn = settings.useApprox;
        toggleViewShadow.isOn = settings.viewShadow;
        sliderPointRadius.value = settings.pointRadius;
        textR.text = settings.r.ToString();
        textG.text = settings.g.ToString();
        textB.text = settings.b.ToString();
        textThreshold.text = settings.threshold.ToString();
        textEpsilon.text = settings.epsilon.ToString("0.000");
        textGaussian.text = settings.gaussian.ToString();
        textContourMinArea.text = settings.contourMinArea.ToString();
        textPointRadius.text = settings.pointRadius.ToString();
    }

    private void Update()
    {
        if (!hasInitDone)
            return;

        // 그림자 메쉬 확인
        if (!uiManager.IsUIOpen && !isSettingMode)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                SetViewShadow(!settings.viewShadow);
                SetMeshColor();
            }
        }

        if (uiManager.IsUIOpen && isSettingMode)
        {
            if (rawImage.texture == null)
                rawImage.texture = detector.GetFrameTexture();

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

    public void SetSettingMode(bool value)
    {
        isSettingMode = value;
        rawImage.gameObject.SetActive(value);
        cameraPointSettingCount = 0;

        if (!value)
        {
            toggleScreen.isOn = true;
            DeactivatePoint();
        }
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

    public int GetR() { return settings.r; }
    public void SetR(float value) { settings.r = (int)value; UpdateUI(); }
    public int GetG() { return settings.g; }
    public void SetG(float value) { settings.g = (int)value; UpdateUI(); }
    public int GetB() { return settings.b; }
    public void SetB(float value) { settings.b = (int)value; UpdateUI(); }
    public int GetThreshold() { return settings.threshold; }
    public void SetThreshold(float value) { settings.threshold = (int)value; UpdateUI(); }
    public double GetEpsilon() { return settings.epsilon; }
    public void SetEpsilon(float value) { settings.epsilon = value; UpdateUI(); }
    public int GetGaussian() { return settings.gaussian; }
    public void SetGaussian(float value) { settings.gaussian = (int)value; UpdateUI(); }
    public int GetContourMinArea() { return settings.contourMinArea; }
    public void SetContourMinArea(float value) { settings.contourMinArea = (int)value; UpdateUI(); }
    public bool GetUseApprox() { return settings.useApprox; }
    public void SetUseApprox(bool value) { settings.useApprox = value; UpdateUI(); }
    public bool GetViewShadow() { return settings.viewShadow; }
    public void SetViewShadow(bool value) { settings.viewShadow = value; UpdateUI(); }
    public int GetPointRadius() { return settings.pointRadius; }
    public void SetPointRadius(float value) { settings.pointRadius = (int)value; UpdateUI(); }

    private void OpenUI()
    {
        settings.CopyTo(settingsTemp);
        meshMaterial.color = shadowSettingsColor;
    }

    private void CloseUI()
    {
        SetMeshColor();
    }

    private void SetMeshColor()
    {
        if (settings.viewShadow)
            meshMaterial.color = shadowViewColor;
        else
            meshMaterial.color = shadowHideColor;
    }

    public void Cancel()
    {
        settingsTemp.CopyTo(settings);
        UpdateUI();
    }

    public void Accept()
    {
        Save();
    }

    private void Save()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetInt($"{currentSceneName}_r", settings.r);
        PlayerPrefs.SetInt($"{currentSceneName}_g", settings.g);
        PlayerPrefs.SetInt($"{currentSceneName}_b", settings.b);
        PlayerPrefs.SetInt($"{currentSceneName}_threshold", settings.threshold);
        PlayerPrefs.SetFloat("epsilon", (float)settings.epsilon);
        PlayerPrefs.SetInt("gaussian", settings.gaussian);
        PlayerPrefs.SetInt($"{currentSceneName}_contourMinArea", settings.contourMinArea);
        PlayerPrefs.SetInt("useApprox", System.Convert.ToInt16(settings.useApprox));
        PlayerPrefs.SetInt("viewShadow", System.Convert.ToInt16(settings.viewShadow));
        PlayerPrefs.SetInt("pointRadius", settings.pointRadius);
        PlayerPrefs.SetFloat("cameraBasedPoint1X", (float)cameraBasedPoints[0].x);
        PlayerPrefs.SetFloat("cameraBasedPoint1Y", (float)cameraBasedPoints[0].y);
        PlayerPrefs.SetFloat("cameraBasedPoint2X", (float)cameraBasedPoints[1].x);
        PlayerPrefs.SetFloat("cameraBasedPoint2Y", (float)cameraBasedPoints[1].y);
        PlayerPrefs.SetFloat("cameraBasedPoint3X", (float)cameraBasedPoints[2].x);
        PlayerPrefs.SetFloat("cameraBasedPoint3Y", (float)cameraBasedPoints[2].y);
        PlayerPrefs.SetFloat("cameraBasedPoint4X", (float)cameraBasedPoints[3].x);
        PlayerPrefs.SetFloat("cameraBasedPoint4Y", (float)cameraBasedPoints[3].y);
    }

    private void Load()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        settings.r = PlayerPrefs.GetInt($"{currentSceneName}_r");
        settings.g = PlayerPrefs.GetInt($"{currentSceneName}_g");
        settings.b = PlayerPrefs.GetInt($"{currentSceneName}_b");
        settings.threshold = PlayerPrefs.GetInt($"{currentSceneName}_threshold");
        settings.epsilon = PlayerPrefs.GetFloat("epsilon");
        settings.gaussian = PlayerPrefs.GetInt("gaussian");
        settings.contourMinArea = PlayerPrefs.GetInt($"{currentSceneName}_contourMinArea");
        settings.useApprox = System.Convert.ToBoolean(PlayerPrefs.GetInt("useApprox"));
        settings.viewShadow = System.Convert.ToBoolean(PlayerPrefs.GetInt("viewShadow"));
        settings.pointRadius = PlayerPrefs.GetInt("pointRadius");
        cameraBasedPoints[0].x = PlayerPrefs.GetFloat("cameraBasedPoint1X");
        cameraBasedPoints[0].y = PlayerPrefs.GetFloat("cameraBasedPoint1Y");
        cameraBasedPoints[1].x = PlayerPrefs.GetFloat("cameraBasedPoint2X");
        cameraBasedPoints[1].y = PlayerPrefs.GetFloat("cameraBasedPoint2Y");
        cameraBasedPoints[2].x = PlayerPrefs.GetFloat("cameraBasedPoint3X");
        cameraBasedPoints[2].y = PlayerPrefs.GetFloat("cameraBasedPoint3Y");
        cameraBasedPoints[3].x = PlayerPrefs.GetFloat("cameraBasedPoint4X");
        cameraBasedPoints[3].y = PlayerPrefs.GetFloat("cameraBasedPoint4Y");
    }
}

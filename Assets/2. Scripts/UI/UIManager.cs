using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject uiObject;

    public UnityEvent onCancelPressed = new UnityEvent();
    public UnityEvent onAcceptPressed = new UnityEvent();
    public UnityEvent onUIOpened = new UnityEvent();
    public UnityEvent onUIClosed = new UnityEvent();

    private CameraBasedShadowDetectorSetting settings;

    private bool isUIOpen = false;
    public bool IsUIOpen => isUIOpen;

    private void Awake()
    {
        settings = FindObjectOfType<CameraBasedShadowDetectorSetting>();
    }

    private void Start()
    {
        CloseUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isUIOpen)
                OpenUI();
            else
            {
                if (settings.IsSettingMode)
                    settings.SetSettingMode(false);
                else
                    Accept();
            }
        }
    }

    public void Cancel()
    {
        CloseUI();
        onCancelPressed.Invoke();
    }

    public void Accept()
    {
        CloseUI();
        onAcceptPressed.Invoke();
    }

    public void OpenUI()
    {
        Time.timeScale = 0;
        isUIOpen = true;
        uiObject.SetActive(true);
        onUIOpened.Invoke();
    }

    public void CloseUI()
    {
        Time.timeScale = 1;
        isUIOpen = false;
        uiObject.SetActive(false);
        onUIClosed.Invoke();
    }
}

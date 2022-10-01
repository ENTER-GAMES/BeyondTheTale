using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using System;
using System.Collections;
using UnityEngine;

public class WebCamTextureToMat : MonoBehaviour
{
    [SerializeField] public string requestedDeviceName = null;
    [SerializeField] public int requestedWidth = 640;
    [SerializeField] public int requestedHeight = 360;
    [SerializeField] public int requestedFPS = 30;
    [SerializeField] public bool requestedIsFrontFacing = false;

    [SerializeField] private CameraBasedShadowDetector detector;

    WebCamTexture webCamTexture;
    WebCamDevice webCamDevice;
    Mat rgbaMat;
    Color32[] colors;
    bool isInitWaiting = false;
    bool hasInitDone = false;
    bool didUpdateFirstFrame = false;
    bool didUpdateSecondFrame = false;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (isInitWaiting)
            return;

        StartCoroutine(_Initialize());
    }


    private IEnumerator _Initialize()
    {
        if (hasInitDone)
            Dispose();

        isInitWaiting = true;

        var devices = WebCamTexture.devices;
        if (!String.IsNullOrEmpty(requestedDeviceName))
        {
            int requestedDeviceIndex = -1;
            if (Int32.TryParse(requestedDeviceName, out requestedDeviceIndex))
            {
                if (requestedDeviceIndex >= 0 && requestedDeviceIndex < devices.Length)
                {
                    webCamDevice = devices[requestedDeviceIndex];
                    webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
                }
            }
            else
            {
                for (int cameraIndex = 0; cameraIndex < devices.Length; cameraIndex++)
                {
                    if (devices[cameraIndex].name == requestedDeviceName)
                    {
                        webCamDevice = devices[cameraIndex];
                        webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
                        break;
                    }
                }
            }
            if (webCamTexture == null)
                Debug.Log("Cannot find camera device " + requestedDeviceName + ".");
        }

        if (webCamTexture == null)
        {
            for (int cameraIndex = 0; cameraIndex < devices.Length; cameraIndex++)
            {
                if (devices[cameraIndex].kind != WebCamKind.ColorAndDepth && devices[cameraIndex].isFrontFacing == requestedIsFrontFacing)
                {
                    webCamDevice = devices[cameraIndex];
                    webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
                    break;
                }
            }
        }

        if (webCamTexture == null)
        {
            if (devices.Length > 0)
            {
                webCamDevice = devices[0];
                webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
            }
            else
            {
                Debug.LogError("Camera device does not exist.");
                isInitWaiting = false;
                yield break;
            }
        }

        webCamTexture.Play();

        while (true)
        {
            if (webCamTexture.didUpdateThisFrame)
            {
                Debug.Log("name:" + webCamTexture.deviceName + " width:" + webCamTexture.width + " height:" + webCamTexture.height + " fps:" + webCamTexture.requestedFPS);
                Debug.Log("videoRotationAngle:" + webCamTexture.videoRotationAngle + " videoVerticallyMirrored:" + webCamTexture.videoVerticallyMirrored + " isFrongFacing:" + webCamDevice.isFrontFacing);

                isInitWaiting = false;
                hasInitDone = true;

                OnInited();

                break;
            }
            else
            {
                yield return null;
            }
        }
    }

    public void Dispose()
    {
        isInitWaiting = false;
        hasInitDone = false;

        if (webCamTexture != null)
        {
            webCamTexture.Stop();
            WebCamTexture.Destroy(webCamTexture);
            webCamTexture = null;
        }
        if (rgbaMat != null)
        {
            rgbaMat.Dispose();
            rgbaMat = null;
        }
    }

    private void OnInited()
    {
        if (colors == null || colors.Length != webCamTexture.width * webCamTexture.height)
            colors = new Color32[webCamTexture.width * webCamTexture.height];

        rgbaMat = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));

        int width = rgbaMat.width();
        int height = rgbaMat.height();
        detector.Initialize(width, height);
        detector.onDispose.AddListener(Dispose);
    }

    void Update()
    {
        if (hasInitDone && webCamTexture.isPlaying && webCamTexture.didUpdateThisFrame)
        {
            Utils.webCamTextureToMat(webCamTexture, rgbaMat, colors);
            detector.Run(rgbaMat);

            if (!didUpdateSecondFrame && didUpdateFirstFrame)
            {
                didUpdateSecondFrame = true;
                detector.onSecondFrameUpdate.Invoke();
            }

            if (!didUpdateFirstFrame)
            {
                didUpdateFirstFrame = true;
                detector.onFirstFrameUpdate.Invoke();
            }
        }
    }

    void OnDestroy()
    {
        Dispose();
    }
}
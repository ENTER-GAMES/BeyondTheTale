using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.UnityUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class CameraBasedPoint
{
    [Range(0, 640)]
    public double x;
    [Range(0, 360)]
    public double y;

    public Point Get()
    {
        return new Point(x, y);
    }
}

public class CameraBasedShadowDetector : ShadowDetector
{
    [SerializeField]
    public string requestedDeviceName = null;
    [SerializeField]
    private int requestedWidth;
    [SerializeField]
    private int requestedHeight;
    [SerializeField]
    private int requestedFPS;
    [SerializeField]
    private new Renderer renderer;
    [SerializeField]
    private Renderer rendererR;
    [SerializeField]
    private Renderer rendererG;
    [SerializeField]
    private Renderer rendererB;
    [SerializeField]
    private Renderer rendererAdd;
    [SerializeField]
    private Renderer rendererGray;
    [SerializeField]
    private Renderer rendererThr;
    [SerializeField]
    [Range(-255, 255)]
    private int r = 0;
    [SerializeField]
    [Range(-255, 255)]
    private int g = 0;
    [SerializeField]
    [Range(-255, 255)]
    private int b = 0;
    [SerializeField]
    [Range(0, 255)]
    private int threshold = 20;
    [SerializeField]
    [Range(0.001f, 0.01f)]
    private double epsilon = 0.001f;
    [SerializeField]
    private bool useApprox = true;
    [SerializeField]
    private bool drawMesh = false;
    [SerializeField]
    private bool drawPoint = false;
    [SerializeField]
    private bool view = true;
    [SerializeField]
    private CameraBasedPoint[] cameraBasedPoints = new CameraBasedPoint[4];

    private Texture2D textureSrc;
    private Texture2D textureR;
    private Texture2D textureG;
    private Texture2D textureB;
    private Texture2D textureFrame;
    private Texture2D textureGray;
    private Texture2D textureThr;
    private WebCamTexture webCamTexture;
    private WebCamDevice webCamDevice;
    private Color32[] colors;
    private Mat frame;
    private int width;
    private int height;
    private List<MatOfPoint> contours;
    private Mat result;

    private bool hasInitDone = false;

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        var devices = WebCamTexture.devices;
        if (!string.IsNullOrEmpty(requestedDeviceName))
        {
            int requestedDeviceIndex = -1;
            if (int.TryParse(requestedDeviceName, out requestedDeviceIndex))
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
                if (devices[cameraIndex].kind != WebCamKind.ColorAndDepth)
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

                OnInited();

                yield return null;

                hasInitDone = true;

                break;
            }
            else
            {
                yield return null;
            }
        }
    }

    private void OnInited()
    {
        width = webCamTexture.width;
        height = webCamTexture.height;

        colors = new Color32[width * height];
        textureSrc = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureR = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureG = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureB = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureFrame = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureGray = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureThr = new Texture2D(width, height, TextureFormat.RGBA32, false);
        frame = new Mat(height, width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
        result = new Mat(height, width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));

        renderer.material.mainTexture = textureSrc;
        rendererR.material.mainTexture = textureR;
        rendererG.material.mainTexture = textureG;
        rendererB.material.mainTexture = textureB;
        rendererAdd.material.mainTexture = textureFrame;
        rendererGray.material.mainTexture = textureGray;
        rendererThr.material.mainTexture = textureThr;
    }

    private void Update()
    {
        if (hasInitDone && webCamTexture.isPlaying && webCamTexture.didUpdateThisFrame)
        {
            Utils.webCamTextureToMat(webCamTexture, frame, colors);
            Run();
        }
    }

    private void Run()
    {
        Mat src = PerspectiveTransform();

        Mat rgb = new Mat(height, width, CvType.CV_8UC4, new Scalar(r, g, b, 0));
        Mat add = CVUtils.Add(src, rgb);
        rgb = new Mat(height, width, CvType.CV_8UC4, new Scalar(r * -1, g * -1, b * -1, 0));
        add = CVUtils.Subtract(add, rgb);
        List<Mat> split = new List<Mat>();
        Core.split(add, split);
        Mat gray = CVUtils.CvtColor(add, Imgproc.COLOR_BGR2GRAY);
        Mat blur = CVUtils.GaussianBlur(gray, new Size(5, 5), 0);
        result = CVUtils.Threshold(blur, threshold, 255);

        DrawMesh(result);
        DrawPerspectivePoint();
        ShowDisplay(src, split[0], split[1], split[2], frame, gray, result);
    }

    private Mat PerspectiveTransform()
    {
        Mat pts1 = new Mat(4, 1, CvType.CV_32FC2);
        Mat pts2 = new Mat(4, 1, CvType.CV_32FC2);
        pts1.put(0, 0, cameraBasedPoints[0].Get().x, cameraBasedPoints[0].Get().y,
                       cameraBasedPoints[1].Get().x, cameraBasedPoints[1].Get().y,
                       cameraBasedPoints[2].Get().x, cameraBasedPoints[2].Get().y,
                       cameraBasedPoints[3].Get().x, cameraBasedPoints[3].Get().y);
        pts2.put(0, 0, 0.0, 0.0, width, 0.0, 0.0, height, width, height);

        Mat mtrx = CVUtils.GetPerspectiveTransform(pts1, pts2);
        return CVUtils.WarpPerspective(frame, mtrx, new Size(width, height));
    }

    private void DrawPerspectivePoint()
    {
        if (drawPoint)
        {
            for (int i = 0; i < cameraBasedPoints.Length; i++)
                CVUtils.DrawCircle(ref frame, cameraBasedPoints[i].Get());
        }
    }

    private void DrawMesh(Mat src)
    {
        contours = CVUtils.FindContours(src);
        List<Shadow> shadows = FindShadow(contours);

        if (drawMesh)
        {
            MeshDrawer.Clear();
            MeshDrawer.Draw(shadows);
        }
    }

    private List<Shadow> FindShadow(List<MatOfPoint> contours)
    {
        List<Shadow> shadows = new List<Shadow>();
        foreach (MatOfPoint c in contours)
        {
            double area = Imgproc.contourArea(c);
            if (area > 1000)
            {
                Point[] points = c.toArray();
                if (useApprox)
                    points = Approx(points);

                SetOffset(ref points);
                Shadow shadow = new Shadow(CVUtils.PointToVector2(points));
                shadows.Add(shadow);
            }
        }

        return shadows;
    }

    private Point[] Approx(Point[] points)
    {
        MatOfPoint2f curve = new MatOfPoint2f(points);
        MatOfPoint2f approx = new MatOfPoint2f();
        double p = Imgproc.arcLength(curve, true);
        Imgproc.approxPolyDP(curve, approx, epsilon * p, true);

        return approx.toArray();
    }

    private void ShowDisplay(Mat src, Mat r, Mat g, Mat b, Mat frame, Mat gray, Mat result)
    {
        if (view)
        {
            DrawPerspectivePoint();

            Utils.matToTexture2D(src, textureSrc, colors);
            Utils.matToTexture2D(r, textureR, colors);
            Utils.matToTexture2D(g, textureG, colors);
            Utils.matToTexture2D(b, textureB, colors);
            Utils.matToTexture2D(frame, textureFrame, colors);
            Utils.matToTexture2D(gray, textureGray, colors);
            Utils.matToTexture2D(result, textureThr, colors);
        }
    }

    private void SetOffset(ref Point[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            float x = (float)points[i].x * (Screen.width / width);
            float y = Screen.height - (float)points[i].y * (Screen.height / height);
            Vector3 point = new Vector3(x, y, 0);
            Vector3 pos = Camera.main.ScreenToWorldPoint(point);
            points[i].x = pos.x;
            points[i].y = pos.y;
        }
    }

    public Mat GetResult()
    {
        return result;
    }
}
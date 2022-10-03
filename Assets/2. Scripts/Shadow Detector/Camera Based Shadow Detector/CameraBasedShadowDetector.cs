using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    [Header("Renderers")]
    [SerializeField] private Renderer rendererFrame;
    [SerializeField] private Renderer rendererPOV;
    [SerializeField] private Renderer rendererR;
    [SerializeField] private Renderer rendererG;
    [SerializeField] private Renderer rendererB;
    [SerializeField] private Renderer rendererRGB;
    [SerializeField] private Renderer rendererGray;
    [SerializeField] private Renderer rendererBlur;
    [SerializeField] private Renderer rendererThreshold;
    [SerializeField] private Renderer rendererContour;
    [SerializeField] private Renderer rendererArea;
    [SerializeField] private Renderer rendererApprox;

    [Header("Draw")]
    [SerializeField] private bool drawMesh = false;
    [SerializeField] private bool drawPoint = false;

    [Header("View")]
    [SerializeField] private bool viewFrame = false;
    [SerializeField] private bool viewPOV = false;
    [SerializeField] private bool viewR = false;
    [SerializeField] private bool viewG = false;
    [SerializeField] private bool viewB = false;
    [SerializeField] private bool viewRGB = false;
    [SerializeField] private bool viewGray = false;
    [SerializeField] private bool viewBlur = false;
    [SerializeField] private bool viewThreshold = false;
    [SerializeField] private bool viewContour = false;
    [SerializeField] private bool viewArea = false;
    [SerializeField] private bool viewApprox = false;

    private CameraBasedShadowDetectorSetting settings;

    private Texture2D textureFrame;
    private Texture2D texturePOV;
    private Texture2D textureR;
    private Texture2D textureG;
    private Texture2D textureB;
    private Texture2D textureRGB;
    private Texture2D textureGray;
    private Texture2D textureBlur;
    private Texture2D textureThr;
    private Texture2D textureContour;
    private Texture2D textureArea;
    private Texture2D textureApprox;

    private Color32[] colors;
    private List<MatOfPoint> contourPoints;
    private List<MatOfPoint> areaPoints;
    private List<MatOfPoint> approxPoints;
    private Mat frame;
    private Mat pov;
    private Mat r;
    private Mat g;
    private Mat b;
    private Mat rgb;
    private Mat gray;
    private Mat blur;
    private Mat thr;
    private Mat contour;
    private Mat area;
    private Mat approx;

    public int width { get; private set; }
    public int height { get; private set; }

    private bool hasInitDone = false;
    public bool HasInitDone => hasInitDone;

    [Header("Events")]
    public UnityEvent onInitDone = new UnityEvent();
    public UnityEvent onFirstFrameUpdate = new UnityEvent();
    public UnityEvent onSecondFrameUpdate = new UnityEvent();
    public UnityEvent onDispose = new UnityEvent();

    private void Awake()
    {
        settings = FindObjectOfType<CameraBasedShadowDetectorSetting>();
    }

    public void Initialize(int width, int height)
    {
        this.width = width;
        this.height = height;
        Debug.Log($"width : {width}, height : {height}, ScreenWidth : {Screen.width}, ScreenHeight : {Screen.height}");

        colors = new Color32[width * height];
        textureFrame = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texturePOV = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureR = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureG = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureB = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureRGB = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureGray = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureBlur = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureThr = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureContour = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureArea = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureApprox = new Texture2D(width, height, TextureFormat.RGBA32, false);
        frame = new Mat(height, width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
        thr = new Mat(height, width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
        areaPoints = new List<MatOfPoint>();
        approxPoints = new List<MatOfPoint>();

        rendererFrame.material.mainTexture = textureFrame;
        rendererPOV.material.mainTexture = texturePOV;
        rendererR.material.mainTexture = textureR;
        rendererG.material.mainTexture = textureG;
        rendererB.material.mainTexture = textureB;
        rendererRGB.material.mainTexture = textureRGB;
        rendererGray.material.mainTexture = textureGray;
        rendererBlur.material.mainTexture = textureBlur;
        rendererThreshold.material.mainTexture = textureThr;
        rendererContour.material.mainTexture = textureContour;
        rendererArea.material.mainTexture = textureArea;
        rendererApprox.material.mainTexture = textureApprox;

        hasInitDone = true;
        onInitDone.Invoke();
    }

    public void Run(Mat frame)
    {
        this.frame = frame;

        pov = PerspectiveTransform();

        Mat rgbTemp = new Mat(height, width, CvType.CV_8UC4, new Scalar(settings.GetR(), settings.GetG(), settings.GetB(), 0));
        rgb = CVUtils.Add(pov, rgbTemp);
        rgbTemp = new Mat(height, width, CvType.CV_8UC4, new Scalar(settings.GetR() * -1, settings.GetG() * -1, settings.GetB() * -1, 0));
        rgb = CVUtils.Subtract(rgb, rgbTemp);
        List<Mat> split = new List<Mat>();
        Core.split(rgb, split);
        r = split[0];
        g = split[1];
        b = split[2];
        gray = CVUtils.CvtColor(rgb, Imgproc.COLOR_BGR2GRAY);
        if (settings.GetGaussian() % 2 == 0)
            settings.SetGaussian(Mathf.Clamp(settings.GetGaussian() - 1, 1, 11));
        blur = CVUtils.GaussianBlur(gray, new Size(settings.GetGaussian(), settings.GetGaussian()), 0);
        thr = CVUtils.Threshold(blur, settings.GetThreshold(), 255);

        DrawMesh(thr);
        DrawPerspectivePoint();
        ShowDisplay();
    }

    private Mat PerspectiveTransform()
    {
        Mat pts1 = new Mat(4, 1, CvType.CV_32FC2);
        Mat pts2 = new Mat(4, 1, CvType.CV_32FC2);
        pts1.put(0, 0, settings.GetCameraBasedPoint(0).x, settings.GetCameraBasedPoint(0).y,
                       settings.GetCameraBasedPoint(1).x, settings.GetCameraBasedPoint(1).y,
                       settings.GetCameraBasedPoint(2).x, settings.GetCameraBasedPoint(2).y,
                       settings.GetCameraBasedPoint(3).x, settings.GetCameraBasedPoint(3).y);
        pts2.put(0, 0, 0.0, 0.0, width, 0.0, 0.0, height, width, height);

        Mat mtrx = CVUtils.GetPerspectiveTransform(pts1, pts2);
        return CVUtils.WarpPerspective(frame, mtrx, new Size(width, height));
    }

    public void DrawPerspectivePoint()
    {
        if (drawPoint)
        {
            Scalar red = new Scalar(255, 0, 0, 255);
            Scalar yellow = new Scalar(255, 255, 0, 255);
            for (int i = 0; i < settings.GetCameraBasedPointLength(); i++)
            {
                if (settings.IsSettingMode && i == settings.CameraPointSettingCount)
                    CVUtils.DrawCircle(ref frame, settings.GetCameraBasedPoint(i), settings.GetPointRadius(), red);
                else
                    CVUtils.DrawCircle(ref frame, settings.GetCameraBasedPoint(i), settings.GetPointRadius(), yellow);
            }
        }
    }

    private void DrawMesh(Mat src)
    {
        contourPoints = CVUtils.FindContours(src);
        List<Shadow> shadows = FindShadow(contourPoints);

        if (drawMesh)
        {
            MeshDrawer.Clear();
            MeshDrawer.Draw(shadows);
        }
    }

    private List<Shadow> FindShadow(List<MatOfPoint> contours)
    {
        List<Shadow> shadows = new List<Shadow>();
        areaPoints.Clear();
        approxPoints.Clear();
        foreach (MatOfPoint c in contours)
        {
            double area = Imgproc.contourArea(c);
            if (area > settings.GetContourMinArea())
            {
                areaPoints.Add(c);
                Point[] points = c.toArray();
                if (settings.GetUseApprox())
                {
                    points = Approx(points);
                    approxPoints.Add(new MatOfPoint(points));
                }

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
        Imgproc.approxPolyDP(curve, approx, settings.GetEpsilon() * p, true);

        return approx.toArray();
    }

    private void ShowDisplay()
    {
        if (viewFrame)
            Utils.matToTexture2D(frame, textureFrame, colors);

        if (viewPOV)
            Utils.matToTexture2D(pov, texturePOV, colors);

        if (viewR)
            Utils.matToTexture2D(r, textureR, colors);

        if (viewG)
            Utils.matToTexture2D(g, textureG, colors);

        if (viewB)
            Utils.matToTexture2D(b, textureB, colors);

        if (viewRGB)
            Utils.matToTexture2D(rgb, textureRGB, colors);

        if (viewGray)
            Utils.matToTexture2D(gray, textureGray, colors);

        if (viewBlur)
            Utils.matToTexture2D(blur, textureBlur, colors);

        if (viewThreshold)
            Utils.matToTexture2D(thr, textureThr, colors);

        if (viewContour)
        {
            contour = new Mat(height, width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
            Imgproc.fillPoly(contour, contourPoints, new Scalar(255, 255, 255, 255));
            Imgproc.drawContours(contour, contourPoints, -1, new Scalar(255, 0, 0, 255), 0);
            Utils.matToTexture2D(contour, textureContour, colors);
        }

        if (viewArea)
        {
            area = new Mat(height, width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
            Imgproc.fillPoly(area, areaPoints, new Scalar(255, 255, 255, 255));
            Imgproc.drawContours(area, areaPoints, -1, new Scalar(0, 0, 255, 255), 2);
            Utils.matToTexture2D(area, textureArea, colors);
        }

        if (viewApprox)
        {
            approx = new Mat(height, width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
            Imgproc.fillPoly(approx, approxPoints, new Scalar(255, 255, 255, 255));
            Imgproc.drawContours(approx, approxPoints, -1, new Scalar(0, 255, 0, 255), 2);
            Utils.matToTexture2D(approx, textureApprox, colors);
        }

        rendererFrame.gameObject.SetActive(viewFrame);
        rendererPOV.gameObject.SetActive(viewPOV);
        rendererR.gameObject.SetActive(viewR);
        rendererG.gameObject.SetActive(viewG);
        rendererB.gameObject.SetActive(viewB);
        rendererRGB.gameObject.SetActive(viewRGB);
        rendererGray.gameObject.SetActive(viewGray);
        rendererBlur.gameObject.SetActive(viewBlur);
        rendererThreshold.gameObject.SetActive(viewThreshold);
        rendererContour.gameObject.SetActive(viewContour);
        rendererArea.gameObject.SetActive(viewArea);
        rendererApprox.gameObject.SetActive(viewApprox);
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
        return thr;
    }

    public Texture GetPOVTexture()
    {
        Utils.matToTexture2D(pov, texturePOV, colors);
        return texturePOV;
    }

    public Texture GetFrameTexture()
    {
        Utils.matToTexture2D(frame, textureFrame, colors);
        return textureFrame;
    }

    public void Dispose()
    {
        onDispose.Invoke();
    }
}
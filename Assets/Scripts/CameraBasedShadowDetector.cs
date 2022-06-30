using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.UnityUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBasedShadowDetector : ShadowDetector
{
    [SerializeField]
    private int requestedWidth;
    [SerializeField]
    private int requestedHeight;
    [SerializeField]
    private int requestedFPS;
    [SerializeField]
    private new Renderer renderer;
    [SerializeField]
    [Range(0, 255)]
    private int threshold = 20;
    [SerializeField]
    [Range(0.001f, 0.01f)]
    private double epsilon = 0.001f;

    private Texture2D texture;
    private WebCamTexture webCamTexture;
    private WebCamDevice webCamDevice;
    private Color32[] colors;
    private Mat origin;
    private Mat frame;
    private Mat result = new Mat();
    private int width;
    private int height;
    private bool isCaptureOrigin = false;


    private void Start()
    {
        Init();
    }

    private void Init()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        webCamDevice = devices[0];
        webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
        webCamTexture.Play();

        width = webCamTexture.width;
        height = webCamTexture.height;

        OnInit();
    }

    private void OnInit()
    {
        colors = new Color32[width * height];
        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        origin = new Mat(height, width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
        frame = new Mat(height, width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
        
        renderer.material.mainTexture = texture;
    }

    private void Update()
    {
        if (!isCaptureOrigin)
        {
            Utils.webCamTextureToMat(webCamTexture, origin, colors);
            Utils.matToTexture2D(origin, texture, colors);
        }
        else
        {
            Utils.webCamTextureToMat(webCamTexture, frame, colors);
            Run();
            Utils.matToTexture2D(frame, texture, colors);
        }
    }

    public void CaptureOrigin()
    {
        isCaptureOrigin = true;
        Utils.webCamTextureToMat(webCamTexture, origin, colors, true);
    }

    private void Run()
    {
        Mat grayA = MyUtils.ConvertToGrayscale(origin);
        Mat grayB = MyUtils.ConvertToGrayscale(frame);
        Mat diff = MyUtils.AbsDiff(grayA, grayB);
        Mat blur = MyUtils.GaussianBlur(diff, new Size(9, 9), 0);
        result = MyUtils.Threshold(diff, threshold, 255);
        Mat hierarchy = new Mat();
        List<MatOfPoint> contours = new List<MatOfPoint>();
        MyUtils.FindContours(result, ref contours, ref hierarchy);

        List<Shadow> shadows = new List<Shadow>();
        foreach (MatOfPoint c in contours)
        {
            Point center = MyUtils.FindCenter(c);
            double area = Imgproc.contourArea(c);
            if (area > 100)
            {
                //DrawCircle(center);
                MatOfPoint2f curve = new MatOfPoint2f(c.toArray());
                MatOfPoint2f approx = new MatOfPoint2f();
                double p = Imgproc.arcLength(curve, true);
                Imgproc.approxPolyDP(curve, approx, epsilon * p, true);

                Point[] points = approx.toArray();
                SetOffset(ref points);
                Shadow shadow = new Shadow(MyUtils.PointToVector2(points));
                shadows.Add(shadow);
            }
        }
        MeshDrawer.Clear();
        MeshDrawer.Draw(shadows);
    }

    private void SetOffset(ref Point[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].x -= width / 2;
            points[i].y *= -1;
            points[i].y += height / 2;
        }
    }
}
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

    private Texture2D texture;
    private WebCamTexture webCamTexture;
    private WebCamDevice webCamDevice;
    private Color32[] colors;
    private Mat rgbaMat;
    private Mat gray = new Mat();
    private Mat thr = new Mat();


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

        OnInit();
    }

    private void OnInit()
    {
        colors = new Color32[webCamTexture.width * webCamTexture.height];
        texture = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);
        rgbaMat = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
        gameObject.GetComponent<Renderer>().material.mainTexture = texture;
    }

    private void Update()
    {
        Utils.webCamTextureToMat(webCamTexture, rgbaMat, colors);

        Run();

        Utils.matToTexture2D(rgbaMat, texture, colors);
    }

    private void Run()
    {
        ConvertToGrayscale();
        Threshold();
        Mat hierarchy = new Mat();
        List<MatOfPoint> contours = new List<MatOfPoint>();
        FindContours(ref contours, ref hierarchy);

        MeshDrawer.Clear();

        for (int i = 0; i < contours.Count; ++i)
        {
            Point center = FindCenter(contours[i]);

            DrawContours(contours, i);
            //DrawCircle(center);

            List<Point> pts = MergeList(contours[i], center);
            SetOffset(ref pts);

            MeshDrawer.Draw(pts);
        }
    }

    private void ConvertToGrayscale()
    {
        Imgproc.cvtColor(rgbaMat, gray, Imgproc.COLOR_BGR2GRAY);
    }

    private void Threshold()
    {
        Imgproc.threshold(gray, thr, 127, 255, Imgproc.THRESH_BINARY);
    }

    private void FindContours(ref List<MatOfPoint> contours, ref Mat hierarchy)
    {
        Imgproc.findContours(thr, contours, hierarchy, Imgproc.RETR_LIST, Imgproc.CHAIN_APPROX_SIMPLE);
    }

    private Point FindCenter(MatOfPoint contour)
    {
        Moments m = Imgproc.moments(contour);
        return new Point((int)(m.m10 / m.m00), (int)(m.m01 / m.m00));
    }

    private void DrawContours(List<MatOfPoint> contours, int i)
    {
        Imgproc.drawContours(rgbaMat, contours, i, new Scalar(255, 0, 0), 3);
    }

    private void DrawCircle(Point point)
    {
        Imgproc.circle(rgbaMat, point, 7, new Scalar(255, 0, 0), -1);
    }

    private List<Point> MergeList(MatOfPoint contour, Point point)
    {
        List<Point> pts = contour.toList();
        pts.Insert(0, point);

        return pts;
    }

    private void SetOffset(ref List<Point> pts)
    {
        for (int j = 0; j < pts.Count; j++)
        {
            pts[j].x -= requestedWidth / 2;
            pts[j].y *= -1;
            pts[j].y += requestedHeight / 2;
        }
    }
}
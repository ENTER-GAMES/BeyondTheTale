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
        Mat grayA = ConvertToGrayscale(origin);
        Mat grayB = ConvertToGrayscale(frame);
        Mat diff = AbsDiff(grayA, grayB);
        //Mat blur = GaussianBlur(diff, new Size(9, 9), 0);
        result = Threshold(diff, threshold, 255);
        Mat hierarchy = new Mat();
        List<MatOfPoint> contours = new List<MatOfPoint>();
        FindContours(result, ref contours, ref hierarchy);

        MeshDrawer.Clear();

        foreach (MatOfPoint c in contours)
        {
            Point center = FindCenter(c);
            double area = Imgproc.contourArea(c);
            if (area > 100)
            {
                //DrawCircle(center);
                MatOfPoint2f curve = new MatOfPoint2f(c.toArray());
                MatOfPoint2f approx = new MatOfPoint2f();
                double p = Imgproc.arcLength(curve, true);
                Imgproc.approxPolyDP(curve, approx, epsilon * p, true);

                List<Point> points = MergeList(new MatOfPoint(approx.toArray()), center);
                SetOffset(ref points);
                Shadow shadow = new Shadow(PointToVector3(points));
                MeshDrawer.Draw(shadow);
            }
        }
    }

    private Mat ConvertToGrayscale(Mat src)
    {
        Mat result = new Mat();
        Imgproc.cvtColor(src, result, Imgproc.COLOR_BGR2GRAY);
        return result;
    }

    private Mat Threshold(Mat src, int threshold, int max)
    {
        Mat result = new Mat();
        Imgproc.threshold(src, result, threshold, max, Imgproc.THRESH_BINARY);
        return result;
    }

    private Mat AbsDiff(Mat srcA, Mat srcB)
    {
        Mat result = new Mat();
        Core.absdiff(srcA, srcB, result);
        return result;
    }

    private Mat GaussianBlur(Mat src, Size ksize, double sigmaX)
    {
        Mat result = new Mat();
        Imgproc.GaussianBlur(src, result, ksize, sigmaX);
        return result;
    }

    private void FindContours(Mat src, ref List<MatOfPoint> contours, ref Mat hierarchy)
    {
        Imgproc.findContours(src, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);
    }

    private Point FindCenter(MatOfPoint contour)
    {
        Moments m = Imgproc.moments(contour);
        return new Point((int)(m.m10 / m.m00), (int)(m.m01 / m.m00));
    }

    private void DrawContours(ref Mat src, List<MatOfPoint> contours, int i)
    {
        Imgproc.drawContours(src, contours, i, new Scalar(255, 0, 0), 3);
    }

    private void DrawCircle(ref Mat src, Point point)
    {
        Imgproc.circle(src, point, 7, new Scalar(255, 0, 0), -1);
    }

    private List<Point> MergeList(MatOfPoint contour, Point point)
    {
        List<Point> points = contour.toList();
        points.Insert(0, point);

        return points;
    }

    private void SetOffset(ref List<Point> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].x -= width / 2;
            points[i].y *= -1;
            points[i].y += height / 2;
        }
    }

    private List<Vector3> PointToVector3(List<Point> points)
    {
        List<Vector3> vector3 = new List<Vector3>();
        for (int i = 0; i < points.Count; i++)
            vector3.Add(new Vector3((float)points[i].x, (float)points[i].y, 0));
        return vector3;
    }
}
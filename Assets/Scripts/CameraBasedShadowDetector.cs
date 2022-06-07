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
    private bool isImage;
    [SerializeField]
    private string imagePath;

    private Texture2D texture;
    private WebCamTexture webCamTexture;
    private WebCamDevice webCamDevice;
    private Color32[] colors;
    private Mat src;
    private Mat gray = new Mat();
    private Mat thr = new Mat();


    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (isImage)
        {
            InitImage();
            OnInitImage();
        }
        else
        {
            InitCamera();
            OnInitCamera();
        }
    }

    private void InitCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        webCamDevice = devices[0];
        webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
        webCamTexture.Play();
    }

    private void OnInitCamera()
    {
        colors = new Color32[webCamTexture.width * webCamTexture.height];
        texture = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);
        src = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
        gameObject.GetComponent<Renderer>().material.mainTexture = texture;
    }

    private void InitImage()
    {
        src = Imgcodecs.imread(imagePath);
    }

    private void OnInitImage()
    {
        Run();
        texture = new Texture2D(src.cols(), src.rows(), TextureFormat.RGBA32, false);
        Utils.matToTexture2D (src, texture);
        gameObject.GetComponent<Renderer>().material.mainTexture = texture;
    }

    private void Update()
    {
        if (isImage)
            return;

        Utils.webCamTextureToMat(webCamTexture, src, colors);
        Run();
        Utils.matToTexture2D(src, texture, colors);
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

            List<Point> points = MergeList(contours[i], center);
            SetOffset(ref points);
            Shadow shadow = new Shadow(PointToVector3(points));
            MeshDrawer.Draw(shadow);
        }
    }

    private void ConvertToGrayscale()
    {
        Imgproc.cvtColor(src, gray, Imgproc.COLOR_BGR2GRAY);
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
        Imgproc.drawContours(src, contours, i, new Scalar(255, 0, 0), 3);
    }

    private void DrawCircle(Point point)
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
        for (int j = 0; j < points.Count; j++)
        {
            points[j].x -= requestedWidth / 2;
            points[j].y *= -1;
            points[j].y += requestedHeight / 2;
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
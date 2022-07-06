using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.UnityUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraBasedPoint
{
    [Range(0, 640)]
    public double x;
    [Range(0, 480)]
    public double y;

    public Point Get()
    {
        return new Point(x, y);
    }
}

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
        textureSrc = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureR = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureG = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureB = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureFrame = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureGray = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureThr = new Texture2D(width, height, TextureFormat.RGBA32, false);
        frame = new Mat(height, width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
        
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
        Utils.webCamTextureToMat(webCamTexture, frame, colors);
        Run();
    }

    private void Run()
    {
        Mat pts1 = new Mat(4, 1, CvType.CV_32FC2);
        Mat pts2 = new Mat(4, 1, CvType.CV_32FC2);
        pts1.put(0, 0, cameraBasedPoints[0].Get().x, cameraBasedPoints[0].Get().y,
                       cameraBasedPoints[1].Get().x, cameraBasedPoints[1].Get().y,
                       cameraBasedPoints[2].Get().x, cameraBasedPoints[2].Get().y,
                       cameraBasedPoints[3].Get().x, cameraBasedPoints[3].Get().y);
        pts2.put(0, 0, 0.0, 0.0, width, 0.0, 0.0, height, width, height);

        Mat mtrx = MyUtils.GetPerspectiveTransform(pts1, pts2);
        Mat src = MyUtils.WarpPerspective(frame, mtrx, new Size(width, height));

        for (int i = 0; i < cameraBasedPoints.Length; i++)
        {
            MyUtils.DrawCircle(ref frame, cameraBasedPoints[i].Get());
        }

        Mat rgb = new Mat(height, width, CvType.CV_8UC4, new Scalar(r, g, b, 0));
        Mat add = MyUtils.Add(src, rgb);
        rgb = new Mat(height, width, CvType.CV_8UC4, new Scalar(r * -1, g * -1, b * -1, 0));
        add = MyUtils.Subtract(add, rgb);
        List<Mat> split = new List<Mat>();
        Core.split(add, split);
        Mat gray = MyUtils.ConvertToGrayscale(add);
        Mat result = MyUtils.Threshold(gray, threshold, 255);
        Mat hierarchy = new Mat();
        List<MatOfPoint> contours = new List<MatOfPoint>();
        MyUtils.FindContours(result, ref contours, ref hierarchy);

        List<Shadow> shadows = new List<Shadow>();
        foreach (MatOfPoint c in contours)
        {
            double area = Imgproc.contourArea(c);
            if (area > 1000)
            {
                Point[] points = c.toArray();
                if (useApprox)
                {
                    MatOfPoint2f curve = new MatOfPoint2f(c.toArray());
                    MatOfPoint2f approx = new MatOfPoint2f();
                    double p = Imgproc.arcLength(curve, true);
                    Imgproc.approxPolyDP(curve, approx, epsilon * p, true);

                    points = approx.toArray();
                }
                SetOffset(ref points);
                Shadow shadow = new Shadow(MyUtils.PointToVector2(points));
                shadows.Add(shadow);
            }
        }
        MeshDrawer.Clear();
        MeshDrawer.Draw(shadows);

        Utils.matToTexture2D(src, textureSrc, colors);
        Utils.matToTexture2D(split[0], textureR, colors);
        Utils.matToTexture2D(split[1], textureG, colors);
        Utils.matToTexture2D(split[2], textureB, colors);
        Utils.matToTexture2D(frame, textureFrame, colors);
        Utils.matToTexture2D(gray, textureGray, colors);
        Utils.matToTexture2D(result, textureThr, colors);
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
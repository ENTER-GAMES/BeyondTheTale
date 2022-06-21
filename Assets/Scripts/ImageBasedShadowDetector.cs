using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.UnityUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageBasedShadowDetector : ShadowDetector
{
    [SerializeField]
    private GameObject texturePrefab;

    [SerializeField]
    private string originImagePath;
    [SerializeField]
    private string targetImagePath;

    private Texture2D textureOrigin;
    private Texture2D textureTarget;
    private Texture2D textureAbsDiff;
    private Texture2D textureBlur;
    private Texture2D textureResult;
    private Mat origin;
    private Mat target;
    private Mat diff;
    private Mat blur;
    private Mat result;

    private int width;
    private int height;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        origin = Imgcodecs.imread(originImagePath);
        target = Imgcodecs.imread(targetImagePath);

        width = Screen.width / 2;
        height = Screen.height / 2;

        Imgproc.resize(origin, origin, new Size(width, height));
        Imgproc.resize(target, target, new Size(width, height));

        diff = new Mat();
        blur = new Mat();
        result = new Mat();
        origin.copyTo(diff);
        origin.copyTo(blur);
        origin.copyTo(result);

        textureOrigin = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureTarget = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureAbsDiff = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureBlur = new Texture2D(width, height, TextureFormat.RGBA32, false);
        textureResult = new Texture2D(width, height, TextureFormat.RGBA32, false);

        GameObject originObj = Instantiate(texturePrefab);
        originObj.name = "Origin";
        originObj.GetComponent<MeshRenderer>().material.mainTexture = textureOrigin;
        GameObject targetObj = Instantiate(texturePrefab);
        targetObj.name = "Target";
        targetObj.GetComponent<MeshRenderer>().material.mainTexture = textureTarget;
        GameObject absDiffObj = Instantiate(texturePrefab);
        absDiffObj.name = "AbsDiff";
        absDiffObj.GetComponent<MeshRenderer>().material.mainTexture = textureAbsDiff;
        GameObject blurObj = Instantiate(texturePrefab);
        blurObj.name = "Blur";
        blurObj.GetComponent<MeshRenderer>().material.mainTexture = textureBlur;
        GameObject resultObj = Instantiate(texturePrefab);
        resultObj.name = "Result";
        resultObj.GetComponent<MeshRenderer>().material.mainTexture = textureResult;

        OnInit();
    }

    private void OnInit()
    {
        Utils.matToTexture2D(origin, textureOrigin);
        Utils.matToTexture2D(target, textureTarget);
        Utils.matToTexture2D(diff, textureAbsDiff);
        Utils.matToTexture2D(blur, textureBlur);
        Utils.matToTexture2D(result, textureResult);

        Run();
    }

    private void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            Point point = new Point((double)Input.mousePosition.x, (double)Input.mousePosition.y);
            DrawCircle(ref origin, point);
            Debug.Log($"{width}, {height}, {origin.cols()}, {origin.rows()}, {point}");
            Utils.matToTexture2D(origin, textureOrigin, false);

            points[pointCount] = point;
            pointCount++;
        }
        */
    }

    private void Run()
    {
        Mat gray_origin = ConvertToGrayscale(origin);
        Mat gray_target = ConvertToGrayscale(target);
        diff = AbsDiff(gray_origin, gray_target);
        blur = GaussianBlur(diff, new Size(9, 9), 0);
        Mat thresh = Threshold(blur, 0, 255);
        List<MatOfPoint> contours = new List<MatOfPoint>();
        Mat hierarchy = new Mat();
        FindContours(thresh, ref contours, ref hierarchy);
        result = thresh;

        MeshDrawer.Clear();

        for (int i = 0; i < contours.Count; ++i)
        {
            Point center = FindCenter(contours[i]);

            DrawContours(ref result, contours, i);
            //DrawCircle(center);

            List<Point> points = MergeList(contours[i], center);
            SetOffset(ref points);
            Shadow shadow = new Shadow(PointToVector3(points));
            MeshDrawer.Draw(shadow);
        }

        Utils.matToTexture2D(origin, textureOrigin, false);
        Utils.matToTexture2D(target, textureTarget, false);
        Utils.matToTexture2D(diff, textureAbsDiff, false);
        Utils.matToTexture2D(blur, textureBlur, false);
        Utils.matToTexture2D(result, textureResult, false);
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

    private Mat AbsDiff(Mat src1, Mat src2)
    {
        Mat result = new Mat();
        Core.absdiff(src1, src2, result);
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
        Imgproc.findContours(src, contours, hierarchy, Imgproc.RETR_LIST, Imgproc.CHAIN_APPROX_SIMPLE);
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
        for (int j = 0; j < points.Count; j++)
        {
            points[j].x -= width / 2;
            points[j].y -= height / 2;
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
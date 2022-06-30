using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using System.Collections.Generic;
using UnityEngine;

public static class MyUtils
{
    public static Mat ConvertToGrayscale(Mat src)
    {
        Mat result = new Mat();
        Imgproc.cvtColor(src, result, Imgproc.COLOR_BGR2GRAY);
        return result;
    }

    public static Mat Threshold(Mat src, int threshold, int max)
    {
        Mat result = new Mat();
        Imgproc.threshold(src, result, threshold, max, Imgproc.THRESH_BINARY);
        return result;
    }

    public static Mat AbsDiff(Mat srcA, Mat srcB)
    {
        Mat result = new Mat();
        Core.absdiff(srcA, srcB, result);
        return result;
    }

    public static Mat GaussianBlur(Mat src, Size ksize, double sigmaX)
    {
        Mat result = new Mat();
        Imgproc.GaussianBlur(src, result, ksize, sigmaX);
        return result;
    }

    public static void FindContours(Mat src, ref List<MatOfPoint> contours, ref Mat hierarchy)
    {
        Imgproc.findContours(src, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);
    }

    public static Point FindCenter(MatOfPoint contour)
    {
        Moments m = Imgproc.moments(contour);
        return new Point((int)(m.m10 / m.m00), (int)(m.m01 / m.m00));
    }

    public static void DrawContours(ref Mat src, List<MatOfPoint> contours, int i)
    {
        Imgproc.drawContours(src, contours, i, new Scalar(255, 0, 0), 3);
    }

    public static void DrawCircle(ref Mat src, Point point)
    {
        Imgproc.circle(src, point, 7, new Scalar(255, 0, 0), -1);
    }

    public static Vector2[] PointToVector2(Point[] points)
    {
        Vector2[] vectors = new Vector2[points.Length];

        for (int i = 0; i < points.Length; i++)
            vectors[i] = new Vector2((float)points[i].x, (float)points[i].y);

        return vectors;
    }
}
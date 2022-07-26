using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using System.Collections.Generic;
using UnityEngine;

public static class CVUtils
{
    public static Mat ConvertToGrayscale(Mat src)
    {
        Mat dst = new Mat();
        Imgproc.cvtColor(src, dst, Imgproc.COLOR_BGR2GRAY);
        return dst;
    }

    public static Mat Threshold(Mat src, int threshold, int max)
    {
        Mat dst = new Mat();
        Imgproc.threshold(src, dst, threshold, max, Imgproc.THRESH_BINARY_INV);
        return dst;
    }

    public static Mat AbsDiff(Mat srcA, Mat srcB)
    {
        Mat dst = new Mat();
        Core.absdiff(srcA, srcB, dst);
        return dst;
    }

    public static Mat GaussianBlur(Mat src, Size ksize, double sigmaX)
    {
        Mat dst = new Mat();
        Imgproc.GaussianBlur(src, dst, ksize, sigmaX);
        return dst;
    }

    public static void FindContours(Mat src, ref List<MatOfPoint> contours, ref Mat hierarchy)
    {
        Imgproc.findContours(src, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);
    }

    public static List<MatOfPoint> FindContours(Mat src)
    {
        Mat hierarchy = new Mat();
        List<MatOfPoint> contours = new List<MatOfPoint>();
        Imgproc.findContours(src, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);
        return contours;
    }

    public static Point FindCenter(MatOfPoint contour)
    {
        Moments m = Imgproc.moments(contour);
        return new Point((int)(m.m10 / m.m00), (int)(m.m01 / m.m00));
    }

    public static void DrawContours(ref Mat src, List<MatOfPoint> contours, int i)
    {
        Imgproc.drawContours(src, contours, i, new Scalar(255, 0, 0, 255), 3);
    }

    public static void DrawCircle(ref Mat src, Point point)
    {
        Imgproc.circle(src, point, 20, new Scalar(255, 0, 0, 255), -1);
    }

    public static Vector2[] PointToVector2(Point[] points)
    {
        Vector2[] vectors = new Vector2[points.Length];

        for (int i = 0; i < points.Length; i++)
            vectors[i] = new Vector2((float)points[i].x, (float)points[i].y);

        return vectors;
    }

    public static Mat Add(Mat src1, Mat src2)
    {
        Mat dst = new Mat();
        Core.add(src1, src2, dst);
        return dst;
    }

    public static Mat Subtract(Mat src1, Mat src2)
    {
        Mat dst = new Mat();
        Core.subtract(src1, src2, dst);
        return dst;
    }

    public static Mat GetPerspectiveTransform(Mat src, Mat dst)
    {
        return Imgproc.getPerspectiveTransform(src, dst);
    }

    public static Mat WarpPerspective(Mat src, Mat M, Size dsize)
    {
        Mat dst = new Mat();
        Imgproc.warpPerspective(src, dst, M, dsize);
        return dst;
    }
}
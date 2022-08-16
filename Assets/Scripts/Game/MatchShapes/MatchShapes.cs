using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.UnityUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchShapes : MonoBehaviour
{
    [SerializeField]
    private Texture2D textureObj;
    [SerializeField]
    private Renderer rendererObj;
    [SerializeField]
    private Renderer rendererSrc;
    [SerializeField]
    private CameraBasedShadowDetector detector;
    [SerializeField]
    private TextMeshProUGUI text;

    private Mat obj;
    private Mat src;
    private Mat dst;
    private MatOfPoint obj_pts;
    private Texture2D obj_tex;
    private Texture2D src_tex;

    private void Start()
    {
        obj = new Mat(textureObj.height, textureObj.width, CvType.CV_8UC1, new Scalar(0, 0, 0, 255));
        src = new Mat(textureObj.height, textureObj.width, CvType.CV_8UC1, new Scalar(255, 255, 255, 255));
        Utils.texture2DToMat(textureObj, obj, false, 0);

        Mat obj_bin = CVUtils.Threshold(obj, 128, 255);
        List<MatOfPoint> obj_contours = CVUtils.FindContours(obj_bin);
        obj_pts = obj_contours[0];

        obj_tex = new Texture2D(textureObj.width, textureObj.height, TextureFormat.RGBA32, false);
        src_tex = new Texture2D(textureObj.width, textureObj.height, TextureFormat.RGBA32, false);

        rendererObj.material.mainTexture = obj_tex;
        rendererSrc.material.mainTexture = src_tex;

        Utils.matToTexture2D(obj, obj_tex, false, 0, false);
        Utils.matToTexture2D(src, src_tex, false, 0, false);
    }

    private void Update()
    {
        dst = new Mat(textureObj.height, textureObj.width, CvType.CV_8UC1, new Scalar(255, 255, 255, 255));
        Core.subtract(src, GetMatFromCamera(), dst);
        UpdateTexture();
    }

    public void Draw()
    {
        Core.subtract(src, GetMatFromCamera(), src);
        UpdateTexture();
    }

    public void Erase()
    {
        Core.add(src, GetMatFromCamera(), src);
        UpdateTexture();
    }

    public void Clear()
    {
        src = new Mat(textureObj.height, textureObj.width, CvType.CV_8UC1, new Scalar(255, 255, 255, 255));
        UpdateTexture();
    }

    private Mat GetMatFromCamera()
    {
        Mat frame = detector.GetResult();
        Imgproc.resize(frame, frame, new Size(src_tex.width, src_tex.height));
        return frame;
    }

    private void Match()
    {
        Mat src_bin = CVUtils.Threshold(dst, 128, 255);
        List<MatOfPoint> src_contours = CVUtils.FindContours(src_bin);
        double minDist = double.MaxValue;
        foreach (MatOfPoint pts in src_contours)
        {
            if (Imgproc.contourArea(pts) < 1000)
                continue;
            double dist = Imgproc.matchShapes(obj_pts, pts, Imgproc.CONTOURS_MATCH_I3, 0);
            if (dist < minDist)
                minDist = dist;
            Debug.Log(dist);
        }
        Print(minDist.ToString());
    }

    private void Print(string str)
    {
        text.text = str;
    }

    private void UpdateTexture()
    {
        Match();

        Mat tempColor = new Mat(textureObj.height, textureObj.width, CvType.CV_8UC1, new Scalar(100, 100, 100, 255));
        Utils.matToTexture2D(dst + tempColor, src_tex, false, 0, false);
    }
}

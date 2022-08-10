using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.UnityUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chapter_5_2
{
    public class BackfaceMask : MonoBehaviour
    {
        [SerializeField]
        private Texture2D texture1;
        [SerializeField]
        private Texture2D texture2;
        [SerializeField]
        private Renderer renderer;
        [SerializeField]
        private CameraBasedShadowDetector detector;

        private Mat src1;
        private Mat src2;
        private Mat dst;
        private Texture2D tex;

        private int width;
        private int height;

        private void Start()
        {
            width = texture1.width;
            height = texture1.height;

            src1 = new Mat(height, width, CvType.CV_8UC4, new Scalar(255, 255, 255, 255));
            src2 = new Mat(height, width, CvType.CV_8UC4, new Scalar(255, 255, 255, 255));
            dst = new Mat(height, width, CvType.CV_8UC4, new Scalar(255, 255, 255, 255));
            Utils.texture2DToMat(texture1, src1, false, 0);
            Utils.texture2DToMat(texture2, src2, false, 0);

            tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            renderer.material.mainTexture = tex;
        }

        private void Update()
        {
            dst = src2.clone();
            Mat mask = GetMatFromCamera();
            src1.copyTo(dst, mask);
            Utils.matToTexture2D(dst, tex, false, 0, false);
        }

        private Mat GetMatFromCamera()
        {
            Mat frame = detector.GetResult();
            Imgproc.resize(frame, frame, new Size(width, height));
            return frame;
        }
    }
}
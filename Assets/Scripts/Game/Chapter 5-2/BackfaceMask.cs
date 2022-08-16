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
        private Texture2D textureBack;
        [SerializeField]
        private Texture2D textureFront;
        [SerializeField]
        private Renderer renderer;
        [SerializeField]
        private CameraBasedShadowDetector detector;

        private Mat matBack;
        private Mat matFront;
        private Mat dst;
        private Texture2D tex;

        private int width;
        private int height;

        private void Start()
        {
            width = textureBack.width;
            height = textureBack.height;

            matBack = new Mat(height, width, CvType.CV_8UC4, new Scalar(255, 255, 255, 255));
            matFront = new Mat(height, width, CvType.CV_8UC4, new Scalar(255, 255, 255, 255));
            dst = new Mat(height, width, CvType.CV_8UC4, new Scalar(255, 255, 255, 255));
            Utils.texture2DToMat(textureBack, matBack, false, 0);
            Utils.texture2DToMat(textureFront, matFront, false, 0);

            tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            renderer.material.mainTexture = tex;
        }

        private void Update()
        {
            dst = matBack.clone();
            Mat mask = GetMatFromCamera();
            matFront.copyTo(dst, mask);
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
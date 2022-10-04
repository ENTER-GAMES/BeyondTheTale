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
        private new Renderer renderer;
        [SerializeField]
        private CameraBasedShadowDetector detector;

        private Mat matBack;
        private Mat matFront;
        private Texture2D tex;

        private int width;
        private int height;

        private bool hasInitDone = false;

        public void Init()
        {
            width = textureBack.width;
            height = textureBack.height;

            matBack = new Mat(height, width, CvType.CV_8UC4, new Scalar(255, 255, 255, 255));
            matFront = new Mat(height, width, CvType.CV_8UC4, new Scalar(255, 255, 255, 255));
            Utils.texture2DToMat(textureBack, matBack);
            Utils.texture2DToMat(textureFront, matFront);

            tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            renderer.material.mainTexture = tex;

            hasInitDone = true;
        }

        private void Update()
        {
            if (!hasInitDone) return;

            Mat src = matBack.clone();
            Mat mask = GetMatFromCamera();
            Mat mask_inv = mask.clone();
            Core.bitwise_not(mask, mask_inv);
            Mat result = new Mat(height, width, CvType.CV_8UC4, new Scalar(255, 255, 255, 0));
            Core.bitwise_and(src, src, result, mask_inv);
            Utils.matToTexture2D(result, tex);
        }

        private Mat GetMatFromCamera()
        {
            Mat frame = detector.GetResult();
            Imgproc.resize(frame, frame, new Size(width, height));
            //Core.flip(frame, frame, 0);
            return frame;
        }
    }
}
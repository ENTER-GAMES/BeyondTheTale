using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.VideoioModule;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VideoTextureToMat : MonoBehaviour
{
    [SerializeField] public int requestedWidth = 640;
    [SerializeField] public int requestedHeight = 360;
    VideoCapture capture;
    Mat rgbMat;
    bool isPlaying = false;
    bool shouldUpdateVideoFrame = false;
    long prevFrameTickCount;
    long currentFrameTickCount;
    [SerializeField] private string videoFilePath;
    [SerializeField] private CameraBasedShadowDetector detector;

    void Start()
    {
        capture = new VideoCapture();
        capture.open(Utils.getFilePath(videoFilePath));

        Initialize();
    }

    private void Initialize()
    {
        rgbMat = new Mat();

        if (!capture.isOpened())
        {
            Debug.LogError(videoFilePath + " is not opened. Please move from ¡°OpenCVForUnity/StreamingAssets/¡± to ¡°Assets/StreamingAssets/¡± folder.");
        }

        Debug.Log("CAP_PROP_FORMAT: " + capture.get(Videoio.CAP_PROP_FORMAT));
        Debug.Log("CAP_PROP_POS_MSEC: " + capture.get(Videoio.CAP_PROP_POS_MSEC));
        Debug.Log("CAP_PROP_POS_FRAMES: " + capture.get(Videoio.CAP_PROP_POS_FRAMES));
        Debug.Log("CAP_PROP_POS_AVI_RATIO: " + capture.get(Videoio.CAP_PROP_POS_AVI_RATIO));
        Debug.Log("CAP_PROP_FRAME_COUNT: " + capture.get(Videoio.CAP_PROP_FRAME_COUNT));
        Debug.Log("CAP_PROP_FPS: " + capture.get(Videoio.CAP_PROP_FPS));
        Debug.Log("CAP_PROP_FRAME_WIDTH: " + capture.get(Videoio.CAP_PROP_FRAME_WIDTH));
        Debug.Log("CAP_PROP_FRAME_HEIGHT: " + capture.get(Videoio.CAP_PROP_FRAME_HEIGHT));
        double ext = capture.get(Videoio.CAP_PROP_FOURCC);
        Debug.Log("CAP_PROP_FOURCC: " + (char)((int)ext & 0XFF) + (char)(((int)ext & 0XFF00) >> 8) + (char)(((int)ext & 0XFF0000) >> 16) + (char)(((int)ext & 0XFF000000) >> 24));

        capture.grab();
        capture.retrieve(rgbMat);
        int frameWidth = rgbMat.cols();
        int frameHeight = rgbMat.rows();
        capture.set(Videoio.CAP_PROP_POS_FRAMES, 0);

        StartCoroutine("WaitFrameTime");

        isPlaying = true;

        detector.Initialize(requestedWidth, requestedHeight);
        detector.onDispose.AddListener(Dispose);
    }

    void Update()
    {
        if (isPlaying && shouldUpdateVideoFrame)
        {
            shouldUpdateVideoFrame = false;

            if (capture.get(Videoio.CAP_PROP_POS_FRAMES) >= capture.get(Videoio.CAP_PROP_FRAME_COUNT))
                capture.set(Videoio.CAP_PROP_POS_FRAMES, 0);

            if (capture.grab())
            {
                capture.retrieve(rgbMat);

                Imgproc.cvtColor(rgbMat, rgbMat, Imgproc.COLOR_BGR2RGBA);
                Imgproc.resize(rgbMat, rgbMat, new Size(requestedWidth, requestedHeight));
                detector.Run(rgbMat);
            }
            else
            {
                capture.open(Utils.getFilePath(videoFilePath));
                capture.set(Videoio.CAP_PROP_POS_FRAMES, 0);
            }
        }
    }

    private IEnumerator WaitFrameTime()
    {
        double videoFPS = (capture.get(Videoio.CAP_PROP_FPS) <= 0) ? 10.0 : capture.get(Videoio.CAP_PROP_FPS);
        float frameTime_sec = (float)(1000.0 / videoFPS / 1000.0);
        WaitForSeconds wait = new WaitForSeconds(frameTime_sec);
        prevFrameTickCount = currentFrameTickCount = Core.getTickCount();

        capture.grab();

        while (true)
        {
            if (isPlaying)
            {
                shouldUpdateVideoFrame = true;

                prevFrameTickCount = currentFrameTickCount;
                currentFrameTickCount = Core.getTickCount();

                yield return wait;
            }
            else
            {
                yield return null;
            }
        }
    }

    void OnDestroy()
    {
        Dispose();
    }

    private void Dispose()
    {
        StopCoroutine("WaitFrameTime");

        capture.release();

        if (rgbMat != null)
            rgbMat.Dispose();
    }
}
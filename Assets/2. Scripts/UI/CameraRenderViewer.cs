using UnityEngine;
using UnityEngine.UI;

public class CameraRenderViewer : MonoBehaviour
{
    [SerializeField]
    private RawImage rawImage;

    private void Update()
    {
        if (rawImage.texture == null)
            rawImage.texture = FindObjectOfType<CameraBasedShadowDetector>().GetSrcTexture();
    }
}

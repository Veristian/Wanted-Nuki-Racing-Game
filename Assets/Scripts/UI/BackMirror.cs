using UnityEngine;
using UnityEngine.UI;

public class BackMirror : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private RawImage image; // Using RawImage for RenderTexture
    [SerializeField] private int resolution = 100;

    private void Awake()
    {
        if (cam == null || image == null) return;

        // Calculate desired resolution
        Vector2 size = image.rectTransform.sizeDelta;
        int width = Mathf.CeilToInt(size.x * resolution);
        int height = Mathf.CeilToInt(size.y * resolution);

        // Create new RenderTexture
        RenderTexture renderTexture = new RenderTexture(width, height, 16);
        renderTexture.Create();

        // Assign RenderTexture
        cam.targetTexture = renderTexture;
        image.texture = renderTexture;
    }
}

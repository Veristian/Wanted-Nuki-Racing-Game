using UnityEngine;

public class BlinkLayer : MonoBehaviour
{
    [Header("Layer Number")]
    [SerializeField] private int normalLayer;
    [SerializeField] private int blinkLayer;
    [Header("Settings")]
    [SerializeField] private float blinkInterval = 0.5f;
    [SerializeField] private bool startBlinking = true;

    private float timer;

    public void StartBlink()
    {
        timer = 0;
        startBlinking = true;
    }

    public void StopBlink()
    {
        startBlinking = false;
        gameObject.layer = normalLayer; // Optional: reset to normal when stopping
    }

    private void Update()
    {
        if (!startBlinking) return;

        timer += Time.deltaTime;

        if (timer < blinkInterval)
        {
            gameObject.layer = blinkLayer;
        }
        else if (timer < blinkInterval * 2)
        {
            gameObject.layer = normalLayer;
        }
        else
        {
            timer = 0f;
        }
    }
}

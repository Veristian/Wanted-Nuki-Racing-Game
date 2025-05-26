using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointLightBlinking : MonoBehaviour
{
    public Light pointLight;
    public Color colorA = Color.red;
    public Color colorB = Color.blue;
    public float speed = 0.5f;

    private int currentColor = 0;
    private float timer = 0f;

    void Start()
    {
        if (pointLight == null)
            pointLight = GetComponent<Light>();

    }

    void Update()
    {
        timer += Time.deltaTime *speed;

        bool useRed = Mathf.PingPong(timer, 1f) < 0.5f;
        Color currentColor = useRed ? colorA : colorB;

        pointLight.color = currentColor;
    }
}

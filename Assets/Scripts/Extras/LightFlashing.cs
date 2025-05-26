using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlashing : MonoBehaviour
{
    public Material policeMaterial;
    public Color colorA = Color.red;
    public Color colorB = Color.blue;
    public float flashSpeed = 2f;
    private float localTime;
    public float timeOffset = 0f;



    void Start()
    {

        localTime = timeOffset;
    }
    void Update()
    {
        localTime += Time.deltaTime * flashSpeed;

        bool useRed = Mathf.PingPong(localTime, 1f) < 0.5f;
        Color currentColor = useRed ? colorA : colorB;

        policeMaterial.SetColor("_EmissionColor", currentColor);

        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            DynamicGI.SetEmissive(rend, currentColor);
        }
    } 
}

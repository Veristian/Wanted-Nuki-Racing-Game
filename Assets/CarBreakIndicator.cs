using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBreakIndicator : MonoBehaviour
{
    public Material material;
    public Color baseColor = Color.red;
    public float Intensity = 3f;
    // Start is called before the first frame update
    void Start()
    {
        material.EnableKeyword("_EMISSION");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Color finalColor = baseColor * Mathf.LinearToGammaSpace(Intensity);
            material.SetColor("_EmissionColor", finalColor);
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            Color offColor = baseColor * 0f;
            material.SetColor("_EmissionColor", offColor);
        }
    }
}

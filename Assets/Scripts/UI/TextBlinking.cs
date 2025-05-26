using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class TextBlinking : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float speed = 1f;
    void Start()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }
            

        StartCoroutine(Blink());
    }

    System.Collections.IEnumerator Blink()
    {
        while (true)
        {
            text.enabled = !text.enabled;
            yield return new WaitForSeconds(speed);
        }
    }

}

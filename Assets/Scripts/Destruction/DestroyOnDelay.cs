using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnDelay : MonoBehaviour
{
    [SerializeField] public float delay = 2f; // Delay in seconds before destruction
    [HideInInspector] public bool startDestroy = false;
    private bool isDestroyed = false;
    private void Update()
    {
        if (startDestroy && !isDestroyed)
        {
            isDestroyed = true; // Prevent multiple calls to the coroutine
            StartCoroutine(DestroyAfterDelay());
        }
    }
    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}

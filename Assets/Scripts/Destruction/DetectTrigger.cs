using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destructable destructable = GetComponentInParent<Destructable>();
            if (destructable != null)
            {
                destructable.OnDestructionEvent.Invoke();
            }
        }
    }
}

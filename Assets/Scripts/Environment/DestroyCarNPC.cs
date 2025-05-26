using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCarNPC : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        print("Car Destroyed");
        if (other.GetComponent<CarNPC>() != null)
        {
            Destroy(other.gameObject); 
        }
    }
}

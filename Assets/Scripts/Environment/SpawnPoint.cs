using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public void SendToSpawn()
    {
        FindAnyObjectByType<CarController>().gameObject.transform.position = transform.position;
    }
}

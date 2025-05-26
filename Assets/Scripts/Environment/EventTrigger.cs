using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class EventTrigger : MonoBehaviour
{
    public UnityEvent OnTriggerEnterEvent;

    private void Awake()
    {
        if (OnTriggerEnterEvent == null)
        {
            OnTriggerEnterEvent = new UnityEvent();
        }
        GetComponent<BoxCollider>().isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnTriggerEnterEvent.Invoke();
        }
    }
}

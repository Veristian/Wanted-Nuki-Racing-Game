using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(DestroyOnDelay), typeof(AudioSource))]
public class Destructable : MonoBehaviour
{
    [SerializeField] private GameObject[] originalObjects;
    [SerializeField] private GameObject[] destructableObjects;

    [SerializeField] public UnityEvent OnDestructionEvent;
    [Header("Settings")]
    [SerializeField] private AudioClip destroyedAudio;
    private AudioSource audioSource;
    private void Awake()
    {
        if (OnDestructionEvent == null) 
        {
            OnDestructionEvent = new UnityEvent();
        }
        OnDestructionEvent.AddListener(ActivateDestruction);
        if (destroyedAudio != null) 
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = destroyedAudio;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D sound
        }
    }
    private void OnValidate()
    {
        if (originalObjects.Length > 0) 
        {
            foreach (var obj in originalObjects) 
            {
                if (obj.GetComponent<MeshCollider>() == null) 
                {
                    obj.AddComponent<MeshCollider>();
                }
                if (obj.GetComponent<DetectTrigger>() == null) 
                {
                    obj.AddComponent<DetectTrigger>();
                }
                MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
                meshCollider.convex = true;
                meshCollider.isTrigger = true;
            }
        }
        if (destructableObjects.Length > 0) 
        {
            foreach (var obj in destructableObjects) 
            {
                if (obj.GetComponent<MeshCollider>() == null) 
                {
                    obj.AddComponent<MeshCollider>();
                }
                if (obj.GetComponent<Rigidbody>() == null) 
                {
                    obj.AddComponent<Rigidbody>();
                }
                MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
                meshCollider.convex = true;
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                rb.mass = 0.01f;
                obj.SetActive(false);
            }
        }


    }

    public void ActivateDestruction()
    {
        if (destroyedAudio != null) 
        {
            audioSource.PlayOneShot(destroyedAudio);
        }
        foreach (var obj in originalObjects) 
        {
            obj.SetActive(false);
        }
        foreach (var obj in destructableObjects) 
        {
            obj.SetActive(true);
        }
        DestroyOnDelay destroyOnDelay = GetComponent<DestroyOnDelay>();
        if (destroyOnDelay != null) 
        {
            destroyOnDelay.startDestroy = true;
        }
        
    }

}

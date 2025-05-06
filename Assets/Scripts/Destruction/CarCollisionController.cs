using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Events;

public class CarCollisionController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask environmentCrashableLayer;
    [SerializeField] [Tooltip("The delta velocity magnitude to car damage ratio")] private AnimationCurve crashDamageScaler;
    [SerializeField] [Tooltip("The difference of each car velocity normalized to damage ratio")] private AnimationCurve carCollisionDamageScaler;
    
    [SerializeField] [Tooltip("The threshold difference in velocity magnitude for cars to take damage")] private float carDamagingThreshold = 20;
    //reference
    private Rigidbody carRB;

    //colliding with environment
    private Vector3 currentVelocity;
    private Vector3 previousVelocity;

    //colliding with car

    private Rigidbody otherCarRB;
    private float CarAttackProduct;

    private Vector3 thisCarAttackVelocity;
    private Vector3 otherCarAttackVelocity;
    private Vector3 collisionNormal;

    //event
    private UnityEvent OnCarCollide; //collide with other cars
    private UnityEvent OnCarCrash; //collide with environment

    //damage
    private float damage;
    [SerializeField] private float carMaxHealth = 100;
    private float carCurrentHealth;

   
    private void Awake()
    {
        carCurrentHealth = carMaxHealth;
        if (OnCarCollide == null)
        {
            OnCarCollide = new UnityEvent();
        }
        if (OnCarCrash == null)
        {
            OnCarCrash = new UnityEvent();
        }
        carRB = GetComponent<Rigidbody>();

        //Event listeners
        OnCarCollide.AddListener(() => CollideDamageCalculator());
        OnCarCrash.AddListener(() => CrashDamageCalculator());

    }

    private void FixedUpdate()
    {
        previousVelocity = currentVelocity;
        currentVelocity = carRB.velocity;
    }
    private void Update()
    {
        if (damage > 0)
        {
            carCurrentHealth -= damage;
            damage = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check for environment collision
        if (((1 << collision.gameObject.layer) & environmentCrashableLayer) != 0)
        {
            OnCarCrash.Invoke();
        }

        // Check for car collision
        var otherCar = collision.gameObject.GetComponent<CarCollisionController>();
        if (otherCar != null)
        {
            otherCarRB = otherCar.GetComponent<Rigidbody>();

            // Store the impact velocities based on the collision normal
            collisionNormal = collision.contacts[0].normal;

            thisCarAttackVelocity = Vector3.Project(previousVelocity, -collisionNormal);
            otherCarAttackVelocity = Vector3.Project(otherCar.previousVelocity, collisionNormal); // Flip normal for the other car

            OnCarCollide.Invoke(); // this car calculates its own damage
        }
    }
   

    #region Crash
    private void CrashDamageCalculator()
    {
        if (Mathf.Abs(previousVelocity.magnitude) > carDamagingThreshold)
        {
            damage = crashDamageScaler.Evaluate(Mathf.Abs(previousVelocity.magnitude));
        }
    }
    #endregion

    #region Collide
    private void CollideDamageCalculator()
    {
        // Calculate relative impact force in normal direction
        Vector3 relativeImpact = (thisCarAttackVelocity - otherCarAttackVelocity);
        print(previousVelocity);
        print(Vector3.Dot(relativeImpact,otherCarAttackVelocity));
        print(relativeImpact.magnitude);
        if (relativeImpact.magnitude > carDamagingThreshold && Vector3.Dot(relativeImpact,transform.position - collisionNormal) < 0)
        {
            damage = carCollisionDamageScaler.Evaluate(relativeImpact.magnitude);
        }
    }
    #endregion

}

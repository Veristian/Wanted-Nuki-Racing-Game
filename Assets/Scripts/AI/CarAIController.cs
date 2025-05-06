using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CarAIController : CarController
{
    NavMeshAgent navMeshAgent;
    public Transform carPos;
    public AnimationCurve speedCurve;
    public bool Active;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.isStopped = !Active;

    }

    public override void GetInputs()
    {
        navMeshAgent.isStopped = !Active;
        if (Active)
        {
            navMeshAgent.destination = carPos.position;
            if (navMeshAgent.path.corners.Length > 2)
            {
                // Calculate distance to next curve
                float distanceToNextCorner = Vector3.Distance(transform.position, navMeshAgent.path.corners[1]);

                // Define a dynamic slowdown factor based on an AnimationCurve
                float slowdownFactor = speedCurve.Evaluate(distanceToNextCorner/100);

                // Apply slowdown factor dynamically
                verticalInput = Mathf.Lerp(1f, 0.2f, slowdownFactor);
            }
            else
            {
                // No more path curves, check if facing the destination
                Vector3 directionToTarget = (navMeshAgent.destination - transform.position).normalized;
                float dotProduct = Vector3.Dot(transform.forward, directionToTarget);

                if (dotProduct > 0.95f) // Very close to directly facing the target
                {
                    // Lock rotation to face the target for higher accuracy
                    transform.forward = Vector3.Lerp(transform.forward, directionToTarget, Time.deltaTime * 100f);
                    verticalInput = 1f; // Full speed ahead
                }
                else if (dotProduct > 0.9f) // Almost aligned
                {
                    verticalInput = 0.8f; // Speed up while adjusting
                }
                else
                {
                    verticalInput = 0.5f; // Slow down if not aligned properly
                }
            }
        }
        else
        {
            verticalInput = 0;
        }
        
        
    }
}
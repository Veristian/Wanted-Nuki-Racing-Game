using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class CarNPC : MonoBehaviour
{
    public float speed = 10f;
    public LayerMask groundLayer;

    private BoxCollider boxCollider;
    private Rigidbody rb; // Rigidbody component for physics interactions
    private float rayOffset = 2f; // Offset from center for front and back raycasts
    private float rayDistance = Mathf.Infinity; // Distance for raycasts

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = new Vector3(2f, 1f, 4f); // Adjust based on your car model dimensions
        boxCollider.center = new Vector3(0f, 0.5f, 0f);

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Set Rigidbody to kinematic for manual control
        rb.useGravity = false; // Disable gravity for the car
        rb.constraints = RigidbodyConstraints.FreezeAll; // Freeze all constraints to prevent unwanted movement

        
    }

    void FixedUpdate()
    {
        Vector3 up = transform.up;
        Vector3 forward = transform.forward;

        Vector3 frontRayOrigin = transform.position + forward * rayOffset + up * 1f;
        Vector3 backRayOrigin = transform.position - forward * rayOffset + up * 1f;

        bool frontHit = Physics.Raycast(frontRayOrigin, -up, out RaycastHit hitFront, rayDistance, groundLayer);
        bool backHit = Physics.Raycast(backRayOrigin, -up, out RaycastHit hitBack, rayDistance, groundLayer);

        if (frontHit && backHit)
        {
            // Position car based on mid-point between front and back hits
            Vector3 avgPoint = (hitFront.point + hitBack.point) / 2;
            transform.position = avgPoint;

            // Rotate car to match ground tilt
            Vector3 groundDir = hitFront.point - hitBack.point;
            transform.rotation = Quaternion.LookRotation(groundDir.normalized, hitBack.normal);

            // Move car forward
            transform.position += transform.forward * speed * Time.fixedDeltaTime;

            Debug.DrawRay(frontRayOrigin, -up * rayDistance, Color.red);
            Debug.DrawRay(backRayOrigin, -up * rayDistance, Color.red);
        }
    }
}

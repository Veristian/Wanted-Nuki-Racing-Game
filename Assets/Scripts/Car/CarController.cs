using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    [Header("General")]
    public Rigidbody rb;
    public TrailRenderer[] TyreMarks;
    Vector3 forwardDirection;

    [Header("Turning")]
    public float  turnStrength=180;
    public float  driftTurnStrength = 90;
    public float accelTurnStrength = 60;
    public AnimationCurve velocityToTurningCurve;
    public float turnStrengthVelocityCheckMultiplier;
    protected float lastTurnInput;
    
    [Header("Acceleration")]
    public float reverseAccel = 2;
    public velocityMatrix[] velocityMatrices;
    public float driftDrag = 0.7f;
    public float accelDrag = 0.4f;

    [Header("Threholds")]
    public float timeThreshold = 2;
    public float timeReverseThreshold = 4;
    [System.Serializable]
    public struct velocityMatrix
    {
        public float force;
        public float maxSpeed;
    }
    protected float verticalInput, speedInput, turnInput, Accel, timeAtMaxVelocity, timeToReverse;
    
    // Start is called before the first frame update
    private int currentAccel;

    private bool isReversing, isGrounded;
    public bool isDrifting {get; set;}

    public static float currentSpeed {get; private set;}
    public LayerMask groundLayer;
    public float groundCheckDistance = 1;

    [Header("Rotation Handler")]
    //the body
    [SerializeField] private GameObject carBody;
    [SerializeField] private GameObject frontRight;
    [SerializeField] private GameObject backRight;
    [SerializeField] private GameObject frontLeft;
    [SerializeField] private GameObject backLeft;
    [SerializeField] private float wheelCheckCollider = 0.51f;
    [SerializeField][Range(1,100)] private float rotationLerpSpeed;
    [SerializeField][Range(1,100)] private float wheelTurnAngleSpeed;
    [SerializeField][Range(1,10)] private float driftAnimationTilt = 5;
    private float wheelRotation;
    private float rotateWheel;
    private float wheelTurnAngle;

    //collider
    private SphereCollider frontRightCollider;
    private SphereCollider backRightCollider;
    private SphereCollider frontLeftCollider;
    private SphereCollider backLeftCollider;
    //base transform

    private Vector3 frontRightBaseTransform;
    private Vector3 backRightBaseTransform;
    private Vector3 frontLeftBaseTransform;
    private Vector3 backLeftBaseTransform;
    private float turnAmount;


    void Awake()
    {
        frontRightCollider = frontRight.GetComponent<SphereCollider>();
        backRightCollider = backRight.GetComponent<SphereCollider>();
        frontLeftCollider = frontLeft.GetComponent<SphereCollider>();
        backLeftCollider = backLeft.GetComponent<SphereCollider>();

        frontRightBaseTransform = frontRight.transform.localPosition;
        backRightBaseTransform = backRight.transform.localPosition;
        frontLeftBaseTransform = frontLeft.transform.localPosition;
        backLeftBaseTransform = backLeft.transform.localPosition;


    }
    void Update()
    {
        
        GetInputs();
    }
    void FixedUpdate()
    {
        AdjustCarRotation();
        CheckGrounded();
        DriftCheck();
        if (isGrounded)
        {
            GetCurrentForce();
            Turn();
            Acceleration();
        }
        currentSpeed = rb.velocity.magnitude;
    }
    #region Checks
    void CheckGrounded()
    {
        // Raycast from the car downwards
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer);

        // Optional: Debug ray for visualization in the editor
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }
    #endregion
    #region Input
    public virtual void GetInputs()
    {
        verticalInput = InputManager.Movement.y;
        turnInput = InputManager.Movement.x;
        if (InputManager.hasPressedDriftButton)
        {
            isDrifting = true;
        } 
        else if (InputManager.hasReleasedDriftButton) 
        {
            isDrifting = false;

        }
    }
    #endregion

    #region Movement
    private void DriftCheck()
    {
        if (isDrifting)
        {
            turnStrength = driftTurnStrength;
            rb.drag = driftDrag;
            rb.angularDrag = 0.1f;
            startEmmiter();

        }
        else
        {
            turnStrength = accelTurnStrength;
            rb.drag = accelDrag;
            rb.angularDrag = 10f;
            stopEmmiter();
        }
    }


    private void Acceleration()
    {
        if (verticalInput > 0)
        {
            speedInput = verticalInput *Accel * 1000f;
            rb.AddForce((forwardDirection.normalized + transform.forward)/2 * speedInput);
        }  
        else if (verticalInput < 0 && isReversing)
        {
            speedInput = verticalInput *reverseAccel * 1000f;
            rb.AddForce((forwardDirection.normalized + transform.forward)/2 * speedInput);
            
        } 
        else if (verticalInput < 0 && isDrifting)
        {
            speedInput = verticalInput *reverseAccel * 200f;
            rb.AddForce((forwardDirection.normalized + transform.forward)/2 * speedInput);
            
        } 
 
    }


    private void GetCurrentForce()
    {
      
        if (currentAccel > 0 && Vector3.Dot(rb.velocity,transform.forward) > 0)
        {
            if (rb.velocity.magnitude < velocityMatrices[currentAccel].maxSpeed && rb.velocity.magnitude > velocityMatrices[currentAccel-1].force )
            {

                timeAtMaxVelocity = 0;
                Accel = velocityMatrices[currentAccel].force;
            }
            else if (rb.velocity.magnitude > velocityMatrices[currentAccel].maxSpeed)
            {
                timeAtMaxVelocity += Time.deltaTime;
                if (timeAtMaxVelocity > timeThreshold)
                {
                    currentAccel++;
                }
            }
            else if (rb.velocity.magnitude < velocityMatrices[currentAccel-1].maxSpeed)
            {
                currentAccel--;
            }
        }
        else if (currentAccel == 0)
        {
            if (rb.velocity.magnitude > velocityMatrices[currentAccel].maxSpeed && Vector3.Dot(rb.velocity,transform.forward) > 0)
            {
                timeAtMaxVelocity += Time.deltaTime;
                if (timeAtMaxVelocity > timeThreshold)
                {

                    currentAccel++;
                }
            }
            else if (rb.velocity.magnitude < velocityMatrices[currentAccel].maxSpeed)
            {

                timeAtMaxVelocity = 0;
                Accel = velocityMatrices[currentAccel].force;
                if (verticalInput < 0 )
                {
                    timeToReverse += Time.deltaTime;
                    if (timeToReverse > timeReverseThreshold)
                    {
                        isReversing = true;
                    }
                }
                else
                {
                    isReversing = false;
                    timeToReverse = 0;
                }

            }
            
        }
    }

    public virtual void Turn()
    {
        if (rb.velocity.magnitude>0.5f )
        {
            float turningCurve = Math.Abs(Vector3.Dot(rb.velocity,transform.forward))*turnStrengthVelocityCheckMultiplier;
            Quaternion turnRotation;
            if (isDrifting && Mathf.Abs(lastTurnInput) > 0.3f)
            {
                if (!isReversing)
                {
                    turnRotation = Quaternion.Euler(0f, (lastTurnInput*0.7f + turnInput*0.5f) * turnStrength * velocityToTurningCurve.Evaluate(turningCurve) * Time.deltaTime, 0f);
                }
                else
                {
                    turnRotation = Quaternion.Euler(0f, -(lastTurnInput*0.7f + turnInput*0.5f) * turnStrength * velocityToTurningCurve.Evaluate(turningCurve) * Time.deltaTime *2, 0f);

                }
            }
            else
            {
                if (!isReversing)
                {
                    turnRotation = Quaternion.Euler(0f, turnInput * turnStrength * velocityToTurningCurve.Evaluate(turningCurve) * Time.deltaTime, 0f);
                    lastTurnInput = turnInput;
                }
                else
                {
                    turnRotation = Quaternion.Euler(0f, -turnInput * turnStrength * velocityToTurningCurve.Evaluate(turningCurve) * Time.deltaTime *2, 0f);
                    lastTurnInput = turnInput;

                }
            }
            rb.MoveRotation(rb.rotation * turnRotation);
        }
        else
        {
            lastTurnInput = 0;
        }

    }
    #endregion

    #region Effects
    private void startEmmiter()
    {
        foreach (TrailRenderer T in TyreMarks)
        {
            T.emitting = true;
        }
    }

    private void stopEmmiter()
    {
        foreach (TrailRenderer T in TyreMarks)
        {
            T.emitting = false;
        }
    }
    #endregion

    #region CarRotation

    private void AdjustCarRotation()
    {
        //calculate location
        Vector3 averageFrontWheelPosition = (frontRight.transform.position + frontLeft.transform.position) / 2f;
        Vector3 averageBackWheelPosition = (backRight.transform.position + backLeft.transform.position) / 2f;
        carBody.transform.position = Vector3.Lerp(carBody.transform.position, (averageFrontWheelPosition + averageBackWheelPosition)/2f, 0.1f);

        Vector3 averageLeftWheelPosition = (frontLeft.transform.position + backLeft.transform.position) / 2f;
        Vector3 averageRightWheelPosition = (frontRight.transform.position + backRight.transform.position) / 2f;
        Vector3 lateralDirection = averageRightWheelPosition - averageLeftWheelPosition;
        forwardDirection = averageFrontWheelPosition - averageBackWheelPosition;
        Vector3 upDirection = Vector3.Cross(forwardDirection.normalized, lateralDirection.normalized);

        //calculate wheel forward and backward rotation
        wheelRotation = Mathf.Lerp(wheelRotation, currentSpeed*3*verticalInput*rotationLerpSpeed, Time.fixedDeltaTime);
        wheelRotation = wheelRotation % 360f;
        rotateWheel += wheelRotation;
        wheelTurnAngle = Mathf.Lerp(wheelTurnAngle, turnInput * 45, Time.fixedDeltaTime * wheelTurnAngleSpeed);


        //apply rotation to the car body
        carBody.transform.rotation = Quaternion.LookRotation(forwardDirection, upDirection);


        //apply rotation to the wheels
        backRight.transform.rotation = Quaternion.Euler(backRight.transform.rotation.x, carBody.transform.rotation.eulerAngles.y, 90 + carBody.transform.rotation.eulerAngles.z);
        backLeft.transform.rotation = Quaternion.Euler(backLeft.transform.rotation.x, carBody.transform.rotation.eulerAngles.y, 90 + carBody.transform.rotation.eulerAngles.z);
        
        frontRight.transform.rotation = Quaternion.Euler(frontRight.transform.rotation.x, carBody.transform.rotation.eulerAngles.y+ wheelTurnAngle, 90 + carBody.transform.rotation.eulerAngles.z);
        frontLeft.transform.rotation = Quaternion.Euler(frontLeft.transform.rotation.x, carBody.transform.rotation.eulerAngles.y+ wheelTurnAngle, 90 + carBody.transform.rotation.eulerAngles.z);

        //calculate wheel forward and backward rotation
        frontLeft.transform.Rotate(Vector3.down,  rotateWheel, Space.Self);
        frontRight.transform.Rotate(Vector3.down, rotateWheel, Space.Self);

        //detect ground and adjust wheel position and rotation
        //front
        if (Physics.Raycast(frontRight.transform.position, Vector3.down, out RaycastHit hitFrontRightUp, wheelCheckCollider, groundLayer) && Vector3.Distance(hitFrontRightUp.point + Vector3.up * frontRightCollider.radius * frontRight.transform.localScale.x, frontRightBaseTransform) > wheelCheckCollider*10 + 1f)
        {
            frontRight.transform.position = hitFrontRightUp.point + Vector3.up * frontRightCollider.radius * frontRight.transform.localScale.x;
        }
        else
        {
            frontRight.transform.localPosition = Vector3.Lerp(frontRight.transform.localPosition,frontRightBaseTransform,0.1f);
        }
        if (Physics.Raycast(frontLeft.transform.position, Vector3.down, out RaycastHit hitFrontLeftUp, wheelCheckCollider, groundLayer) && Vector3.Distance(hitFrontLeftUp.point + Vector3.up * frontLeftCollider.radius * frontLeft.transform.localScale.x, frontLeftBaseTransform) > wheelCheckCollider*10 + 1f)
        {
            frontLeft.transform.position = hitFrontLeftUp.point + Vector3.up * frontLeftCollider.radius * frontLeft.transform.localScale.x;
        }
        else
        {
            frontLeft.transform.localPosition = Vector3.Lerp(frontLeft.transform.localPosition,frontLeftBaseTransform,0.1f);
        }

        //back
        if (Physics.Raycast(backRight.transform.position, Vector3.down, out RaycastHit hitBackRightUp, wheelCheckCollider, groundLayer) && Vector3.Distance(hitBackRightUp.point + Vector3.up * backRightCollider.radius * backRight.transform.localScale.x, backRightBaseTransform) > wheelCheckCollider*10 + 1f)
        {
            backRight.transform.position = hitBackRightUp.point + Vector3.up * backRightCollider.radius * backRight.transform.localScale.x;
            backRight.transform.Rotate(Vector3.down, rotateWheel, Space.Self);

        }
        else
        {
            backRight.transform.localPosition = Vector3.Lerp(backRight.transform.localPosition,backRightBaseTransform,0.1f);
        }

        

        if (Physics.Raycast(backLeft.transform.position, Vector3.down, out RaycastHit hitBackLeftUp, wheelCheckCollider, groundLayer) && Vector3.Distance(hitBackLeftUp.point + Vector3.up * backLeftCollider.radius * backLeft.transform.localScale.x, backLeftBaseTransform) > wheelCheckCollider*10 + 1f)
        {
            backLeft.transform.position = hitBackLeftUp.point + Vector3.up * backLeftCollider.radius * backLeft.transform.localScale.x;
            backLeft.transform.Rotate(Vector3.down, rotateWheel, Space.Self);

        }
        else
        {
            backLeft.transform.localPosition = Vector3.Lerp(backLeft.transform.localPosition,backLeftBaseTransform,0.1f);
        }

        //apply rotation to the carbody for drifting
        if (isDrifting && lastTurnInput > 0.3f)
        {
            turnAmount = driftAnimationTilt;
            carBody.transform.RotateAround(backLeft.transform.position, carBody.transform.forward, turnAmount);
            backLeft.transform.RotateAround(backLeft.transform.position, carBody.transform.forward, turnAmount);
            backRight.transform.RotateAround(backLeft.transform.position, carBody.transform.forward, turnAmount);
            frontLeft.transform.RotateAround(backLeft.transform.position, carBody.transform.forward, turnAmount);
            frontRight.transform.RotateAround(backLeft.transform.position, carBody.transform.forward, turnAmount);

        }
        else if (isDrifting && lastTurnInput < -0.3f)
        {
            turnAmount = -driftAnimationTilt;
            carBody.transform.RotateAround(backRight.transform.position, carBody.transform.forward, turnAmount);
            backLeft.transform.RotateAround(backRight.transform.position, carBody.transform.forward, turnAmount);
            backRight.transform.RotateAround(backRight.transform.position, carBody.transform.forward, turnAmount);
            frontLeft.transform.RotateAround(backRight.transform.position, carBody.transform.forward, turnAmount);
            frontRight.transform.RotateAround(backRight.transform.position, carBody.transform.forward, turnAmount);

        }

        frontRight.transform.localPosition = new Vector3 (frontRightBaseTransform.x, frontRight.transform.localPosition.y, frontRightBaseTransform.z);
        frontLeft.transform.localPosition = new Vector3 (frontLeftBaseTransform.x, frontLeft.transform.localPosition.y, frontLeftBaseTransform.z);
        backRight.transform.localPosition = new Vector3 (backRightBaseTransform.x, backRight.transform.localPosition.y, backRightBaseTransform.z);
        backLeft.transform.localPosition = new Vector3 (backLeftBaseTransform.x, backLeft.transform.localPosition.y, backLeftBaseTransform.z);


        

    }
    #endregion
}

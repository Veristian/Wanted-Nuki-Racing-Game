using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.UI;


public class SpeedoMeter : MonoBehaviour
{
    public float MaxSpeed = 0.0f;
    public float MinSpeedPointerAngle;
    public float MaxSpeedPointerAngle;
    public Text SpeedLabel;
    public RectTransform Pointer;
    private float speed = 0.0f;
    public Text GearLabel;
    private string currentGear = "N";

    private void Update()
    {
        speed = CarController.currentSpeed * 3.6f;

        if (SpeedLabel != null)
        {
            SpeedLabel.text = ((int)speed) + "";
        }
        if (Pointer != null)
        {
            Pointer.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(MinSpeedPointerAngle, MaxSpeedPointerAngle, speed / MaxSpeed));
        }
        if (GearLabel != null)
        {
            GearLabel.text = GetCurrentGear();
        }
    }

    private string GetCurrentGear()
    {
        if (InputManager.Movement.y > 0.1f)
        {
            currentGear = "D";
        }
        else if (InputManager.Movement.y < -0.1f)
        {
            currentGear = "R";
        }
        else if (speed == 0f)
        {
            currentGear = "N";
        }
        return currentGear;
    }
}

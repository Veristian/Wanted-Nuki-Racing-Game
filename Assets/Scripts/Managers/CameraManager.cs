using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject accelCam;
    public GameObject driftCam;

    public CarController carController;
    private bool wasDrifting;

    private void Awake()
    {
        switchCam();
    }
    private void FixedUpdate()
    {
        
        if (wasDrifting != carController.isDrifting)
        {
            switchCam();
        }

        wasDrifting = carController.isDrifting;
    }
    private void switchCam()
    {
        if (carController.isDrifting)
        {
            driftCam.SetActive(true);
            accelCam.SetActive(false);
        }
        else
        {
            accelCam.SetActive(true);
            driftCam.SetActive(false);

        }
    }
    
}

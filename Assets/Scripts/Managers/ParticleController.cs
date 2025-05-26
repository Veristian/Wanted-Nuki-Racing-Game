using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{

    private float speed = 0.0f;
    public ParticleSystem CarSmoke;
    public ParticleSystem CarBoost;
    public ParticleSystem Burning;
    public bool IsDamaged = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        speed = CarController.currentSpeed * 3.6f;
        GetCurrentState();
        GetCurrentCondition();
    }

    private void GetCurrentState()
    {
        if (InputManager.Movement.y > 0.1f)
        {
            CarSmoke.Stop();
            CarBoost.Play();
        }
        else
        {
            CarBoost.Stop();
//CarSmoke.Play();
        }
        
    }

    private void GetCurrentCondition()
    {
        if (IsDamaged)
        { 
            Burning.Play();
        }
        else if (!IsDamaged)
        {
            Burning.Stop();
        }
    }
}

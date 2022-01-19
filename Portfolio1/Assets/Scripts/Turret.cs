using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Turret : MonoBehaviour
{
    [SerializeField] GameObject turret;
    [SerializeField] float rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if(bRotateActive)
        {
            turret.transform.Rotate(Vector3.up, 1 * rotationSpeed * turretRotDir);
        }
    }

    public void OnTankHandling(InputValue value)
    {
        Debug.Log($"Outputing input value {value.Get<Vector2>()}");
    }


    bool bRotateActive = false;
    float turretRotDir = 0.0f;
    public void OnTurretHandling(InputValue value)
    {
        Debug.Log($"Ouputing turret handling value {value.Get<float>()}");
        turretRotDir = value.Get<float>();
        if(Mathf.Abs(turretRotDir) > Mathf.Epsilon)
        {
            bRotateActive = true;          
        }
        else
        {
            turretRotDir = 0.0f;
            bRotateActive = false;
        }
    }
    
}

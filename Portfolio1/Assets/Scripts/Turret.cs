using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Turret : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTankHandling(InputValue value)
    {
        Debug.Log($"Outputing input value {value.Get<Vector2>()}");
    }
}

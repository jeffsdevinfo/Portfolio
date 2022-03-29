using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Plane collided");
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb)
        {
            //rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            //rb.isKinematic = true;            
            collision.rigidbody.velocity = Vector3.zero;
        }
    }
}

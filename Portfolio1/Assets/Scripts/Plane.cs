using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    public delegate void BulletCollidedWithPlane();
    public static event BulletCollidedWithPlane OnBulletCollidedWithPlane;


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Plane collided");
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb)
        {        
            collision.rigidbody.velocity = Vector3.zero;
            OnBulletCollidedWithPlane();
        }
    }
}

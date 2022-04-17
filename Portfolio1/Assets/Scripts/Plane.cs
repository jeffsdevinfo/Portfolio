// Copyright (c) 2022 Jeff Simon
// Distributed under the MIT/X11 software license, see the accompanying
// file license.txt or http://www.opensource.org/licenses/mit-license.php.

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

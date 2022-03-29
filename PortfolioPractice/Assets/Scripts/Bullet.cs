using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet collided");
        //if (collision.gameObject.tag == "Ground")
        //{
        rb.velocity = Vector3.zero;
            Debug.Log("Bullet Position = " + transform.position);
            //StartCoroutine(TimerDestroy());
            rb.velocity = Vector3.zero;
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bullet triggered");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float bulletVelocity = .01f;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        //rb.velocity = new Vector3(rb.velocity.x * bulletVelocity, rb.velocity.y * bulletVelocity, rb.velocity.z * bulletVelocity);
        rb.AddForce(gameObject.transform.forward * bulletVelocity, ForceMode.Force);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }
}

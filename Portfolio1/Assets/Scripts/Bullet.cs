using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float bulletVelocity = 745f; // average tank velocity
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        //rb.velocity = new Vector3(rb.velocity.x * bulletVelocity, rb.velocity.y * bulletVelocity, rb.velocity.z * bulletVelocity);
        rb.AddForce(gameObject.transform.up * bulletVelocity * 100, ForceMode.Force);
        //rb.AddForce()
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
        StartCoroutine(TimerDestroy());
    }

    IEnumerator TimerDestroy()
    {
        yield return new WaitForSeconds(4.0f);        
        Object.Destroy(gameObject);
    }
}

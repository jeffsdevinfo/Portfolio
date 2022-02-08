using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float bulletVelocity = 745f; // average tank velocity
    [SerializeField] Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        //rb.velocity = new Vector3(rb.velocity.x * bulletVelocity, rb.velocity.y * bulletVelocity, rb.velocity.z * bulletVelocity);
        rb.AddForce(gameObject.transform.up * bulletVelocity * 100, ForceMode.Impulse);
        //rb.AddForce()
        camera.transform.SetParent(gameObject.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        gameObject.transform.localScale = new Vector3(10, 10, 10);
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

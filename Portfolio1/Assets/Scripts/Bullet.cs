using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float bulletVelocity = 745f; // average tank velocity given 1020 high, 470 low    
    [SerializeField] GameObject tank;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Float max range = " + float.MaxValue);
        rb = gameObject.GetComponent<Rigidbody>();
        //rb.velocity = new Vector3(rb.velocity.x * bulletVelocity, rb.velocity.y * bulletVelocity, rb.velocity.z * bulletVelocity);
        rb.AddForce(gameObject.transform.up * bulletVelocity * rb.mass, ForceMode.Impulse);
        //rb.AddForce()
        //camera.transform.SetParent(gameObject.transform);
        
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localPosition += new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 10, gameObject.transform.position.z);
        Camera.main.transform.Rotate(Vector3.right, 90);
        Camera.main.transform.SetParent(gameObject.transform);
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
        if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("Bullet Position = " + transform.position);
            StartCoroutine(TimerDestroy());
        }
    }

    IEnumerator TimerDestroy()
    {
        yield return new WaitForSeconds(4.0f);
        transform.DetachChildren();
        Destroy(gameObject);
    }
}

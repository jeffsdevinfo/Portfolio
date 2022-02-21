using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] public float bulletVelocity = 745; // average tank velocity given 1020 high, 470 low
    static public float BulletSpeed = 745;        
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Float max range = " + float.MaxValue);
        rb = gameObject.GetComponent<Rigidbody>();
        //rb.AddForce(gameObject.transform.up * bulletVelocity, ForceMode.Impulse);
        rb.AddForce(PitchData.PitchTransform.transform.forward * bulletVelocity, ForceMode.Impulse);

        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localPosition += new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 10, gameObject.transform.position.z);
        Camera.main.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
        //Camera.main.transform.Rotate(Vector3.right, 90);
        Camera.main.transform.SetParent(gameObject.transform);
        BulletSpeed = bulletVelocity;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //gameObject.transform.localScale = new Vector3(10, 10, 10);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            rb.velocity = Vector3.zero;
            Debug.Log("Bullet Position = " + transform.position);
            //StartCoroutine(TimerDestroy());
            rb.velocity = Vector3.zero;
        }
    }

    IEnumerator TimerDestroy()
    {
        yield return new WaitForSeconds(4.0f);
        transform.DetachChildren();
        Destroy(gameObject);
    }
}

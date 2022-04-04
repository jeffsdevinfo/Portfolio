using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] public float bulletVelocity = 745; // average tank velocity given 1020 high, 470 low
    static public float BulletSpeed = 745;
    [SerializeField] TMPro.TMP_Text timeOfFlightText;
    bool bShouldExecute;
    
    // Start is called before the first frame update

    static float timerVal;
    static bool start, stop, reset;

    void Start()
    {
        Debug.Log("Float max range = " + float.MaxValue);
        rb = gameObject.GetComponent<Rigidbody>();        

        //rb.AddForce(PitchData.PitchTransform.transform.forward * bulletVelocity, ForceMode.Impulse);
        CalculateProjection();
        Camera.main.transform.position = Vector3.zero;

        Camera.main.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 10, gameObject.transform.position.z);
        Camera.main.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);        
        Camera.main.transform.SetParent(gameObject.transform);
        BulletSpeed = bulletVelocity;
        timeOfFlightText.text = "0.0";

        ResetTimer();
        bShouldExecute = true;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ResetTimer()
    {
        start = true;        
        timerVal = 0;
    }

    private void FixedUpdate()
    {
        //gameObject.transform.localScale = new Vector3(10, 10, 10);
        if(start && bShouldExecute)
        {
            timerVal += Time.fixedDeltaTime;
            double d = System.Math.Round(timerVal, 2);
            timeOfFlightText.text = d.ToString();
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            rb.velocity = Vector3.zero;
            Debug.Log("Bullet Position = " + transform.position);
            //StartCoroutine(TimerDestroy());
            rb.velocity = Vector3.zero;
            start = false;
            bShouldExecute = false;
        }
    }

    IEnumerator TimerDestroy()
    {
        yield return new WaitForSeconds(4.0f);
        transform.DetachChildren();
        Camera.main.transform.parent = null;
        Destroy(gameObject);
    }


    void CalculateProjection()
    {
    }

}

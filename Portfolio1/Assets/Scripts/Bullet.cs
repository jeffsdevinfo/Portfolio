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

        rb.AddForce(PitchData.PitchTransform.transform.forward * bulletVelocity, ForceMode.Impulse);
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
        float xAngle = PitchData.PitchTransform.transform.rotation.eulerAngles.x;
        float yAngle = PitchData.PitchTransform.transform.rotation.eulerAngles.y;

        float mass = gameObject.GetComponent<Rigidbody>().mass;
        float mag = gameObject.GetComponent<Rigidbody>().velocity.magnitude;

        float yVelocity = (Bullet.BulletSpeed / mass) * Mathf.Sin(xAngle * (Mathf.PI / 180));
        float xVelocity = (Bullet.BulletSpeed / mass) * Mathf.Cos(xAngle * (Mathf.PI / 180));

        float fallRateA = -4.905f;//-4.9035;
        float vertDisplacementC = TestFire.BulletSpawnPosition.y;//PitchData.PitchTransform.position.y;
        float QuadDivisor = 2 * fallRateA;

        float QuadQuotientFinal = -1 * yVelocity - Mathf.Sqrt(Mathf.Pow(yVelocity, 2) - 4 * fallRateA * vertDisplacementC);//Barrel.BulletSpawnPosition.y);
        float QuadResultFinal = QuadQuotientFinal / QuadDivisor;
        Debug.Log("QuadResultFinal = " + QuadResultFinal);

        float xPosition = xVelocity * QuadResultFinal;
        Debug.Log("Xposition = " + xPosition);
    }

    //public void DrawAiming()
    //{
    //    //float xAngle = 360.0f - PitchData.PitchTransform.transform.rotation.eulerAngles.x;
    //    //float yAngle = 360.0f - PitchData.PitchTransform.transform.rotation.eulerAngles.y;

    //    //float mass = gameObject.GetComponent<Rigidbody>().mass;
    //    //float mag = gameObject.GetComponent<Rigidbody>().velocity.magnitude;

    //    //float yVelocity = (Bullet.BulletSpeed / mass) * Mathf.Sin(xAngle * (Mathf.PI / 180));
    //    //float xVelocity = (Bullet.BulletSpeed / mass) * Mathf.Cos(xAngle * (Mathf.PI / 180));

    //    //float fallRateA = -4.9035f;
    //    //float vertDisplacementC = TestFire.BulletSpawnPosition.y;//PitchData.PitchTransform.position.y;
    //    //float QuadDivisor = 2 * fallRateA;

    //    //float QuadQuotientFinal = -1 * yVelocity - Mathf.Sqrt(Mathf.Pow(yVelocity, 2) - 4 * fallRateA * vertDisplacementC);//Barrel.BulletSpawnPosition.y);
    //    //float QuadResultFinal = QuadQuotientFinal / QuadDivisor;
    //    //Debug.Log("QuadResultFinal = " + QuadResultFinal);

    //    //float xPosition = xVelocity * QuadResultFinal;
    //    //Debug.Log("Xposition = " + xPosition);

    //    //double xAngle = 360.0 - PitchData.PitchTransform.transform.rotation.eulerAngles.x;
    //    //double yAngle = 360.0 - PitchData.PitchTransform.transform.rotation.eulerAngles.y;



    //    //double xAngle = PitchData.PitchTransform.transform.rotation.eulerAngles.x;
    //    //double yAngle = PitchData.PitchTransform.transform.rotation.eulerAngles.y;

    //    //double mass = gameObject.GetComponent<Rigidbody>().mass;
    //    //double mag = gameObject.GetComponent<Rigidbody>().velocity.magnitude;

    //    //double yVelocity = (Bullet.BulletSpeed / mass) * Math.Sin(xAngle * (Math.PI / 180));
    //    //double xVelocity = (Bullet.BulletSpeed / mass) * Math.Cos(xAngle * (Math.PI / 180));

    //    //double fallRateA = -4.905;//-4.9035;
    //    //double vertDisplacementC = TestFire.BulletSpawnPosition.y;//PitchData.PitchTransform.position.y;
    //    //double QuadDivisor = 2 * fallRateA;

    //    //double QuadQuotientFinal = -1 * yVelocity - Math.Sqrt(Math.Pow(yVelocity, 2) - 4 * fallRateA * vertDisplacementC);//Barrel.BulletSpawnPosition.y);
    //    //double QuadResultFinal = QuadQuotientFinal / QuadDivisor;
    //    //Debug.Log("QuadResultFinal = " + QuadResultFinal);

    //    //double xPosition = xVelocity * QuadResultFinal;
    //    //Debug.Log("Xposition = " + xPosition);


    //    float xAngle = PitchData.PitchTransform.transform.rotation.eulerAngles.x;
    //    float yAngle = PitchData.PitchTransform.transform.rotation.eulerAngles.y;

    //    float mass = gameObject.GetComponent<Rigidbody>().mass;
    //    float mag = gameObject.GetComponent<Rigidbody>().velocity.magnitude;

    //    float yVelocity = (Bullet.BulletSpeed / mass) * Mathf.Sin(xAngle * (Mathf.PI / 180));
    //    float xVelocity = (Bullet.BulletSpeed / mass) * Mathf.Cos(xAngle * (Mathf.PI / 180));

    //    float fallRateA = -4.905f;//-4.9035;
    //    float vertDisplacementC = TestFire.BulletSpawnPosition.y;//PitchData.PitchTransform.position.y;
    //    float QuadDivisor = 2 * fallRateA;

    //    float QuadQuotientFinal = -1 * yVelocity - Mathf.Sqrt(Mathf.Pow(yVelocity, 2) - 4 * fallRateA * vertDisplacementC);//Barrel.BulletSpawnPosition.y);
    //    float QuadResultFinal = QuadQuotientFinal / QuadDivisor;
    //    Debug.Log("QuadResultFinal = " + QuadResultFinal);

    //    float xPosition = xVelocity * QuadResultFinal;
    //    Debug.Log("Xposition = " + xPosition);

    //}
}

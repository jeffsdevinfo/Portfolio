﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFire : MonoBehaviour
{
    [SerializeField] GameObject PrefabToFire;
    [SerializeField] LineRenderer lr;
    GameObject firedBullet;
    int totalNumberOfVertices = 100;
    static public Vector3 BulletSpawnPosition = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        BulletSpawnPosition = gameObject.transform.position;
        //lr = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFire()
    {
        Debug.Log("Fire Pressed");
        
        firedBullet = Instantiate(PrefabToFire, PitchData.PitchTransform.position, Quaternion.identity);        
        firedBullet.SetActive(true);
        Bullet.BulletSpeed = firedBullet.GetComponent<Bullet>().bulletVelocity;

        Rigidbody rb = PrefabToFire.GetComponent<Rigidbody>();
        rb.mass = rb.mass + 1;
        //DrawAiming();
    }

    private void FixedUpdate()
    {
        float xAngle = PitchData.PitchTransform.transform.rotation.eulerAngles.x;
        float yAngle = PitchData.PitchTransform.transform.rotation.eulerAngles.y;

        float mass = PrefabToFire.GetComponent<Rigidbody>().mass;
        float mag = PrefabToFire.GetComponent<Rigidbody>().velocity.magnitude;

        float yVelocity = (Bullet.BulletSpeed / mass) * Mathf.Sin(xAngle * (Mathf.PI / 180));
        float xVelocity = (Bullet.BulletSpeed / mass) * Mathf.Cos(xAngle * (Mathf.PI / 180));

        float fallRateA = -4.905f;//-4.9035;
        float vertDisplacementC = TestFire.BulletSpawnPosition.y;//PitchData.PitchTransform.position.y;
        float QuadDivisor = 2 * fallRateA;

        float QuadQuotientFinal = -1 * yVelocity - Mathf.Sqrt(Mathf.Pow(yVelocity, 2) - 4 * fallRateA * vertDisplacementC);//Barrel.BulletSpawnPosition.y);
        float QuadResultFinal = QuadQuotientFinal / QuadDivisor;
        
        float xPosition = xVelocity * QuadResultFinal;

        float curveDivisionIncrements = QuadResultFinal / 100f;

        //int totalNumberOfVertices = numberOfCircles + ((numberOfVerticesPerCircle + 1) * numberOfCircles);
        lr.positionCount = 100;
        Vector3 originalPosition = gameObject.transform.position;
        int lineSegmentIndex = 0;
        lr.SetPosition(lineSegmentIndex, originalPosition);
        for (int i = 1; i < 100; i++)
        {

            //DrawNextLineWithIndex(ref circleIndex, ref currentPosition);
            Vector3 position = new Vector3();
            position.x = originalPosition.x + (i * curveDivisionIncrements * xVelocity);//Quaternion.AngleAxis(Mathf.Rad2Deg * stepAngle, Vector3.up) * position;
            //float totalYDisplacement = originalPosition.y + yVelocity; // max y = totalYDisplacment
            //position.y = originalPosition.y + yVelocity + vertDisplacementC //(i * curveDivisionIncrements * yVelocity);
            float yDisplacement = yVelocity + (i * curveDivisionIncrements * (fallRateA));
            position.y += yDisplacement;
            position.z = originalPosition.z;
            lr.SetPosition(lineSegmentIndex++, position);
        }


        //float xAngle = 360.0f - PitchData.PitchTransform.transform.rotation.eulerAngles.x;
        //float yAngle = 360.0f - PitchData.PitchTransform.transform.rotation.eulerAngles.y;

        //float mass = PrefabToFire.GetComponent<Rigidbody>().mass;
        //float mag = PrefabToFire.GetComponent<Rigidbody>().velocity.magnitude;

        //float fallRateA = -4.9035f;
        //float vertDisplacementC = PitchData.PitchTransform.position.y;
        //float QuadDivisor = 2 * fallRateA;

        //float yVelocity = (Bullet.BulletSpeed / mass) * Mathf.Sin(xAngle * (Mathf.PI / 180));


        //lr.positionCount = totalNumberOfVertices;
        //Vector3 currentPosition = gameObject.transform.position;
        //int index = 0;
        //lr.SetPosition(index, currentPosition);
        ////for (int i = 0; i < numberOfCircles; i++)
        ////{
        ////    DrawNextLineWithIndex(ref circleIndex, ref currentPosition);
        ////}
        //for (int i = 0; i < 100; i++)
        //{                        
        //    //need to calculate the point along the trajectory here


        //}
    }

    //void CalculateProjection()
    //{
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

    //        float xPosition = xVelocity * QuadResultFinal;
    //    Debug.Log("Xposition = " + xPosition);
    //}


    //void DrawProjectile()
    //    {
    //    float xAngle = 360.0f - PitchData.PitchTransform.transform.rotation.eulerAngles.x;
    //    float yAngle = 360.0f - PitchData.PitchTransform.transform.rotation.eulerAngles.y;

    //    float mass = PrefabToFire.GetComponent<Rigidbody>().mass;
    //    float mag = PrefabToFire.GetComponent<Rigidbody>().velocity.magnitude;

    //    float yVelocity = (Bullet.BulletSpeed / mass) * Mathf.Cos(xAngle * (Mathf.PI / 180));
    //    float xVelocity = (Bullet.BulletSpeed / mass) * Mathf.Sin(xAngle * (Mathf.PI / 180));

    //    float fallRateA = -4.9035f;
    //    float vertDisplacementC = PitchData.PitchTransform.position.y;
    //    float QuadDivisor = 2 * fallRateA;

    //    float QuadQuotientFinal = -1 * yVelocity - Mathf.Sqrt(Mathf.Pow(yVelocity, 2) - 4 * fallRateA * Barrel.BulletSpawnPosition.y);
    //    float QuadResultFinal = QuadQuotientFinal / QuadDivisor;
    //    Debug.Log("QuadResultFinal = " + QuadResultFinal);

    //        float xPosition = xVelocity * QuadResultFinal;
    //    Debug.Log("Xposition = " + xPosition);

    //}

}

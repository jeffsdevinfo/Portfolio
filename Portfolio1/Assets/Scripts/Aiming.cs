using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aiming : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] GameObject parent;
    [SerializeField] float InitialVelocity;
    [SerializeField] float Acceleration;

    bool bDrawShotActive = true;
    LineRenderer lr;    
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = parent.GetComponent<LineRenderer>();
        DoSomething();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("The X angle of the bulletspacelocation = " + parent.transform.rotation.eulerAngles.x);
        //Debug.Log("The Y angle of the bulletspacelocation = " + parent.transform.rotation.eulerAngles.y);
        //if (Input.GetKeyDown(KeyCode.T))
        {
            bDrawShotActive = true;
        }        
    }

    private void FixedUpdate()
    {
        if(bDrawShotActive)
        {
            Draw();
        }
    }

    void Draw()
    {
        DrawProjectileLine();
        //DrawSineWave(parent.transform.position, 10, 30);
    }

    void DrawSineWave(Vector3 startPoint, float amplitude, float wavelength)
    {
        float x = 0f;
        float y;
        float k = 2 * Mathf.PI / wavelength;
        lineRenderer.positionCount = 200;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            x += i * 0.001f;
            y = amplitude * Mathf.Sin(k * x);
            lineRenderer.SetPosition(i, new Vector3(x, y, 0) + startPoint);
        }
    }

    void DrawProjectileLine()
    {
        Vector3 startPoint = parent.transform.position;
        Quaternion quat = parent.transform.rotation;

        Vector3 forward = parent.transform.forward;


        //float V0 = InitialVelocity;
        //Vector3 P0, P1;
        //P0 = parent.transform.position;
        //float a = Acceleration;
        //float t = 0.0f;
        //float temp1 = 0.5f * a * t * t;
        //P1 = new Vector3(P0.x + (V0 * t) + temp1, P0.y + (V0 * t) + temp1, P0.z + (V0 * t) + temp1);

    }

    void DoSomething()
    {
        float yVelocity = Bullet.BulletSpeed * Mathf.Cos(Barrel.BarrelAngle * (Mathf.PI / 180));
        float xVelocity = Bullet.BulletSpeed * Mathf.Sin(Barrel.BarrelAngle * (Mathf.PI / 180));

        float fallRateA = -4.9035f;
        float tempYVelocityB = 4.25f;
        float vertDisplacementC = 100.0f;

        float QuadQuotientV1 = -1 * tempYVelocityB + Mathf.Sqrt(Mathf.Pow(tempYVelocityB, 2) - 4 * fallRateA * vertDisplacementC);
        float QuadQuotientV2 = -1 * tempYVelocityB - Mathf.Sqrt(Mathf.Pow(tempYVelocityB, 2) - 4 * fallRateA * vertDisplacementC);
        float QuadDivisor = 2 * fallRateA;

        float QuadResultV1 = QuadQuotientV1 / QuadDivisor;
        Debug.Log("Result = " + QuadResultV1);

        float QuadResultV2 = QuadQuotientV2 / QuadDivisor;
        Debug.Log("Result = " + QuadResultV2);

        float QuadQuotientFinal = -1 * yVelocity - Mathf.Sqrt(Mathf.Pow(yVelocity, 2) - 4 * fallRateA * Barrel.BulletSpawnPosition.y);
        float QuadResultFinal = QuadQuotientFinal / QuadDivisor;
        Debug.Log("QuadResultFinal = " + QuadResultFinal);

        float xPosition = xVelocity * QuadResultFinal;
        Debug.Log("Xposition = " + xPosition);

    }
}

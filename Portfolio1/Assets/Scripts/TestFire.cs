using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFire : MonoBehaviour
{
    [SerializeField] GameObject PrefabToFire;
    [SerializeField] LineRenderer lr;
    GameObject firedBullet;
    // Start is called before the first frame update
    void Start()
    {
        lr = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFire()
    {
        Debug.Log("Fire Pressed");

        Rigidbody rb = PrefabToFire.GetComponent<Rigidbody>();
        rb.mass = rb.mass + 1;
        firedBullet = Instantiate(PrefabToFire, PitchData.PitchTransform.position, Quaternion.identity);
        firedBullet.SetActive(true);
        Bullet.BulletSpeed = firedBullet.GetComponent<Bullet>().bulletVelocity;
        //DrawAiming();
    }

    private void FixedUpdate()
    {
        float xAngle = 360.0f - PitchData.PitchTransform.transform.rotation.eulerAngles.x;
        float yAngle = 360.0f - PitchData.PitchTransform.transform.rotation.eulerAngles.y;

        float mass = PrefabToFire.GetComponent<Rigidbody>().mass;
        float mag = PrefabToFire.GetComponent<Rigidbody>().velocity.magnitude;

        float fallRateA = -4.9035f;
        float vertDisplacementC = PitchData.PitchTransform.position.y;
        float QuadDivisor = 2 * fallRateA;


        float yVelocity = (Bullet.BulletSpeed / mass) * Mathf.Cos(xAngle * (Mathf.PI / 180));
        for (int i = 0; i < 100; i++)
        {
            
            float QuadQuotientFinal = -1 * yVelocity - Mathf.Sqrt(Mathf.Pow(yVelocity, 2) - 4 * fallRateA * Barrel.BulletSpawnPosition.y);
        }
    }


    void DrawProjectile()
        {
        float xAngle = 360.0f - PitchData.PitchTransform.transform.rotation.eulerAngles.x;
        float yAngle = 360.0f - PitchData.PitchTransform.transform.rotation.eulerAngles.y;

        float mass = PrefabToFire.GetComponent<Rigidbody>().mass;
        float mag = PrefabToFire.GetComponent<Rigidbody>().velocity.magnitude;

        float yVelocity = (Bullet.BulletSpeed / mass) * Mathf.Cos(xAngle * (Mathf.PI / 180));
        float xVelocity = (Bullet.BulletSpeed / mass) * Mathf.Sin(xAngle * (Mathf.PI / 180));

        float fallRateA = -4.9035f;
        float vertDisplacementC = PitchData.PitchTransform.position.y;
        float QuadDivisor = 2 * fallRateA;

        float QuadQuotientFinal = -1 * yVelocity - Mathf.Sqrt(Mathf.Pow(yVelocity, 2) - 4 * fallRateA * Barrel.BulletSpawnPosition.y);
        float QuadResultFinal = QuadQuotientFinal / QuadDivisor;
        Debug.Log("QuadResultFinal = " + QuadResultFinal);

            float xPosition = xVelocity * QuadResultFinal;
        Debug.Log("Xposition = " + xPosition);

    }

}

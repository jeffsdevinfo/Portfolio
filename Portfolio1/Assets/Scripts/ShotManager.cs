using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotManager : MonoBehaviour
{
    [SerializeField] GameObject Barrel;
    [SerializeField] GameObject BulletPrefab;
    [SerializeField] GameObject projectileVectorObject;
    [SerializeField] float speed = 10;
    [SerializeField] float mass = 1f;
    [SerializeField] LineRenderer lr;
    [SerializeField] GameObject FinalPositionPrefab;
    GameObject FinalPositionMoveableGameObject;
    [SerializeField, Range(.0001f, 1f)] float redrawRate = .01f;


    [SerializeField, Range(5,1000)] int totalNumberOfVerticies = 10;
    bool bCalculations = true;
    // Start is called before the first frame update
    void Start()
    {
        FinalPositionMoveableGameObject = Instantiate(FinalPositionPrefab, Barrel.transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (bCalculations)
        {
            CalculateDistance(false);
        }
    }

    public void OnFire()
    {
        CalculateDistance(true);

        GameObject bulletSpawned = Instantiate(BulletPrefab, Barrel.transform.position, Quaternion.identity);

        Rigidbody rb = bulletSpawned.GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.isKinematic = false;
        mass = rb.mass;
        Vector3 directionNorm = new Vector3(projectileVectorObject.transform.position.x - Barrel.transform.position.x,
            projectileVectorObject.transform.position.y - Barrel.transform.position.y,
            projectileVectorObject.transform.position.z - Barrel.transform.position.z).normalized;
        directionNorm = directionNorm * speed;
        rb.AddForce(directionNorm, ForceMode.Impulse);//ForceMode.VelocityChange);   
        StartCoroutine(BypassCalculation());
    }

        float time = 0;
    private void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        if (time >= redrawRate)
        {
            DrawProjectileCurve();
            time = 0;
        }
    }

    
    void DrawProjectileCurve()
    {
        lr.positionCount = totalNumberOfVerticies + 1;
        lr.SetPosition(0, Barrel.transform.position);
        float timeStep = projectileFlightDuration / totalNumberOfVerticies;
        for (int i = 1; i <= totalNumberOfVerticies; i++)
        {
            //calculate x position 
            //calculate y position
            //rotate when ready
            float currentTimeStep = i * timeStep;
            //float xPos = (speedWithMassAdjusted * Mathf.Cos(90.0f - Barrel.transform.rotation.eulerAngles.x)) * currentTimeStep;
            float xPos = Barrel.transform.position.x + xVelocity * currentTimeStep;
            //float yPos = (speedWithMassAdjusted * Mathf.Sin(90.0f - Barrel.transform.rotation.eulerAngles.x)) * currentTimeStep - 0.5f * (-9.81f * Mathf.Pow(currentTimeStep, 2));
            float yPos = Barrel.transform.position.y + yVelocity * currentTimeStep - .5f * (9.81f * Mathf.Pow(currentTimeStep, 2));
            float zPos = Barrel.transform.position.z + 0.0f;
            Vector3 tempVec = new Vector3(xPos, yPos, zPos);
            tempVec = Quaternion.Euler(new Vector3(0.0f, Barrel.transform.rotation.eulerAngles.y - 90, 0.0f)) * tempVec;
            lr.SetPosition(i, tempVec);
        }
    }

    float yVelocity, xVelocity, speedWithMassAdjusted, projectileFlightDuration;
    void CalculateDistance(bool bOutput)
    {
        speedWithMassAdjusted = speed / mass;

        yVelocity = (90.0f - Barrel.transform.rotation.eulerAngles.x);
        yVelocity = yVelocity * (Mathf.PI / 180);
        yVelocity = Mathf.Sin(yVelocity);
        yVelocity = speedWithMassAdjusted * yVelocity;

        xVelocity = (90.0f - Barrel.transform.rotation.eulerAngles.x);
        xVelocity = xVelocity * (Mathf.PI / 180);
        xVelocity = Mathf.Cos(xVelocity);
        xVelocity = speedWithMassAdjusted * xVelocity;

        float fallRateA = Physics.gravity.y / 2.0f;// -4.905f;

        float QuadDivisor = 2 * fallRateA;
        float QuadQuotientFinal = -1 * yVelocity - Mathf.Sqrt(Mathf.Pow(yVelocity, 2) - 4 * fallRateA * Barrel.transform.position.y);
        projectileFlightDuration = QuadQuotientFinal / QuadDivisor;


        float xPosition = xVelocity * projectileFlightDuration;
        if (bOutput)
        {
            Debug.Log("Xposition = " + xPosition);
            Debug.Log("QuadResultFinal = " + projectileFlightDuration);
        }

        Vector3 finalShotPosition = new Vector3(xPosition, 0.0f, 0.0f); // use 0.0f for y and z for now until rotation        
        finalShotPosition = Quaternion.Euler(new Vector3(0.0f, Barrel.transform.rotation.eulerAngles.y - 90, 0.0f)) * finalShotPosition;
        FinalPositionMoveableGameObject.transform.position = finalShotPosition;
    }

    IEnumerator BypassCalculation()
    {
        bCalculations = false;
        yield return new WaitForSeconds(projectileFlightDuration);
        bCalculations = true;
    }
}


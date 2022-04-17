// Copyright (c) 2022 Jeff Simon
// Distributed under the MIT/X11 software license, see the accompanying
// file license.txt or http://www.opensource.org/licenses/mit-license.php.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AimManager : MonoBehaviour
{
    [SerializeField] GameObject AimPivot;
    [SerializeField] GameObject ProjectilePrefab;
    [SerializeField] GameObject FinalPositionPrefab;
    GameObject FinalPositionMoveableGameObject;
    GameObject projectileVectorObject;
    
    [SerializeField] float speed = 10;
    float prevSpeed = 0.0f;
    [SerializeField] float mass = 1f;
    float prevMass = 0.0f;
    [SerializeField] LineRenderer lr;    
    
    //[SerializeField, Range(.0001f, 1f)] float ProjetileCurveRedrawRate = .01f;
    [SerializeField, Range(5,1000)] int ProjectileCurveVertices = 10;
    
    // Since we are using a cylinder to represent the barrel inside this demonstration, the default orientation is that the cylinder
    //  is pointing upwards but the the forward is down the Z.  Our calcuations are based off the X Y axis, thus we set the 
    //  XRotationOffset and YRotationOffset to both be 90 degrees (inside Unity editor) so that the calculation is based off XY.
    [SerializeField] float XRotationOffset = 0.0f;
    [SerializeField] float YRotationOffset = 0.0f;
    [SerializeField] float ZRotationOffset = 0.0f;
    [SerializeField] bool OverrideGravity = false;    
    [SerializeField] float Gravity = -9.81f;

    [SerializeField] Vector3 AimVector = Vector3.up;

    bool bCalculations = true;
    
    [SerializeField] float m_XAxisRotation;
    [SerializeField] float m_YAxisRotation;

    [SerializeField] TMPro.TMP_Text timer_text;

    static bool BTimerActive = false;
    float time = 0;
    bool bRedrawFlag = false;

    void Start()
    {
        //CacheAngles();
        m_XAxisRotation = m_YAxisRotation = 0;
        // Object used to show the final projected position
        FinalPositionMoveableGameObject = Instantiate(FinalPositionPrefab, AimPivot.transform.position, Quaternion.identity);

        // Empty game object used for calculating vector projection
        GameObject newObject = new GameObject("ProjectileVectorGO");
        projectileVectorObject = Instantiate(newObject, AimPivot.transform, false);
        projectileVectorObject.transform.localPosition = AimVector;

        // Should we bypass the Unity gravity and use the specified value in the Unity Editor?
        if (!OverrideGravity)
        {
            Gravity = Physics.gravity.y;
        }
        mass = ProjectilePrefab.GetComponent<Rigidbody>().mass;
    }
    
    void Update()
    {
        if (bCalculations)
        {
            // if bCalculation and any rotations have occurred to the aiming pivot recalculate and redraw the projectile curve
            //if (Mathf.Abs(AimPivot.transform.eulerAngles.x - m_XAxisRotation) + Mathf.Abs(AimPivot.transform.eulerAngles.y - m_YAxisRotation) > Mathf.Epsilon)
            if(CheckForVariableChanges())
            {
                CalculateDistance(false);
                CacheVariables();
                bRedrawFlag = true;
            }
        }        
    }

    private void OnEnable()
    {
        Plane.OnBulletCollidedWithPlane += OnBulletCollidedWithPlane;
    }

    private void OnDisable()
    {
        Plane.OnBulletCollidedWithPlane -= OnBulletCollidedWithPlane;
    }

    void OnBulletCollidedWithPlane()
    {
        // stop timer when bullet collides with plane
        BTimerActive = false;
    }

    bool CheckForVariableChanges()
    {
        if (Mathf.Abs(AimPivot.transform.eulerAngles.x - m_XAxisRotation) + Mathf.Abs(AimPivot.transform.eulerAngles.y - m_YAxisRotation) > Mathf.Epsilon)
        {
            return true;
        }
        if (mass != prevMass)
        {
            return true;
        }
        if (speed != prevSpeed)
        {
            return true;
        }
        return false;
    }


    void CacheVariables()
    {
        m_XAxisRotation = AimPivot.transform.eulerAngles.x;
        m_YAxisRotation = AimPivot.transform.eulerAngles.y;
        prevSpeed = speed;
        prevMass = mass;        
    }

    public void OnFire()
    {
        // Call CalculateDistance and have it output the shot location since we are actually firing now
        CalculateDistance(true);

        // Spawn a projectile at the AimPivot position
        GameObject bulletSpawned = Instantiate(ProjectilePrefab, AimPivot.transform.position, Quaternion.identity);
        
        Rigidbody rb = bulletSpawned.GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.isKinematic = false;
        mass = rb.mass;
        Vector3 directionNorm = new Vector3(
            projectileVectorObject.transform.position.x - AimPivot.transform.position.x,
            projectileVectorObject.transform.position.y - AimPivot.transform.position.y,
            projectileVectorObject.transform.position.z - AimPivot.transform.position.z).normalized;
        Vector3 force = directionNorm * speed; // apply the speed to the direction to get the force
        
        // Apply the force to the bullet's rigidbody
        rb.AddForce(force, ForceMode.Impulse);

        // Bypass the calculation update while the projectile is moving
        StartCoroutine(BypassCalculation());
        time = 0;                   // restart the shot timer
        BTimerActive = true;        // set the timer to active
    }

    
    private void FixedUpdate()
    {
        // if timer is active update timer text field with duration
        if (BTimerActive)
        {
            time += Time.fixedDeltaTime;
            timer_text.text = time.ToString() + " seconds";
        }
        
        // if bRedrawFlag is true redraw the projectile curve
        if (bRedrawFlag)
        {
            bRedrawFlag = false;
            DrawProjectileCurve();
            time = 0;
        }

        #region OldRedrawPatternDisabled
        //if (time >= ProjetileCurveRedrawRate)
        //{
        //    // only draw 
        //    if (bRedrawFlag)
        //    {
        //        bRedrawFlag = false;
        //        DrawProjectileCurve();
        //        time = 0;                
        //    }
        //}
        #endregion OldRedrawPatternDisabled

    }


    void DrawProjectileCurve()
    {
        // setup line renderer
        lr.positionCount = ProjectileCurveVertices + 1;
        lr.SetPosition(0, AimPivot.transform.position);

        // divide the time of flight by the amount of verticies in the projectile curve
        float timeStep = projectileFlightDuration / ProjectileCurveVertices;
        for (int i = 1; i <= ProjectileCurveVertices; i++)
        {
            // calcuate each step along the curve for the set amount of points
            float currentTimeStep = i * timeStep;            
            float xPos = AimPivot.transform.position.x + xVelocity * currentTimeStep;            
            float yPos = AimPivot.transform.position.y + yVelocity * currentTimeStep - .5f * (Mathf.Abs(Gravity) * Mathf.Pow(currentTimeStep, 2));
            float zPos = AimPivot.transform.position.z;
            Vector3 tempVec = new Vector3(xPos, yPos, zPos);
            
            // rotate the point
            tempVec = Quaternion.Euler(new Vector3(0.0f, AimPivot.transform.rotation.eulerAngles.y - YRotationOffset, 0.0f)) * tempVec;            
            lr.SetPosition(i, tempVec);
        }
    }

    float yVelocity, xVelocity, speedWithMassAdjusted, projectileFlightDuration;
    void CalculateDistance(bool bOutput)
    {
        // offset the speed with the mass
        speedWithMassAdjusted = speed / mass;

        // calcuate the Aim direction 
        float AimingAngleDegs = (XRotationOffset - AimPivot.transform.rotation.eulerAngles.x);
        float AimingAngleRads = AimingAngleDegs * (Mathf.PI / 180); // convert to rads
                
        yVelocity = speedWithMassAdjusted * Mathf.Sin(AimingAngleRads); // get force in yAxis (vertical)
        xVelocity = speedWithMassAdjusted * Mathf.Cos(AimingAngleRads); // get applied force in xAxis (horizontal)

        float fallRateA = Gravity / 2.0f;       
        float QuadDivisor = 2 * fallRateA;
        // quadratic equation to calcuate time of flight
        float QuadQuotient = -1 * yVelocity - Mathf.Sqrt(Mathf.Pow(yVelocity, 2) - 4 * fallRateA * AimPivot.transform.position.y);
        projectileFlightDuration = QuadQuotient / QuadDivisor;

        // apply flight time to xVelocity to get horizontal displacement
        float xPosition = xVelocity * projectileFlightDuration;

        // rotate calculated horizontal displacement to AimPivot rotations

        float xFinalAngle = AimPivot.transform.rotation.eulerAngles.x - XRotationOffset;
        float yFinalAngle = AimPivot.transform.rotation.eulerAngles.y - YRotationOffset;
        Vector3 finalShotPosition = new Vector3(xPosition, 0.0f, 0.0f);     
        finalShotPosition = Quaternion.Euler(new Vector3(
            AimPivot.transform.rotation.eulerAngles.x - XRotationOffset, 
            AimPivot.transform.rotation.eulerAngles.y - YRotationOffset, 
            AimPivot.transform.rotation.eulerAngles.z - ZRotationOffset)) * finalShotPosition;

        // set FinalPositionMoveableGameObject to calculated and rotated position
        FinalPositionMoveableGameObject.transform.position = finalShotPosition;

        // output the position during a fire operation
        if (bOutput)
        {
            string tempOutput = "---" + "\nCalculated final shot position = " + finalShotPosition + "\nCalculated flight duration = " + projectileFlightDuration;            
            Debug.Log(tempOutput);            
        }
    }

    IEnumerator BypassCalculation()
    {
        bCalculations = false;
        yield return new WaitForSeconds(projectileFlightDuration);
        bCalculations = true;
    }
}


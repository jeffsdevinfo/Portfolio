using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Barrel : MonoBehaviour
{
    [SerializeField] GameObject barrel;
    [SerializeField] GameObject barrelPivot;
    [SerializeField] float pitchSpeed;
    [SerializeField] GameObject BulletPrefab;
    [SerializeField] GameObject BulletSpawnLocation;
    static public Vector3 BulletSpawnPosition;
    static public float BarrelAngle = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        BulletSpawnPosition = BulletSpawnLocation.transform.position;
        BarrelAngle = 360.0f - barrelPivot.transform.rotation.eulerAngles.x;
    }

    private void FixedUpdate()
    {
        if(bPitchActive)
        {
            BarrelAngle = 360.0f - barrelPivot.transform.rotation.eulerAngles.x;
            Debug.Log("Barrel vertical angle = " + BarrelAngle);
            barrelPivot.transform.Rotate(Vector3.right, 1 * pitchSpeed * barrelRotRate);
        }
    }

    // Update is called once per frame
    void Update()
    {
        BulletSpawnPosition = BulletSpawnLocation.transform.position;
    }

    bool bPitchActive = false;
    float barrelRotRate = 0.0f;
    public void OnBarrelHandling(InputValue value)
    {
        //Debug.Log($"Ouputing barrel handling value {value.Get<float>()}");
        barrelRotRate = value.Get<float>();
        if (Mathf.Abs(barrelRotRate) > Mathf.Epsilon)
        {
            bPitchActive = true;
        }
        else
        {
            barrelRotRate = 0.0f;
            bPitchActive = false;
        }
    }

    public void OnFire()
    {
        Debug.Log("Fire Pressed");
        GameObject gm = Instantiate(BulletPrefab, BulletSpawnLocation.transform.position, Quaternion.identity);// BulletSpawnLocation.transform.rotation);

        //Vector3 random = new Vector3(0, 0, 10);
        //GameObject gm2 = Instantiate(BulletPrefab, random, BulletSpawnLocation.transform.rotation);
        int i = 0;
        //Rigidbody gmrb = gm.GetComponent<Rigidbody>();
        //gmrb.AddForce(gm.transform.up * 1200);
        //gmrb.velocity = gm.transform.up * 1200;
    }
}

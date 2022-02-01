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
    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        if(bPitchActive)
        {
            barrelPivot.transform.Rotate(Vector3.right, 1 * pitchSpeed * barrelRotRate);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    bool bPitchActive = false;
    float barrelRotRate = 0.0f;
    public void OnBarrelHandling(InputValue value)
    {
        Debug.Log($"Ouputing barrel handling value {value.Get<float>()}");
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
        GameObject gm = Instantiate(BulletPrefab, BulletSpawnLocation.transform.position, BulletSpawnLocation.transform.rotation);
        //Rigidbody gmrb = gm.GetComponent<Rigidbody>();
        //gmrb.AddForce(gm.transform.up * 1200);
        //gmrb.velocity = gm.transform.up * 1200;
    }
}

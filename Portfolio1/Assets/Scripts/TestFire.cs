using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFire : MonoBehaviour
{
    [SerializeField] GameObject PrefabToFire;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFire()
    {
        Debug.Log("Fire Pressed");
        GameObject gm = Instantiate(PrefabToFire, PitchData.PitchTransform.position, Quaternion.identity);
    }
}

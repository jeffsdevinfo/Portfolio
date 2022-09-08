using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldManaager : MonoBehaviour
{
    [SerializeField] GameObject[] worldTiles = new GameObject[25];
    [SerializeField] GameObject TilePrefab;
    [SerializeField] float tileWidth = 256;
    [SerializeField] float tileLength = 256;    

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 1; i < 25; i++)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

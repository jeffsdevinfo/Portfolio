using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int gameTileIndex;

    public void UnloadTile()
    {
        Destroy(gameObject);
    }
}

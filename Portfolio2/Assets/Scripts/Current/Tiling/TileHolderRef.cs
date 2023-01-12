// Copyright (c) 2022 Jeff Simon
// Distributed under the MIT/X11 software license, see the accompanying
// file license.txt or http://www.opensource.org/licenses/mit-license.php.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHolderRef : MonoBehaviour
{
    public uint gameTileIndex;
    [SerializeField] public GameObject LowerLeft;
    [SerializeField] public GameObject LowerRight;
    [SerializeField] public GameObject UpperRight;
    [SerializeField] public GameObject UpperLeft;

    public void UnloadTile()
    {
        Destroy(gameObject);
    }
}

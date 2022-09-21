using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using V2 = UnityEngine.Vector2;

[System.Serializable]
public class WorldManager : MonoBehaviour
{
    [SerializeField] GameObject[] worldTiles = new GameObject[25];
    [SerializeField] public List<GameObject> TileDefaultPositions = new List<GameObject>();

    List<sbyte[]> TileMoveLookup = new List<sbyte[]>();
    //List<Vector2[]> TileRegions = new List<Vector2[]>();
    List<sbyte[]> TilesToDelete = new List<sbyte[]>();
    
    [SerializeField] GameObject TilePrefab;
    [SerializeField] float tileWidth = 256;
    [SerializeField] float tileLength = 256;

    [SerializeField] GameObject player;

    private void Awake()
    {   //128,256,384,640
        //Generate tile movment lookup table
        /*
         * 24   9   10  11  12
         * 23   8   1   2   13
         * 22   7   0   3   14
         * 21   6   5   4   15
         * 20   19  18  17  16
         */
        #region previousTable
        ////                                   N  NE E  SE S  SW W  NW        // index 0 is not used
        TileMoveLookup.Insert(0, new sbyte[] {0, 1, 2, 3, 4, 5, 6, 7, 8 }); //0                           
        TileMoveLookup.Insert(1, new sbyte[] {0, 10, 11, 2, 3, 0, 7, 8, 9 }); //1
        TileMoveLookup.Insert(2, new sbyte[] {0, 11, 12, 13, 14, 3, 0, 1, 10 }); //2
        TileMoveLookup.Insert(3, new sbyte[] {0, 2, 13, 14, 15, 4, 5, 0, 1 }); //3
        TileMoveLookup.Insert(4, new sbyte[] {0, 3, 14, 15, 16, 17, 18, 5, 0 }); //4
        TileMoveLookup.Insert(5, new sbyte[] {0, 0, 3, 4, 17, 18, 19, 6, 7 }); //5
        TileMoveLookup.Insert(6, new sbyte[] {0, 7, 0, 5, 18, 19, 20, 21, 22 }); //6
        TileMoveLookup.Insert(7, new sbyte[] {0, 8, 1, 0, 5, 6, 21, 22, 23 }); //7
        TileMoveLookup.Insert(8, new sbyte[] {0, 9, 10, 1, 0, 7, 22, 23, 24 }); //8
        TileMoveLookup.Insert(9, new sbyte[] {0, -1, -1, 10, 1, 8, 23, 24, -1 }); //9
        TileMoveLookup.Insert(10, new sbyte[] {0, -1, -1, 11, 2, 1, 8, 9, -1 }); //10
        TileMoveLookup.Insert(11, new sbyte[] {0, -1, -1, 12, 13, 2, 1, 10, -1 }); //11
        TileMoveLookup.Insert(12, new sbyte[] {0, -1, -1, -1, -1, 13, 2, 11, -1 }); //12
        TileMoveLookup.Insert(13, new sbyte[] {0, 12, -1, -1, -1, 14, 3, 2, 11 }); //13
        TileMoveLookup.Insert(14, new sbyte[] {0, 13, -1, -1, -1, 15, 4, 3, 2 }); //14
        TileMoveLookup.Insert(15, new sbyte[] {0, 14, -1, -1, -1, 16, 17, 4, 3 }); //15
        TileMoveLookup.Insert(16, new sbyte[] {0, 15, -1, -1, -1, -1, -1, 17, 4 }); //16
        TileMoveLookup.Insert(17, new sbyte[] {0, 4, 15, 16, -1, -1, -1, 18, 5 }); //17
        TileMoveLookup.Insert(18, new sbyte[] {0, 5, 4, 17, -1, -1, -1, 19, 6 }); //18
        TileMoveLookup.Insert(19, new sbyte[] {0, 6, 5, 18, -1, -1, -1, 20, 21 }); //19
        TileMoveLookup.Insert(20, new sbyte[] {0, 21, 6, 19, -1, -1, -1, -1, -1 }); //20
        TileMoveLookup.Insert(21, new sbyte[] {0, 22, 7, 6, 19, 20, -1, -1, -1 }); //21
        TileMoveLookup.Insert(22, new sbyte[] {0, 23, 8, 7, 6, 21, -1, -1, -1 }); //22
        TileMoveLookup.Insert(23, new sbyte[] {0, 24, 9, 8, 7, 22, -1, -1, -1 }); //23
        TileMoveLookup.Insert(24, new sbyte[] {0, -1, -1, 9, 8, 23, -1, -1, -1 }); //24

        TilesToDelete.Insert(0, new sbyte[] {}); //0 empty
        TilesToDelete.Insert(1, new sbyte[] {16, 17, 18, 19, 20 });
        TilesToDelete.Insert(2, new sbyte[] {16, 17, 18, 19, 20, 21, 22, 23, 24 });
        TilesToDelete.Insert(3, new sbyte[] {20, 21, 22, 23, 24 });
        TilesToDelete.Insert(4, new sbyte[] {9, 10, 11, 12, 20, 21, 22, 23, 24 });
        TilesToDelete.Insert(5, new sbyte[] {9, 10, 11, 12, 24 });
        TilesToDelete.Insert(6, new sbyte[] {9, 10, 11, 12, 13, 14, 15, 16 });
        TilesToDelete.Insert(7, new sbyte[] {12, 13, 14, 15, 16 });
        TilesToDelete.Insert(8, new sbyte[] {12, 13, 14, 15, 16, 17, 18, 19, 20 });
        #endregion previousTable

        #region newTable
        //                              N   NE  E   SE  S   SW  W   NW          tiles To load / unload
        //TileMoveLookup.Add(new int[] {  5,  6,  7,  8,  1,  2,  3,  4   }); //0                           
        //TileMoveLookup.Add(new int[] {  0,  7,  8,  9,  10, 11, 2,  3   }); //1
        //TileMoveLookup.Add(new int[] {  3,  0,  1,  10, 11, 12, 13, 14  }); //2
        //TileMoveLookup.Add(new int[] {  4,  5,  0,  1,  2,  13, 14, 15  }); //3
        //TileMoveLookup.Add(new int[] {  17, 18, 5,  0,  3,  14, 15, 16  }); //4
        //TileMoveLookup.Add(new int[] {  18, 19, 6,  7,  0,  3,  4,  17  }); //5
        //TileMoveLookup.Add(new int[] {  19, 20, 21, 22, 7,  0,  5,  18  }); //6
        //TileMoveLookup.Add(new int[] {  6,  21, 22, 23, 8,  1,  0,  5   }); //7
        //TileMoveLookup.Add(new int[] {  7,  22, 23, 24, 9,  10, 1,  0   }); //8
        //TileMoveLookup.Add(new int[] {  8,  23, 24, -1, -1, -1, 10, 1   }); //9
        //TileMoveLookup.Add(new int[] {  1,  8,  9,  -1, -1, -1, 11, 2   }); //10
        //TileMoveLookup.Add(new int[] {  2,  1,  10, -1, -1, -1, 12, 13  }); //11
        //TileMoveLookup.Add(new int[] {  13, 2,  11, -1, -1, -1, -1, -1  }); //12
        //TileMoveLookup.Add(new int[] {  14, 3,  2,  11, 12, -1, -1, -1  }); //13
        //TileMoveLookup.Add(new int[] {  15, 4,  3,  2,  13, -1, -1, -1  }); //14
        //TileMoveLookup.Add(new int[] {  16, 17, 4,  3,  14, -1, -1, -1  }); //15
        //TileMoveLookup.Add(new int[] {  -1, -1, 17, 4,  15, -1, -1, -1  }); //16
        //TileMoveLookup.Add(new int[] {  -1, -1, 18, 5,  4,  15, 16, -1  }); //17
        //TileMoveLookup.Add(new int[] {  -1, -1, 19, 6,  5,  4,  17, -1  }); //18
        //TileMoveLookup.Add(new int[] {  -1, -1, 20, 21, 6,  5,  18, -1  }); //19
        //TileMoveLookup.Add(new int[] {  -1, -1, -1, -1, 21, 6,  19  -1  }); //20
        //TileMoveLookup.Add(new int[] {  20, -1, -1, -1, 22, 7,  6,  19  }); //21
        //TileMoveLookup.Add(new int[] {  21, -1, -1, -1, 23, 8,  7,  6   }); //22
        //TileMoveLookup.Add(new int[] {  22, -1, -1, -1, 24, 9,  8,  7   }); //23
        //TileMoveLookup.Add(new int[] {  23, -1, -1, -1, -1, -1, 9,  8   }); //24
        #endregion newTable

        //float halfRegion = tileWidth / 2;
    }
    // Start is called before the first frame update
    void Start()
    {
        for(sbyte i = 1; i < 25; i++)
        {            
            worldTiles[i] = Instantiate(TilePrefab, TileDefaultPositions[i].GetComponent<Tile>().LowerLeft.transform.position, Quaternion.identity);
            worldTiles[i].name = worldTiles[i].name + "-Tile-" + i.ToString();
        }
        StartCoroutine(PositionManageChecker());
    }

    public sbyte WhatTileAmIIn()
    {
        sbyte index = 0;
        Vector3 playerPos = player.transform.position;
        for (sbyte i = 0; i < 25; i++)
        {
            GameObject gameObjTile = TileDefaultPositions[i];
            if (gameObjTile != null)
            {
                Tile tile = gameObjTile.GetComponent<Tile>();
                if (playerPos.x > tile.LowerLeft.transform.position.x)
                    if (playerPos.x < tile.LowerRight.transform.position.x)
                        if (playerPos.z > tile.LowerLeft.transform.position.z)
                            if (playerPos.z < tile.UpperLeft.transform.position.z)
                            {
                                index = i;
                                break;
                            }
            }
        }
        return index;
    }

    //public void TileUpdateRoutine(int index)
    //{
    //    int index = 0;
    //    Vector3 playerPos = player.transform.position;
    //    for(int i = 0; i < 25; i++)
    //    {
    //        GameObject gameObjTile = TileDefaultPositions[i];
    //        if(gameObjTile != null)
    //        {
    //            Tile tile = gameObjTile.GetComponent<Tile>();
    //            if (playerPos.x > tile.LowerLeft.transform.position.x)
    //                if (playerPos.x < tile.LowerRight.transform.position.x)
    //                    if (playerPos.z > tile.LowerLeft.transform.position.z)
    //                        if (playerPos.z < tile.UpperLeft.transform.position.z)
    //                        {
    //                            index = i;
    //                            break;
    //                        }
    //        }
    //    }     
    //    if(index != 0)
    //    {
    //        UpdateTiles(index);
    //    }
    //    //return index;
    //}

    public void UpdateTiles(sbyte index)
    {
        List<GameObject> tilesToDelete = new List<GameObject>();

        for (sbyte i = 0; i < TilesToDelete[index].Length; i++)
        {
            sbyte delIndex = TilesToDelete[index][i];
            Destroy(worldTiles[delIndex]);
        }

        //List<GameObject> tempList = new List<GameObject>();
        GameObject[] tempList = new GameObject[25];
        for (sbyte i = 0; i < 25; i++)
        {
            tempList[i] = worldTiles[i];
        }

        for (sbyte i = 0; i < 25; i++)
        {

            sbyte newIndex = TileMoveLookup[i][index];
            if (newIndex > -1)
                worldTiles[i] = tempList[TileMoveLookup[i][index]];       
            else
                worldTiles[i] = Instantiate(TilePrefab, TileDefaultPositions[i].transform.position, Quaternion.identity);
        }
    }

    IEnumerator PositionManageChecker()
    {
        //while(true)
        //{
            yield return new WaitForSeconds(10);
        //    int index = WhatTileAmIIn();
        //    if(index != 0)
        //    {
        //        UpdateTiles(index);
        //    }
        //}
    }
}

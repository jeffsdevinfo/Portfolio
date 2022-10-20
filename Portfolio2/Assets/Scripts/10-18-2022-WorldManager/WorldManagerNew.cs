using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using V2 = UnityEngine.Vector2;

[System.Serializable]
public class WorldManagerNew : MonoBehaviour
{
    [SerializeField] GameObject[] worldTiles = new GameObject[25];
    [SerializeField] public List<GameObject> TileDefaultPositions = new List<GameObject>();

    List<sbyte[]> TileMoveLookup = new List<sbyte[]>();
    //List<Vector2[]> TileRegions = new List<Vector2[]>();
    List<sbyte[]> TilesToDelete = new List<sbyte[]>();
    Dictionary<int,int> indexLookup = new Dictionary<int, int>();
    
    [SerializeField] GameObject TilePrefab;
    [SerializeField] float tileWidth = 256;
    [SerializeField] float tileLength = 256;

    [SerializeField] GameObject player;

    [SerializeField] GameObject TileHolder;

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


        #region UsedTable
        ////                                      N  NE E  SE S  SW W  NW           // index 0 is not used
        //TileMoveLookup.Insert(0,  new sbyte[] {0, 1, 2, 3, 4, 5, 6, 7, 8 });        //0                           
        //TileMoveLookup.Insert(1,  new sbyte[] {0, 10, 11, 2, 3, 0, 7, 8, 9 });      //1
        //TileMoveLookup.Insert(2,  new sbyte[] {0, 11, 12, 13, 14, 3, 0, 1, 10 });   //2
        //TileMoveLookup.Insert(3,  new sbyte[] {0, 2, 13, 14, 15, 4, 5, 0, 1 });     //3
        //TileMoveLookup.Insert(4,  new sbyte[] {0, 3, 14, 15, 16, 17, 18, 5, 0 });   //4
        //TileMoveLookup.Insert(5,  new sbyte[] {0, 0, 3, 4, 17, 18, 19, 6, 7 });     //5
        //TileMoveLookup.Insert(6,  new sbyte[] {0, 7, 0, 5, 18, 19, 20, 21, 22 });   //6
        //TileMoveLookup.Insert(7,  new sbyte[] {0, 8, 1, 0, 5, 6, 21, 22, 23 });     //7
        //TileMoveLookup.Insert(8,  new sbyte[] {0, 9, 10, 1, 0, 7, 22, 23, 24 });    //8
        //TileMoveLookup.Insert(9,  new sbyte[] {0, -1, -1, 10, 1, 8, 23, 24, -1 });  //9
        //TileMoveLookup.Insert(10, new sbyte[] {0, -1, -1, 11, 2, 1, 8, 9, -1 });    //10
        //TileMoveLookup.Insert(11, new sbyte[] {0, -1, -1, 12, 13, 2, 1, 10, -1 });  //11
        //TileMoveLookup.Insert(12, new sbyte[] {0, -1, -1, -1, -1, 13, 2, 11, -1 }); //12
        //TileMoveLookup.Insert(13, new sbyte[] {0, 12, -1, -1, -1, 14, 3, 2, 11 });  //13
        //TileMoveLookup.Insert(14, new sbyte[] {0, 13, -1, -1, -1, 15, 4, 3, 2 });   //14
        //TileMoveLookup.Insert(15, new sbyte[] {0, 14, -1, -1, -1, 16, 17, 4, 3 });  //15
        //TileMoveLookup.Insert(16, new sbyte[] {0, 15, -1, -1, -1, -1, -1, 17, 4 }); //16
        //TileMoveLookup.Insert(17, new sbyte[] {0, 4, 15, 16, -1, -1, -1, 18, 5 });  //17
        //TileMoveLookup.Insert(18, new sbyte[] {0, 5, 4, 17, -1, -1, -1, 19, 6 });   //18
        //TileMoveLookup.Insert(19, new sbyte[] {0, 6, 5, 18, -1, -1, -1, 20, 21 });  //19
        //TileMoveLookup.Insert(20, new sbyte[] {0, 21, 6, 19, -1, -1, -1, -1, -1 }); //20
        //TileMoveLookup.Insert(21, new sbyte[] {0, 22, 7, 6, 19, 20, -1, -1, -1 });  //21
        //TileMoveLookup.Insert(22, new sbyte[] {0, 23, 8, 7, 6, 21, -1, -1, -1 });   //22
        //TileMoveLookup.Insert(23, new sbyte[] {0, 24, 9, 8, 7, 22, -1, -1, -1 });   //23
        //TileMoveLookup.Insert(24, new sbyte[] {0, -1, -1, 9, 8, 23, -1, -1, -1 });  //24

        TilesToDelete.Insert(0, new sbyte[] { });  // empty
        TilesToDelete.Insert(1, new sbyte[] { }); // empty
        TilesToDelete.Insert(2, new sbyte[] { }); // empty
        TilesToDelete.Insert(3, new sbyte[] { }); // empty
        TilesToDelete.Insert(4, new sbyte[] { }); // empty
        TilesToDelete.Insert(5, new sbyte[] { }); // empty
        TilesToDelete.Insert(6, new sbyte[] { 20, 21, 22, 23, 24, 19, 14, 9, 4 });
        TilesToDelete.Insert(7, new sbyte[] { 20, 21, 22, 23, 24 });
        TilesToDelete.Insert(8, new sbyte[] { 0, 5, 10, 15, 20, 21, 22, 23, 24 });
        TilesToDelete.Insert(9, new sbyte[] { });  // empty
        TilesToDelete.Insert(10, new sbyte[] { }); // empty
        TilesToDelete.Insert(11, new sbyte[] { 24, 19, 14, 9, 4 });
        TilesToDelete.Insert(12, new sbyte[] { }); // empty
        TilesToDelete.Insert(13, new sbyte[] { 20, 15, 10, 5, 0 });
        TilesToDelete.Insert(14, new sbyte[] { }); // empty
        TilesToDelete.Insert(15, new sbyte[] { }); // empty
        TilesToDelete.Insert(16, new sbyte[] { 0, 1, 2, 3, 4, 9, 14, 19, 24 });
        TilesToDelete.Insert(17, new sbyte[] {0,1,2,3,4});
        TilesToDelete.Insert(18, new sbyte[] {20,15,10,5,0,1,2,3,4});
        
        #endregion UsedTable

        #region newTable
        //                                          N   NE  E   SE  S   SW  W   NW          
        TileMoveLookup.Insert(0,  new sbyte[] {0,   5,  6,  1,  -1, -1, -1, -1, -1  });     //0                           
        TileMoveLookup.Insert(1,  new sbyte[] {0,   6,  7,  2,  -1, -1, -1, 0,  5   });     //1
        TileMoveLookup.Insert(2,  new sbyte[] {0,   7,  8,  3,  -1, -1, -1, 1,  6   });     //2
        TileMoveLookup.Insert(3,  new sbyte[] {0,   8,  9,  4,  -1, -1, -1, 2,  7   });     //3
        TileMoveLookup.Insert(4,  new sbyte[] {0,   9,  -1, -1, -1, -1, -1, 3,  8   });     //4
        TileMoveLookup.Insert(5,  new sbyte[] {0,   10, 11, 6,  1,  0,  -1, -1, -1  });     //5
        TileMoveLookup.Insert(6,  new sbyte[] {0,   11, 12, 7,  2,  1,  0,  5,  10  });     //6
        TileMoveLookup.Insert(7,  new sbyte[] {0,   12, 13, 8,  3,  2,  1,  6,  11  });     //7
        TileMoveLookup.Insert(8,  new sbyte[] {0,   13, 14, 9,  4,  3,  2,  7,  12  });     //8
        TileMoveLookup.Insert(9,  new sbyte[] {0,   14, -1, -1, -1, 4,  3,  8,  13  });     //9
        TileMoveLookup.Insert(10, new sbyte[] {0,   15, 16, 11, 6,  5,  -1, -1, -1  });     //10
        TileMoveLookup.Insert(11, new sbyte[] {0,   16, 17, 12, 7,  6,  5,  10, 15  });     //11
        TileMoveLookup.Insert(12, new sbyte[] {0,   17, 18, 13, 8,  7,  6,  11, 16  });     //12
        TileMoveLookup.Insert(13, new sbyte[] {0,   18, 19, 14, 9,  8,  7,  12, 17  });     //13
        TileMoveLookup.Insert(14, new sbyte[] {0,   19, -1, -1, -1, 9,  8,  13, 18  });     //14
        TileMoveLookup.Insert(15, new sbyte[] {0,   20, 21, 16, 11, 10, -1, -1, -1  });     //15
        TileMoveLookup.Insert(16, new sbyte[] {0,   21, 22, 17, 12, 11, 10, 15, 20  });     //16
        TileMoveLookup.Insert(17, new sbyte[] {0,   22, 23, 18, 13, 12, 11, 16, 21  });     //17
        TileMoveLookup.Insert(18, new sbyte[] {0,   23, 24, 19, 14, 13, 12, 17, 22  });     //18
        TileMoveLookup.Insert(19, new sbyte[] {0,   24, -1, -1, -1, 14, 13, 18, 23  });     //19
        TileMoveLookup.Insert(20, new sbyte[] {0,   -1, -1, 21, 16, 15, -1, -1, -1  });     //20
        TileMoveLookup.Insert(21, new sbyte[] {0,   -1, -1, 22, 17, 16, 15, 20, -1  });     //21
        TileMoveLookup.Insert(22, new sbyte[] {0,   -1, -1, 23, 18, 17, 16, 21, -1  });     //22
        TileMoveLookup.Insert(23, new sbyte[] {0,   -1, -1, 24, 19, 18, 17, 22, -1  });     //23
        TileMoveLookup.Insert(24, new sbyte[] {0,   -1, -1, -1, -1, 19, 18, 23, -1  });     //24
        #endregion newTable

        //float halfRegion = tileWidth / 2;

        indexLookup.Add(17, 1);
        indexLookup.Add(18, 2);
        indexLookup.Add(13, 3);
        indexLookup.Add(8,  4);
        indexLookup.Add(7,  5);
        indexLookup.Add(6,  6);
        indexLookup.Add(11, 7);
        indexLookup.Add(16, 8);

    }
    // Start is called before the first frame update
    void Start()
    {
        for(sbyte i = 0; i < 25; i++)
        {
            if (i == 12)
                continue;
            worldTiles[i] = Instantiate(TilePrefab, TileDefaultPositions[i].GetComponent<TileHolderRef>().LowerLeft.transform.position, Quaternion.identity);
            worldTiles[i].name = worldTiles[i].name + "-TileHolderRef-" + i.ToString();
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
                TileHolderRef tile = gameObjTile.GetComponent<TileHolderRef>();
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

        int actualLookup = indexLookup[index];
        for (sbyte i = 0; i < 25; i++)
        {

            sbyte newIndex = TileMoveLookup[i][actualLookup];
            if (newIndex > -1)
                worldTiles[i] = tempList[TileMoveLookup[i][actualLookup]];
            else
            {
                Vector3 temp = new Vector3(-128, 0, -128);
                temp = temp + TileDefaultPositions[i].transform.position;
                //worldTiles[i] = Instantiate(TilePrefab, temp, Quaternion.identity, TileHolder.transform);
                worldTiles[i] = Instantiate(TilePrefab, temp, Quaternion.identity);
            }
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using V2 = UnityEngine.Vector2;

[System.Serializable]
public class WorldManagerNew : MonoBehaviour
{
    [SerializeField] GameObject[] worldTiles = new GameObject[25];
    [SerializeField] public List<GameObject> TileDefaultPositions = new List<GameObject>();

    [SerializeField] GameObject WorldTileSceneContainer;
    [SerializeField] GameObject WorldTilePrefab;
    [SerializeField] GameObject player;
    [SerializeField] int universeRowTileCount = 46340; // squareroot(int max)
    

    [SerializeField] int startingTilePosition = 0;

    int activePlaceHolderCenter;
    List<sbyte[]> TileMoveLookup = new List<sbyte[]>();
    List<sbyte[]> TilesToDelete = new List<sbyte[]>();
    Dictionary<int, int> indexLookup = new Dictionary<int, int>();

    public enum Direction {N, NE, E, SE, S, SW, W, NW, None };
    Direction lastDirection = Direction.None;

    private void Awake()
    {   //128,256,384,640
        //Generate tile movment lookup table
        /*
         * 20   21  22  23  24
         * 15   16  17  18  19
         * 10   11  12  13  14
         *  5   6   7   8   9
         *  0   1   2   3   4
         */
        #region LookupTables

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

        indexLookup.Add(17, 1);
        indexLookup.Add(18, 2);
        indexLookup.Add(13, 3);
        indexLookup.Add(8, 4);
        indexLookup.Add(7, 5);
        indexLookup.Add(6, 6);
        indexLookup.Add(11, 7);
        indexLookup.Add(16, 8);
        #endregion LookupTables

    }
    // Start is called before the first frame update
    void Start()
    {
        #region old Initialization
        //for(sbyte i = 0; i < 25; i++)
        //{
        //    //skipping first tile for now
        //    if (i == 12)
        //        continue;
        //    if (startingTilePosition > -1 && startingTilePosition < Int32.MaxValue)
        //    {
        //        //lookup tile
        //        // if exist, load it and its data objects
        //        // else Instantiate one
        //        if(DBAccess.GetTile(startingTilePosition))
        //        { }

        //    }
        //    InStantiateATile(i);
        //}
        #endregion old Initialization
        StartingTableIndexes(startingTilePosition);
        //NonMonoWorldTile nonMonoStartTile1 = null;
        //if (DBAccess.GetTile(startingTilePosition, ref nonMonoStartTile1))        
        //{
        //    // build off the starting position for the rest of the tiles
        //    //worldTiles[12] = startTile.gameObject;            
        //    InstantiateATile(12, ref nonMonoStartTile1);

            for (int i = 0; i < 25; i++)
            {
            CreateTileWrapperStarter(i);
            }
        //}
        //else
        //{
        //    // no starting index in db so just generate the tiles
        //    for (sbyte i = 0; i < 25; i++)
        //    {
        //        InstantiateATile(i);                
        //    }
        //}
        StartCoroutine(PositionManageChecker());
    }

    public void CreateTileWrapperStarter(int placeHolderIndex)
    {
        NonMonoWorldTile nonMonoInputTil = null;
        if (DBAccess.GetTile(startingIndexes[placeHolderIndex], ref nonMonoInputTil))
        {
            if (nonMonoInputTil != null)
            {
                InstantiateATile(placeHolderIndex, ref nonMonoInputTil);
            }
            else
            { InstantiateATile(placeHolderIndex, startingIndexes[placeHolderIndex]); }
        }
    }

    public void CreateTileWrapper(int placeHolderIndex, int tileTableIndex)
    {
        NonMonoWorldTile nonMonoInputTil = null;
        if (DBAccess.GetTile(tileTableIndex, ref nonMonoInputTil))
        {
            if (nonMonoInputTil != null)
            {
                InstantiateATile(placeHolderIndex, ref nonMonoInputTil);
            }
            else
            { InstantiateATile(placeHolderIndex, tileTableIndex); }
        }
    }

    void InstantiateATile(int index, int tileTableIndex)
    {        
        worldTiles[index] = Instantiate(WorldTilePrefab, TileDefaultPositions[index].GetComponent<TileHolderRef>().LowerLeft.transform.position, 
            Quaternion.identity, WorldTileSceneContainer.transform);
        worldTiles[index].name = worldTiles[index].name + "-TileHolderRef-" + index.ToString();
        //worldTiles[index].GetComponent<TerrainGenerator>().ManualGenerateTerrain();
        worldTiles[index].GetComponent<WorldTile>().terrainGenRef.ManualGenerateTerrain();
        WorldTile wt = worldTiles[index].GetComponent<WorldTile>();
        wt.DatabaseTileIndex = tileTableIndex;
    }

    void InstantiateATile(int index, WorldTile inputTile)
    {
        worldTiles[index] = Instantiate(WorldTilePrefab, TileDefaultPositions[index].GetComponent<TileHolderRef>().LowerLeft.transform.position,
            Quaternion.identity, WorldTileSceneContainer.transform);
        worldTiles[index].name = worldTiles[index].name + "-TileHolderRef-" + index.ToString();
        worldTiles[index].GetComponent<TerrainGenerator>().LoadTerrainData(inputTile.terrainGenRef.dbTileTerrain.Heights);
    }

    void InstantiateATile(int index, ref NonMonoWorldTile nonMonoInputTile)
    {
        worldTiles[index] = Instantiate(WorldTilePrefab, TileDefaultPositions[index].GetComponent<TileHolderRef>().LowerLeft.transform.position,
            Quaternion.identity, WorldTileSceneContainer.transform);
        WorldTile wt = worldTiles[index].GetComponent<WorldTile>();
        wt.name = worldTiles[index].name + "-TileHolderRef-" + index.ToString();
        wt.ConfigureWithNonMonoWorldTile(ref nonMonoInputTile);
        //wt.gameObject.GetComponentInChildren<TerrainGenerator>().LoadTerrainData(wt.terrainGenRef.dbTileTerrain.Heights);
        //wt.terrainGenRef.LoadTerrainData(wt.terrainGenRef.dbTileTerrain.Heights);

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

        activePlaceHolderCenter = index;
        //check for edge cases
        if (index == 17) lastDirection = Direction.N;       //north movement = MaxInt - current > RowCount?
        else if (index == 13) lastDirection = Direction.E;  //east movement = Current % RowCount == (RowCount - 1)?
        else if (index == 7) lastDirection = Direction.S;   //south movement = RowCount - Current > 1?
        else if (index == 11) lastDirection = Direction.W;  //west movement = Current % RowCount == 0?
        //  possibly need nw, ne, se, sw possibles        
        return index;
    }

    public void UpdateTiles(sbyte index)
    {
        List<GameObject> tilesToDelete = new List<GameObject>();

        for (sbyte i = 0; i < TilesToDelete[index].Length; i++)
        {
            sbyte delIndex = TilesToDelete[index][i];
            Destroy(worldTiles[delIndex]);
        }
        
        GameObject[] tempList = new GameObject[25];
        for (sbyte i = 0; i < 25; i++)
        {
            tempList[i] = worldTiles[i];
        }

        List<int> indexOfNew = new List<int>();
        int actualLookup = indexLookup[index];
        for (sbyte i = 0; i < 25; i++)
        {

            sbyte newIndex = TileMoveLookup[i][actualLookup];
            if (newIndex > -1)
                worldTiles[i] = tempList[TileMoveLookup[i][actualLookup]];
            else
            {
                indexOfNew.Add(i);
                // calculate new tile to load
                //Vector3 temp = new Vector3(-128, 0, -128);
                //temp = temp + TileDefaultPositions[i].transform.position;
                //InstantiateATile(i);
            }
        }

        for(int i = 0; i < indexOfNew.Count; i++)
        {
            //int newTileIndex = MovementGetNewTableIndex()
            CreateTileWrapper(indexOfNew[i], MovementGetNewTableIndex(indexOfNew[i], lastDirection));
        }
        
    }

    int[] startingIndexes = new int[25];
    public void StartingTableIndexes(int startingIndex)
    {
        startingIndexes = Enumerable.Repeat(-1, 25).ToArray();  // initialize values to -1
        startingIndexes[12] = startingTilePosition;
        startingIndexes[11] = GetNewTableIndex(startingIndexes[12], Direction.W);
        startingIndexes[10] = GetNewTableIndex(startingIndexes[11], Direction.W);
        startingIndexes[13] = GetNewTableIndex(startingIndexes[12], Direction.E);
        startingIndexes[14] = GetNewTableIndex(startingIndexes[13], Direction.E);

        StartingTableIndexesMinor(10);
        StartingTableIndexesMinor(11);
        StartingTableIndexesMinor(12);
        StartingTableIndexesMinor(13);
        StartingTableIndexesMinor(14);

        int i = 0;

    }

    public void StartingTableIndexesMinor(int placeHolderIndex)
    {
        startingIndexes[placeHolderIndex + 5] = GetNewTableIndex(startingIndexes[placeHolderIndex],Direction.N);
        startingIndexes[placeHolderIndex + 10] = GetNewTableIndex(startingIndexes[placeHolderIndex+5], Direction.N);
        startingIndexes[placeHolderIndex - 5] = GetNewTableIndex(startingIndexes[placeHolderIndex], Direction.S);
        startingIndexes[placeHolderIndex - 10] = GetNewTableIndex(startingIndexes[placeHolderIndex - 5], Direction.S);
    }

    public int GetNewTableIndex(int currentTileTableIndex, Direction dir)
    {
        int calculatedIndex = currentTileTableIndex;
        if (dir == Direction.N)
        {
            calculatedIndex += universeRowTileCount;

        }
        else if (dir == Direction.E)
        {
            calculatedIndex += 1;
        }
        else if (dir == Direction.S)
        {
            calculatedIndex -= universeRowTileCount;
        }
        else if (dir == Direction.W)
        {
            calculatedIndex -= 1;
        }
        return calculatedIndex; // return the newly calculated tileIndex
    }

    public int MovementGetNewTableIndex(int placeHolderIndex, Direction dir)
    {
        WorldTile wt = null;// = worldTiles[index].GetComponent<WorldTile>();
        int worldPHIndex = -1;
        //int calculatedIndex = currentTileTableIndex;

        if (dir == Direction.N)
        {
            //calculatedIndex += universeRowTileCount;
            worldPHIndex = placeHolderIndex - 5;

        }
        else if (dir == Direction.E)
        {
            worldPHIndex = placeHolderIndex - 1;
            //calculatedIndex += 1;
        }
        else if (dir == Direction.S)
        {
            worldPHIndex = placeHolderIndex +5;
            //calculatedIndex -= universeRowTileCount;
        }
        else if (dir == Direction.W)
        {
            worldPHIndex = placeHolderIndex + 1;
            //calculatedIndex -= 1;
        }

        wt = worldTiles[worldPHIndex].GetComponent<WorldTile>();
        
        return GetNewTableIndex(wt.DatabaseTileIndex,dir); // return the newly calculated tileIndex
    }

    IEnumerator PositionManageChecker()
    {        
        yield return new WaitForSeconds(10);
    }
}

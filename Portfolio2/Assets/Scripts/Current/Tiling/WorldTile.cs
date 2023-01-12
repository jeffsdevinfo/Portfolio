// Copyright (c) 2022 Jeff Simon
// Distributed under the MIT/X11 software license, see the accompanying
// file license.txt or http://www.opensource.org/licenses/mit-license.php.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class WorldTile : MonoBehaviour
{
    [HideInInspector] public int DatabaseRecordId = -1;
    public int DatabaseTileIndex = 0;
    public bool OverwriteExistingDBTile = false;
    public float LoadDistance = 0;

    public TerrainGenerator terrainGenRef;

    public List<DBGameObject> worldDBGameObjects = new List<DBGameObject>();
    
    public void SaveTileOnlyToDatabase()
    {
        // check if tile exist in db
        if(DBAccess.CheckTileExist(DatabaseTileIndex))
        {
            if(!OverwriteExistingDBTile)
            {
                return;
            }
            DBAccess.UpdateTile(this);
            return;
        }
        DBAccess.InsertTile(this);
    }

    public void SaveTileAndChildrenToDatabase()
    {          
        // check if tile exist in db
        bool bTileSave = false;
        if (DBAccess.CheckTileExist(DatabaseTileIndex))
        {
            if (!OverwriteExistingDBTile)
            {
                Debug.Log("Write tile failed. Tile already exists.  Please select Overwirte Existing DB Tile to Update existing tile.");
                return;
            }
            bTileSave = DBAccess.UpdateTile(this);        
        }
        else 
        {
            bTileSave = DBAccess.InsertTile(this);
        }

        //save terrainData
        UpdateDBTileTerrain();
        terrainGenRef.EditorSaveTerrainToDB();

        //loop through children and save
        UpdateDBPropsOnChildren();
        UpdateGameObjectList();
        for (int i = 0; i < worldDBGameObjects.Count; i++)
        {
            worldDBGameObjects[i].GenerateDBProperties();
            worldDBGameObjects[i].SaveDBGameObjectToDB();
        }
    }

    public void UpdateDBPropsOnChildren()
    {
        DBGameObject[] dbGameObjects = GetComponentsInChildren<DBGameObject>();
        for (int i = 0; i < dbGameObjects.Length; i++)
        {
            dbGameObjects[i].GenerateDBProperties();
        }
    }

    public void UpdateDBTileTerrain()
    {
        TerrainGenerator terrainGenRef = GetComponentInChildren<TerrainGenerator>();
        if (terrainGenRef != null)
        {
            terrainGenRef.dbTileTerrain.Heights = Utility.TwoDToOneDArray(terrainGenRef.GetTerrainDataArray()).ToList<float>();
        }
    }

    public void UpdateGameObjectList()
    {
        worldDBGameObjects.Clear();
        DBGameObject[] dbGameObjects = GetComponentsInChildren<DBGameObject>();
        for(int i = 0; i < dbGameObjects.Length; i++)
        {
            if(!worldDBGameObjects.Any(item => item.gameIdGUID == dbGameObjects[i].gameIdGUID))
            {
                worldDBGameObjects.Add(dbGameObjects[i]);
            }
        }
    }

    public void ConfigureWithNonMonoWorldTile(ref NonMonoWorldTile inputNonMonoWorldTile)
    {
        DatabaseRecordId = inputNonMonoWorldTile.DatabaseRecordId;
        DatabaseTileIndex = inputNonMonoWorldTile.DatabaseTileIndex;
        OverwriteExistingDBTile = false;
        LoadDistance = inputNonMonoWorldTile.LoadDistance;       
        terrainGenRef.LoadTerrainData(ref inputNonMonoWorldTile.worldDBTerrain);

        //load tile child objects
        for (int i = 0; i < inputNonMonoWorldTile.worldDBGameObjects.Count; i++)
        {
            NonMonoDBGameObject nmDBObj = inputNonMonoWorldTile.worldDBGameObjects[i];

            string pathToResource = $"Prefabs\\{nmDBObj.prefabName}";
            GameObject goFromResources = Resources.Load(pathToResource) as GameObject;            
            GameObject go = Instantiate(goFromResources,gameObject.transform,false);
            go.transform.localPosition = new Vector3(nmDBObj.x, nmDBObj.y, nmDBObj.z);
        }
    }
}

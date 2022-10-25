using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class WorldTile : MonoBehaviour
{
    public int DatabaseRecordId = -1;
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
        UpdateGameObjectList();
        // check if tile exist in db
        if (DBAccess.CheckTileExist(DatabaseTileIndex))
        {
            if (!OverwriteExistingDBTile)
            {
                return;
            }
            DBAccess.UpdateTileFull(this);
            return;
        }
        DBAccess.InsertTileFull(this);
    }

    public void UpdateGameObjectList()
    {
        DBGameObject[] dbGameObjects = GetComponentsInChildren<DBGameObject>();
        for(int i = 0; i < dbGameObjects.Length; i++)
        {
            if(!worldDBGameObjects.Any(item => item.gameIdGUID == dbGameObjects[i].gameIdGUID))
            {
                worldDBGameObjects.Add(dbGameObjects[i]);
            }
        }

        TerrainGenerator terrainGenRef = GetComponentInChildren<TerrainGenerator>();
        if (terrainGenRef != null)
        {
            terrainGenRef.dbTileTerrain.Heights = Utility.TwoDToOneDArray(terrainGenRef.GetTerrainDataArray()).ToList<float>();
        }
    }

    public void ConfigureWithNonMonoWorldTile(ref NonMonoWorldTile inputNonMonoWorldTile)
    {
        DatabaseRecordId = inputNonMonoWorldTile.DatabaseRecordId;
        DatabaseTileIndex = inputNonMonoWorldTile.DatabaseTileIndex;
        OverwriteExistingDBTile = false;
        LoadDistance = inputNonMonoWorldTile.LoadDistance;
        //terrainGenRef.LoadTerrainData(inputNonMonoWorldTile.worldDBTerrain.Heights);
        terrainGenRef.LoadTerrainData(ref inputNonMonoWorldTile.worldDBTerrain);
    }
}

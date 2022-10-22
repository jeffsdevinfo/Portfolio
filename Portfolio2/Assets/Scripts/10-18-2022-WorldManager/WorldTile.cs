using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldTile : MonoBehaviour
{
    public int DatabaseRecordId = -1;
    public int DatabaseTileIndex = 0;
    public bool OverwriteExistingDBTile = false;
    public float LoadDistance = 0;

    public TerrainGenerator terrainGenRef;

    public List<DBGameObject> worldDBGameObjects = new List<DBGameObject>();
    public DBTerrain dbTileTerrain = new DBTerrain();    
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
        dbTileTerrain.Heights = Utility.TwoDToOneDArray(terrainGenRef.GetTerrainDataArray()).ToList<float>();
    }
}

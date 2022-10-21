using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTile : MonoBehaviour
{
    public int DatabaseRecordNumber = -1;
    public int DatabaseTileNumber = 0;
    public bool OverwriteExistingDBTile = false;
    public float LoadDistance = 0;

    public TerrainGenerator terrainGenRef;

    public List<DBGameObject> worldGameObjects = new List<DBGameObject>();
    public DBTerrain worldTileTerrain = new DBTerrain();    
    public void SaveToDatabase()
    {
        // check if tile exist in db
        if(DBAccess.CheckTileExist(DatabaseTileNumber))
        {
            if(!OverwriteExistingDBTile)
            {
                return;
            }            
        }
        DBAccess.WriteTile(this);        
    }
}

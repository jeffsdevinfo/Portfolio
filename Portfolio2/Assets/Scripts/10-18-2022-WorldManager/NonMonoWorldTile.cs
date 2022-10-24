using System.Collections.Generic;

public class NonMonoWorldTile
{
    public int DatabaseRecordId = -1;
    public int DatabaseTileIndex = 0;
    public bool OverwriteExistingDBTile = false;
    public float LoadDistance = 0;    
    public List<DBGameObject> worldDBGameObjects = new List<DBGameObject>();
    public NonMonoDBTerrain worldDBTerrain = new NonMonoDBTerrain();
}

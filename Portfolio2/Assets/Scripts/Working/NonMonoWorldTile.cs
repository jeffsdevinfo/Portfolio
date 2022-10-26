using System.Collections.Generic;

public class NonMonoWorldTile
{
    public int DatabaseRecordId = -1;
    public int DatabaseTileIndex = 0;
    public bool OverwriteExistingDBTile = false;
    public float LoadDistance = 0;    
    public List<NonMonoDBGameObject> worldDBGameObjects = new List<NonMonoDBGameObject>();
    public NonMonoDBTerrain worldDBTerrain = new NonMonoDBTerrain();
}

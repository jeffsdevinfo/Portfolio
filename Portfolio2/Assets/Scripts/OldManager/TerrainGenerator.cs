using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.PlayerLoop;
using System.Linq;
using TreeEditor;

public class TerrainGenerator : MonoBehaviour
{
    public int depth = 20;
    public int width = 256;
    public int height = 256;
    public float scale = 20;

    public bool sharedRandomPerlinNoise = false;
    //public bool completeRandomPerlinNoise = false;

    //[Range(0.0f, .99f)]
    //public float lowerRandomRange = 0.0f;

    [Range(0.0f, .99f)]
    public float upperRandomRange = .99f;

    bool IsDirty = false;
    bool OverwriteExisting = false;
    [SerializeField] WorldTile worldTileRef;
    public DBTerrain dbTileTerrain;// = new DBTerrain();    
    private void Start()
    {
        //dbTileTerrain = GetComponent<DBTerrain>();
        //Terrain terrain = GetComponent<Terrain>();
        //terrain.terrainData = GenerateTerrain(terrain.terrainData);
        //OutputTerrainData(terrain.terrainData);
    }

    static bool Output = false;
    public static void OutputTerrainData(TerrainData td)
    {
        if (Output == false)
        {
            Output = true;
            for(int i = 0; i < 256; i++)
            {
                for (int j= 0; j < 256; j++)
                {
                    Debug.Log(td.GetHeight(i, j));
                }
            }            
        }
    }

    public float[,] GetTerrainDataArray()
    {
        Terrain terrain = GetComponent<Terrain>();
        return terrain.terrainData.GetHeights(0, 0, width, height);
    }

    public void EditorLoadTerrain()
    {
        EditorLoadTerrain(worldTileRef.DatabaseTileIndex);
    }

    public void EditorLoadTerrain(int tileTableIndex)
    {
        NonMonoWorldTile tempTile = null;

        if(DBAccess.GetTerrain(tileTableIndex, ref tempTile))
        {
            depth = tempTile.worldDBTerrain.depth;
            scale = tempTile.worldDBTerrain.scale;
            LoadTerrainData(ref tempTile.worldDBTerrain);
        }
    }

    public void LoadTerrainData(ref NonMonoDBTerrain nonMonoDBTerrain)
    {
        // section that will need to be optimized
        float[] tempArray = Utility.ListFloatTo1DArray(nonMonoDBTerrain.Heights);
        float[,] convertedArray = Utility.OneDToTwoDArray(tempArray, width, height);
        Terrain terrain = GetComponent<Terrain>();

        //TerrainData newTerrainData = (TerrainData)UnityEngine.Object.Instantiate(terrain.terrainData);
        terrain.terrainData = TerrainDataCloner.Clone(terrain.terrainData);
        terrain.terrainData.heightmapResolution = width + 1;
        terrain.terrainData.size = new Vector3(width, depth, height);
        terrain.terrainData.SetHeights(0, 0, convertedArray);
        //terrain.terrainData = terrain.terrainData;
        //terrain.terrainData.heightmapResolution = height; 


        //        terrain.terrainData.SetHeights(0, 0, convertedArray);
        TerrainCollider tc = terrain.GetComponent<TerrainCollider>();
        tc.terrainData = terrain.terrainData;
    }

    public void LoadTerrainData(List<float> heights)
    {
        // section that will need to be optimized
        float[] tempArray = Utility.ListFloatTo1DArray(heights);
        float[,] convertedArray = Utility.OneDToTwoDArray(tempArray, width, height);
        Terrain terrain = GetComponent<Terrain>();

        //TerrainData newTerrainData = (TerrainData)UnityEngine.Object.Instantiate(terrain.terrainData);
        terrain.terrainData = TerrainDataCloner.Clone(terrain.terrainData);        
        terrain.terrainData.heightmapResolution = width + 1;
        terrain.terrainData.size = new Vector3(width, depth, height);
        terrain.terrainData.SetHeights(0, 0, convertedArray);
        //terrain.terrainData = terrain.terrainData;
        //terrain.terrainData.heightmapResolution = height; 


        //        terrain.terrainData.SetHeights(0, 0, convertedArray);
        TerrainCollider tc = terrain.GetComponent<TerrainCollider>();
        tc.terrainData = terrain.terrainData;
    }

    public void ManualGenerateTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }


    public void EditorGenerateTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();
        //TerrainData tData = new TerrainData();
        //tData.size = new Vector3(width, height, height);
        //terrain.terrainData = tData;
        terrain.terrainData = TerrainDataCloner.Clone(terrain.terrainData);
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    public void EditorSaveTerrainToDB()
    {        
        TerrainGenerator terrainGenRef = GetComponentInChildren<TerrainGenerator>();
        dbTileTerrain.Heights = Utility.TwoDToOneDArray(terrainGenRef.GetTerrainDataArray()).ToList<float>();
        dbTileTerrain.scale = scale;
        dbTileTerrain.depth = depth;
        Terrain terrain = GetComponent<Terrain>();
        if (DBAccess.CheckTerrainExist(worldTileRef.DatabaseTileIndex))
        {
            if (IsDirty && OverwriteExisting)
            {
                DBAccess.UpdateTerrain(worldTileRef.DatabaseTileIndex, dbTileTerrain);
            }
        }

        DBAccess.InsertTerrain(worldTileRef.DatabaseTileIndex, dbTileTerrain);
        IsDirty = false;
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        IsDirty = true;
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);

        terrainData.SetHeights(0, 0, GenerateHeights());
        
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);//some perlin noise value
            }
        }

        return heights; 
    }

    
    float CalculateHeight (int x, int y)
    {
        float tempScale = scale;
        //if(completeRandomPerlinNoise)
        //{
        //    tempScale = UnityEngine.Random.Range(lowerRandomRange, upperRandomRange) * tempScale;
        //}
        if(sharedRandomPerlinNoise)
        {
            tempScale = scale * upperRandomRange;  // only use upper Random Range            
        }

        float xCoord = (float)x / width * tempScale;
        float yCoord = (float)y / height * tempScale;
        float perlinResult = Mathf.PerlinNoise(xCoord, yCoord);
        return perlinResult;
    }

    
    public void WriteTerrain(TerrainData td, int refTile, bool bOverWrite = false)
    {
        //convert terrainData to byte array
        var byteArray = new byte[256 * 256 * 4];        
        var floatArray = td.GetHeights(0, 0, 256, 256);

        Buffer.BlockCopy(floatArray, 0, byteArray, 0, 256* 256);
        //need to write Terrain here

    }
}

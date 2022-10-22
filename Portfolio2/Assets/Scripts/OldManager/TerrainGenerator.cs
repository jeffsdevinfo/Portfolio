using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.PlayerLoop;
using System.Linq;

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

    private void Start()
    {
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

    public void LoadTerrainData(List<float> heights)
    {
        // section that will need to be optimized
        float[] tempArray = Utility.ListFloatTo1DArray(heights);
        float[,] convertedArray = Utility.OneDToTwoDArray(tempArray, width, height);
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData.SetHeights(0, 0, convertedArray);        
    }

    public void ManualGenerateTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }


    public void EditorGenerateTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    public void EditorSaveTerrainToDB()
    {        
        TerrainGenerator terrainGenRef = GetComponentInChildren<TerrainGenerator>();
        worldTileRef.dbTileTerrain.Heights = Utility.TwoDToOneDArray(terrainGenRef.GetTerrainDataArray()).ToList<float>();

        Terrain terrain = GetComponent<Terrain>();
        if (DBAccess.CheckTerrainExist(worldTileRef.DatabaseTileIndex))
        {
            if (IsDirty && OverwriteExisting)
            {
                DBAccess.UpdateTerrain(worldTileRef.DatabaseTileIndex, worldTileRef.dbTileTerrain);
            }
        }

        DBAccess.InsertTerrain(worldTileRef.DatabaseTileIndex, worldTileRef.dbTileTerrain);
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
        
        return Mathf.PerlinNoise(xCoord, yCoord);
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

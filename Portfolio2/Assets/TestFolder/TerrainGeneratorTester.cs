using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneratorTester : MonoBehaviour
{
    public int depth = 20;
    public int width = 256;
    public int height = 256;

    public float scale = 20;

    private void Start()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
        //OutputTerrainData(terrain.terrainData);
    }

    static bool Output = false;
    public static void OutputTerrainData(TerrainData td)
    {
        if (Output == false)
        {
            Output = true;
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    Debug.Log(td.GetHeight(i, j));
                }
            }

        }
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);

        terrainData.SetHeights(0, 0, GenerateHeights());

        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);//some perlin noise value
            }
        }

        return heights;
    }


    float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / width * scale;
        float yCoord = (float)y / height * scale;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }


    //SAVE DATA
    public void WriteTerrain(TerrainData td, int refTile, bool bOverWrite = false)
    {
        //convert terrainData to byte array
        var byteArray = new byte[256 * 256 * 4];
        var floatArray = td.GetHeights(0, 0, 256, 256);

        Buffer.BlockCopy(floatArray, 0, byteArray, 0, 256 * 256);
        //need to write Terrain here

    }

}

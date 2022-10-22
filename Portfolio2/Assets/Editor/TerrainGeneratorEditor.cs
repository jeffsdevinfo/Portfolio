using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TerrainGenerator tg = (TerrainGenerator)target;
        //tg.lowerRandomRange = EditorGUILayout.FloatField(new GUIContent("Lower Rand Range","Not used unless completeRandomPerlinNoise is true"), tg.lowerRandomRange);
        //tg.upperRandomRange = EditorGUILayout.FloatField(new GUIContent("Upper Rand Range", "Used exclusively when sharedRandomPerlinNoise is true"), tg.upperRandomRange);
        if (GUILayout.Button("Generate Terrain"))
        {
            //Debug.Log("Pressed Generate Terrain");
            tg.EditorGenerateTerrain();
        }

        if (GUILayout.Button("Save terrain to DB"))
        {
            //Debug.Log("We pressed Generate Terrain");
            tg.EditorSaveTerrainToDB();
        }
    }
}



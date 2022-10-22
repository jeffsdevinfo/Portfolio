using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldTile))]

public class WorldTileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WorldTile tg = (WorldTile)target;
        //tg.lowerRandomRange = EditorGUILayout.FloatField(new GUIContent("Lower Rand Range","Not used unless completeRandomPerlinNoise is true"), tg.lowerRandomRange);
        //tg.upperRandomRange = EditorGUILayout.FloatField(new GUIContent("Upper Rand Range", "Used exclusively when sharedRandomPerlinNoise is true"), tg.upperRandomRange);
        if (GUILayout.Button("Save Tile to Database"))
        {
            Debug.Log("Save Tile To DB");
            tg.SaveTileOnlyToDatabase();
        }

        if (GUILayout.Button("Save Entire Tile to Database"))
        {
            Debug.Log("Save Tile To DB");
            tg.SaveTileAndChildrenToDatabase();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DBGameObject))]

public class DBGameObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DBGameObject tg = (DBGameObject)target;
        
        if (GUILayout.Button("Initiate DB Properties"))
        {
            //Debug.Log("Initiate DB Properties pressed");
            tg.GenerateDBProperties();
        }

        if (GUILayout.Button("Save DB Object To Database"))
        {
            //Debug.Log("Saveed DB Object To DB");
            tg.SaveDBGameObjectToDB();
        }
    }
}

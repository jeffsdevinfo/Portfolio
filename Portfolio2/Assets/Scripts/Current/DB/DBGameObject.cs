// Copyright (c) 2022 Jeff Simon
// Distributed under the MIT/X11 software license, see the accompanying
// file license.txt or http://www.opensource.org/licenses/mit-license.php.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using System;

public class DBGameObject : MonoBehaviour
{
    public int worldTileIndex;    
    //public int worldTileRecordRowId;
    public string gameIdGUID = "";
    public string prefabName;    
    public float x, y, z;
    public bool OverwriteExisting = false;
    public bool forceOverwrite = false;
    //cache states
    private string gameIdGUIdPreserve = "";
    private string prefabPreserve = "";
    //private int worldTileRecordRowIdPreserve;
    private int worldTileIndexPreserve;

    private void Reset()
    {        
        //restored cached values
        prefabName = prefabPreserve;
        gameIdGUID = gameIdGUIdPreserve;
        worldTileIndex = worldTileIndexPreserve;
        //worldTileRecordRowId = worldTileRecordRowIdPreserve;
    }

    public void GenerateDBProperties()
    {
        GameObject prefabGo = PrefabUtility.GetCorrespondingObjectFromOriginalSource<GameObject>(gameObject);
        prefabName = prefabGo.name;
        if (gameIdGUID == "")
        {
            gameIdGUID = GUID.Generate().ToString();            
        }

        Transform rootTrans = gameObject.transform.root;
        WorldTile wt = rootTrans.gameObject.GetComponent<WorldTile>();
        if(wt != null)
        {
            //get root tile index
            worldTileIndex = wt.DatabaseTileIndex;
        }

        CacheStates();
    }

    private void CacheStates()
    {
        //worldTileRecordRowIdPreserve = worldTileRecordRowId;
        worldTileIndexPreserve = worldTileIndex;
        gameIdGUIdPreserve = gameIdGUID;
        prefabPreserve = prefabName;
        x = gameObject.transform.position.x;
        y = gameObject.transform.position.y;
        z = gameObject.transform.position.z;
    }

    public void SaveDBGameObjectToDB()
    {        
        if(DBAccess.CheckDBGameObjectExist(gameIdGUID))
        {
            if (OverwriteExisting && CheckIsDirty() || forceOverwrite)
            {
                //update 
                DBAccess.UpdatetObject(this);
            }
            return;
        }

        DBAccess.InsertObject(this);
    }

    private bool CheckIsDirty()
    {
        if((worldTileIndexPreserve != worldTileIndex) ||
            //(worldTileRecordRowIdPreserve != worldTileRecordRowId) ||
            (gameIdGUIdPreserve != gameIdGUID) ||
            (prefabName != prefabPreserve))
        {
            return true;
        }
        return false;
    }
}
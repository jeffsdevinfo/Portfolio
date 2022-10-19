using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class DBAccess : MonoBehaviour
{    
    // Start is called before the first frame update
    void Start()
    {
        TileTableQuery();
    }

    void TileTableQuery()
    {
        string conn = "URI=file:" + Application.dataPath + "/TileDB.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        //string sqlQuery = "SELECT value,name, randomSequence " + "FROM PlaceSequence";
        string sqlQuery = "SELECT id,xcol,ycol,loadDistance FROM Tiles";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader;
        try
        {
            reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                //int value = reader.GetInt32(0);
                //string name = reader.GetString(1);
                //int rand = reader.GetInt32(2);

                //Debug.Log("value= " + value + "  name =" + name + "  random =" + rand);

                int id = reader.GetInt32(0);
                int xcol = reader.GetInt32(1);
                int ycol = reader.GetInt32(2);
                int loadDistance = reader.GetInt32(3);

                Debug.Log("id= " + id + "| xcol =" + xcol + "| ycol =" + ycol + "| loadDistance=" + loadDistance);
                
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            string code = ex.Message.ToString();
            Debug.Log("SQL Exception {" + code + "}");
        }
        
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    void ObjectTableQuery()
    {
        string conn = "URI=file:" + Application.dataPath + "/TileDB.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        //string sqlQuery = "SELECT value,name, randomSequence " + "FROM PlaceSequence";
        string sqlQuery = "SELECT id,tileId,prefabName,x,y,z" + "FROM Objects";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            //int value = reader.GetInt32(0);
            //string name = reader.GetString(1);
            //int rand = reader.GetInt32(2);

            //Debug.Log("value= " + value + "  name =" + name + "  random =" + rand);

            int id = reader.GetInt32(0);
            int tileId = reader.GetInt32(1);
            string prefabName = reader.GetString(2);
            float x = reader.GetFloat(3);
            float y = reader.GetFloat(4);
            float z = reader.GetFloat(5);

            Debug.Log("id= " + id + "| tileId =" + tileId + "| prefabName =" + prefabName + "| x=" + x + "| y=" + y + "| z=" + z);

        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

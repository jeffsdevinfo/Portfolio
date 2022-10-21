using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
using System.Linq;
using UnityEngine.UIElements;

public class DBAccess : MonoBehaviour
{
    [SerializeField] static string dbFileName = "URI=file:" + Application.dataPath + "/TileDB.db"; //Path to database.
    static IDbConnection dbconn;

    static DBAccess()
    {
        Debug.Log("DBAccess static constructor called");
        dbconn = (IDbConnection)new SqliteConnection(dbFileName);
        dbconn.Open(); //Open connection to the database.
        EditorPlayMode.PlayModeChanged += OnPlayModeChanged;
    }

    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit OnDestroy called");
        if (dbconn != null)
        {
            if (dbconn != null && dbconn.State == ConnectionState.Open)
            {
                dbconn.Close();
                dbconn = null;
            }
        }
    }

    private static void OnPlayModeChanged(PlayModeState currentMode, PlayModeState changedMode)
    {
        if (changedMode == PlayModeState.Stopped)
        {
            Debug.Log("OnPlayModeChanged Stopped called");
            if (dbconn != null)
            {
                if (dbconn != null && dbconn.State == ConnectionState.Open)
                {
                    dbconn.Close();
                    dbconn = null;
                }
            }            
        }
    }
   
    void Start()
    {
        //TileTableQuery();        
        
    }

    private void OnDestroy()
    {
        Debug.Log("DBAccess OnDestroy called");
        dbconn.Close();
        dbconn = null;
    }

    public static void TileTableQuery()
    {
        
        //IDbConnection dbconn;
        //dbconn = (IDbConnection)new SqliteConnection(conn);
        //dbconn.Open(); //Open connection to the database.
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
        //string conn = "URI=file:" + Application.dataPath + "/TileDB.db"; //Path to database.
        //IDbConnection dbconn;
        //dbconn = (IDbConnection)new SqliteConnection(conn);
        //dbconn.Open(); //Open connection to the database.
        //IDbCommand dbcmd = dbconn.CreateCommand();
        ////string sqlQuery = "SELECT value,name, randomSequence " + "FROM PlaceSequence";
        //string sqlQuery = "SELECT id,tileId,prefabName,x,y,z" + "FROM Objects";
        //dbcmd.CommandText = sqlQuery;
        //IDataReader reader = dbcmd.ExecuteReader();
        //while (reader.Read())
        //{
        //    //int value = reader.GetInt32(0);
        //    //string name = reader.GetString(1);
        //    //int rand = reader.GetInt32(2);

        //    //Debug.Log("value= " + value + "  name =" + name + "  random =" + rand);

        //    int id = reader.GetInt32(0);
        //    int tileId = reader.GetInt32(1);
        //    string prefabName = reader.GetString(2);
        //    float x = reader.GetFloat(3);
        //    float y = reader.GetFloat(4);
        //    float z = reader.GetFloat(5);

        //    Debug.Log("id= " + id + "| tileId =" + tileId + "| prefabName =" + prefabName + "| x=" + x + "| y=" + y + "| z=" + z);

        //}
        //reader.Close();
        //reader = null;
        //dbcmd.Dispose();
        //dbcmd = null;
        //dbconn.Close();
        //dbconn = null;
    }

    static public bool ExecuteSQLStatement(string query)
    {
        bool bResult = true;
        IDbCommand dbcmd = dbconn.CreateCommand();                
        dbcmd.CommandText = query;

        try
        {
            int rowsAffected = dbcmd.ExecuteNonQuery();
            Debug.Log($"{rowsAffected} rows were affected");
        }
        catch(Exception ex)
        {
            bResult = false;
        }
        
        dbcmd.Dispose();
        dbcmd = null;
        return bResult;
        //dbconn.Close();
        //dbconn = null;
    }

    static public bool ExecuteSQLStatement(IDbCommand dbcmd)
    {
        bool bResult = true;                
        try
        {
            int rowsAffected = dbcmd.ExecuteNonQuery();
            Debug.Log($"{rowsAffected} rows were affected");
        }
        catch (Exception ex)
        {
            bResult = false;
        }
        return bResult;
    }

    static public void WriteTile(WorldTile wt)
    {
        //write WorldTile to the DB in the Tile table
        

        //dbcmd.CommandText = $"INSERT INTO Tiles (xcol, ycol, tileIndex, loadDistance) VALUES (,,,)";
        string queryToInsertTile = $"INSERT INTO Tiles (xcol, ycol, tileIndex, loadDistance) VALUES (,,,)";
        if(ExecuteSQLStatement(queryToInsertTile)) // if successful writing tile continue with writing terrain and objects
        {
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = "select last_insert_rowid()";            
            IDataReader reader;
            int tileRowId = -1;
            try
            {
                reader = dbcmd.ExecuteReader();
                while (reader.Read())
                {
                    tileRowId = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                string code = ex.Message.ToString();
                Debug.Log("SQL Exception - Unable to fetch tileRowId {" + code + "}");
            }
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            //write WorldTile.worldTileTerrain.Heights to the DB in the Terrain table
            if (tileRowId != -1)
            {
                WriteTerrain(tileRowId, ref wt.worldTileTerrain);
                //Write each worldTile.worldGameObjects objec to the DB in the Objects table
                for(int i = 0; i < wt.worldGameObjects.Count; i++)
                {
                    WriteObject(tileRowId, wt.worldGameObjects[i]);
                }
            }
        }
    }

    static public bool WriteObject(int tileRowId, DBGameObject objectToWrite)
    {      
        IDbCommand dbcmd = dbconn.CreateCommand();
        dbcmd.CommandText = $"INSERT INTO Objects (tileId,prefabName,x,y,z) VALUES ({tileRowId},{objectToWrite.worldTileRecordRowId},{objectToWrite.x},{objectToWrite.y},{objectToWrite.z})";        
        bool bReturn = ExecuteSQLStatement(dbcmd.CommandText);
        dbcmd.Dispose();
        dbcmd = null;

        return bReturn;
    }

    static public bool WriteTerrain(int tileRowId, ref DBTerrain dbTer)
    {        
        var heightArray = dbTer.Heights.ToArray();
        var byteArray = new byte[heightArray.Length * 4];
        Buffer.BlockCopy(heightArray, 0, byteArray, 0, byteArray.Length);

        IDbCommand dbcmd = dbconn.CreateCommand();
        dbcmd.CommandText = $"INSERT INTO Terrain (tileId,heightData) VALUES ({tileRowId},@heightData)";
        var parameter = dbcmd.CreateParameter();
        parameter.ParameterName = "@heightData";
        parameter.DbType = DbType.Binary;
        parameter.Size = heightArray.Length * 4;
        parameter.Value = heightArray;
        dbcmd.Parameters.Add(parameter);                

        bool bReturn = ExecuteSQLStatement(dbcmd.CommandText);

        dbcmd.Dispose();
        dbcmd = null;

        return bReturn;
    }



    static public void GetTileObjects(int tileRecord, ref WorldTile wt)
    {

    }

    static public bool CheckTileExist(int tileWorldIndex)
    {
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = $"SELECT id FROM Tiles WHERE tileIndex = {tileWorldIndex}";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader;
        bool bReturn = true;
        try
        {
            reader = dbcmd.ExecuteReader();
            if (reader.Read()) { bReturn = true; }
            else { bReturn = false; }
            reader.Close();
        }
        catch (Exception ex)
        {
            string code = ex.Message.ToString();
            bReturn = false;
            Debug.Log("SQL Exception {" + code + "}");
        }

        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        return bReturn;
    }

    static public bool GetTile(int tileWorldIndex, WorldTile wt)
    {
        IDbCommand dbcmd = dbconn.CreateCommand();        
        string sqlQuery = $"SELECT id,xcol,ycol,loadDistance FROM Tiles WHERE tileIndex = {tileWorldIndex}";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader;
        bool bReturn = true;
        try
        {
            reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {                                
                wt = new WorldTile();
                wt.DatabaseRecordNumber = reader.GetInt32(0);                
                wt.LoadDistance = reader.GetFloat(3);
                wt.DatabaseTileNumber = reader.GetInt32(4);

            }
            reader.Close();
        }
        catch (Exception ex)
        {
            string code = ex.Message.ToString();
            bReturn = false;
            Debug.Log("SQL Exception {" + code + "}");
        }

        reader = null;
        dbcmd.Dispose();
        dbcmd = null;

        if (wt != null)
        {
            //load terrain and any child objects
            //load terrain
            GetTerrain(wt.DatabaseRecordNumber, ref wt);
            //load child objects
            GetTileObjects(wt.DatabaseRecordNumber, ref wt);
        }

        return bReturn;
    }

    static public void GetTerrain(int tileTableIndex, ref WorldTile wt)
    {
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = $"SELECT heightData FROM Terrain WHERE id = {tileTableIndex}";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader;
        try
        {
            reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                byte[] buffer = GetBytes(reader);
                var heightArray = new float[buffer.Length / 4];
                Buffer.BlockCopy(buffer, 0, heightArray, 0, buffer.Length);
                
                wt.worldTileTerrain.Heights = heightArray.ToList();

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
    }

    static byte[] GetBytes(IDataReader reader)
    {
        const int CHUNK_SIZE = 2 * 1024;
        byte[] buffer = new byte[CHUNK_SIZE];
        long bytesRead;
        long fieldOffset = 0;
        using (MemoryStream stream = new MemoryStream())
        {
            while ((bytesRead = reader.GetBytes(0, fieldOffset, buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, (int)bytesRead);
                fieldOffset += bytesRead;
            }
            return stream.ToArray();
        }
    }


    #region Demo

    //public static void Demo()
    //{
    //    #region Demo

    //    if (File.Exists("test.db3"))
    //    {
    //        File.Delete("test.db3");
    //    }
    //    using (var connection = new SQLiteConnection("Data Source=test.db3;Version=3"))
    //    using (var command = new SQLiteCommand("CREATE TABLE PHOTOS(ID INTEGER PRIMARY KEY AUTOINCREMENT, PHOTO BLOB)", connection))
    //    {
    //        connection.Open();
    //        command.ExecuteNonQuery();

    //        byte[] photo = new byte[] { 1, 2, 3, 4, 5 };

    //        command.CommandText = "INSERT INTO PHOTOS (PHOTO) VALUES (@photo)";
    //        command.Parameters.Add("@photo", DbType.Binary, 20).Value = photo;
    //        command.ExecuteNonQuery();

    //        command.CommandText = "SELECT PHOTO FROM PHOTOS WHERE ID = 1";
    //        using (var reader = command.ExecuteReader())
    //        {
    //            while (reader.Read())
    //            {
    //                byte[] buffer = GetBytes(reader);
    //                for (int i = 0; i < buffer.Length; i++)
    //                {
    //                    Console.WriteLine(buffer[i]);
    //                }
    //            }
    //        }
    //    }
    //}

    //static byte[] GetBytes(SQLiteDataReader reader)
    //{
    //    const int CHUNK_SIZE = 2 * 1024;
    //    byte[] buffer = new byte[CHUNK_SIZE];
    //    long bytesRead;
    //    long fieldOffset = 0;
    //    using (MemoryStream stream = new MemoryStream())
    //    {
    //        while ((bytesRead = reader.GetBytes(0, fieldOffset, buffer, 0, buffer.Length)) > 0)
    //        {
    //            stream.Write(buffer, 0, (int)bytesRead);
    //            fieldOffset += bytesRead;
    //        }
    //        return stream.ToArray();
    //    }
    //}

    #endregion Demo

}

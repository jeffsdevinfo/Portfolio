using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
using System.Linq;
using UnityEngine.UIElements;
using System.Xml.Linq;
using UnityEngine.PlayerLoop;

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
    }

    static public bool ExecuteSQLStatement(IDbCommand dbcmd)
    {
        bool bResult = true;
        int rowsAffected = -1;
        try
        {
            rowsAffected = dbcmd.ExecuteNonQuery();
        
        }
        catch (Exception ex)
        {
            bResult = false;
        }

        Debug.Log($"{rowsAffected} rows were affected");
        return bResult;
    }

    static public bool UpdateTile(WorldTile wt)
    {
        string queryToUpdateTile = $"UPDATE Tiles SET tileIndex = {wt.DatabaseTileIndex}, xcol = {0}, ycol = {0}, loadDistance = {wt.LoadDistance})";
        if (ExecuteSQLStatement(queryToUpdateTile)) // if successful writing tile continue with writing terrain and objects
        {
            Debug.Log("Updated tile");
            return true;
        }
        else
        {
            Debug.Log("Updated tile failed");
            return false;
        }
    }

    static public void UpdateTileFull(WorldTile wt)
    {
        if(UpdateTile(wt))
        {
            for(int i = 0; i < wt.worldDBGameObjects.Count; i++)
            {
                wt.worldDBGameObjects[i].SaveDBGameObjectToDB();
            }
        }
    }

    static public bool InsertTile(WorldTile wt)
    {
        string queryToInsertTile = $"INSERT INTO Tiles (xcol, ycol, tileIndex, loadDistance) VALUES ({0},{0},{wt.DatabaseTileIndex},{wt.LoadDistance})";        
        int tileDBRowId = -1;
        if (ExecuteSQLStatement(queryToInsertTile)) // if successful writing tile continue with writing terrain and objects
        {
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = "select last_insert_rowid()";
            IDataReader reader;            
            try
            {
                reader = dbcmd.ExecuteReader();
                while (reader.Read())
                {
                    tileDBRowId = reader.GetInt32(0);
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
        }

        if(tileDBRowId == -1)
        {
            return false;
        }
        return true;
    }

    static public void InsertTileFull(WorldTile wt)
    {     
        //string queryToInsertTile = $"INSERT INTO Tiles (xcol, ycol, tileIndex, loadDistance) VALUES ({0},{0},{wt.DatabaseTileIndex},{wt.LoadDistance})";
        //if(ExecuteSQLStatement(queryToInsertTile)) // if successful writing tile continue with writing terrain and objects
        //{
        //    IDbCommand dbcmd = dbconn.CreateCommand();
        //    dbcmd.CommandText = "select last_insert_rowid()";            
        //    IDataReader reader;
        //    int tileDBRowId = -1;
        //    try
        //    {
        //        reader = dbcmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            tileDBRowId = reader.GetInt32(0);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string code = ex.Message.ToString();
        //        Debug.Log("SQL Exception - Unable to fetch tileRowId {" + code + "}");
        //    }
        //    reader = null;
        //    dbcmd.Dispose();
        //    dbcmd = null;
            //write WorldTile.worldTileTerrain.Heights to the DB in the Terrain table
        if(InsertTile(wt))
        { 
            //WriteTerrain(wt.DatabaseTileIndex, ref wt.worldTileTerrain);
            ////Write each worldTile.worldGameObjects objec to the DB in the Objects table
            //for(int i = 0; i < wt.worldDBGameObjects.Count; i++)
            //{
            //    InsertObject(wt.worldDBGameObjects[i]);
            //}            
        }
    }

    static public bool InsertObject(DBGameObject objectToWrite)
    {
        bActiveTempConnection = CreateATempConnection();
        IDbCommand dbcmd = dbconn.CreateCommand();
        //dbcmd.CommandText = $"INSERT INTO Objects (tileIndex,prefabName,x,y,z,gameGUID) VALUES ({objectToWrite.worldTileIndex},{objectToWrite.prefabName},{objectToWrite.x},{objectToWrite.y},{objectToWrite.z},{objectToWrite.gameIdGUID})";
        dbcmd.CommandText = String.Format("INSERT INTO Objects (tileIndex,prefabName,x,y,z,gameGUID) VALUES ({0},\"{1}\",{2},{3},{4},\"{5}\")",
            objectToWrite.worldTileIndex,
            objectToWrite.prefabName,
            objectToWrite.x,
            objectToWrite.y,
            objectToWrite.z,
            objectToWrite.gameIdGUID);

        bool bReturn = ExecuteSQLStatement(dbcmd.CommandText);
        dbcmd.Dispose();
        dbcmd = null;

        if (bActiveTempConnection) CloseATempConnection();


        return bReturn;
    }

    static public bool UpdatetObject(DBGameObject objectToWrite)
    {
        bActiveTempConnection = CreateATempConnection();        
        string queryToUpdateTile = String.Format("UPDATE Objects SET tileIndex = {0}, prefabName = \"{1}\", x = {2}, y = {3}, z = {4}, gameGuid = \"{5}\"))",
            objectToWrite.worldTileIndex,
            objectToWrite.prefabName,
            objectToWrite.x,
            objectToWrite.y,
            objectToWrite.z,
            objectToWrite.gameIdGUID);

        if (ExecuteSQLStatement(queryToUpdateTile))
        {
            Debug.Log("Updated gameObject");
            return true;
        }
        else
        {
            Debug.Log("Updated gameObject failed");
            return false;
        }
        if (bActiveTempConnection) CloseATempConnection();
    }


    static public bool InsertTerrain(int tileIndex, DBTerrain dbTer)
    {
        bActiveTempConnection = CreateATempConnection();
        var heightArray = dbTer.Heights.ToArray();
        var byteArray = new byte[heightArray.Length * 4];
        Buffer.BlockCopy(heightArray, 0, byteArray, 0, byteArray.Length);

        IDbCommand dbcmd = dbconn.CreateCommand();
        //dbcmd.CommandText = $"INSERT INTO Terrain (tileIndex,heightData) VALUES ({tileIndex},@heightData)";
        dbcmd.CommandText = String.Format("INSERT INTO Terrain (tileIndex,heightData) VALUES ({0},@heightData)", tileIndex);

        var parameter = dbcmd.CreateParameter();
        //dbcmd.Parameters.Add("@photo", DbType.Binary, 20).Value = photo;

        parameter.ParameterName = "@heightData";        
        parameter.DbType = DbType.Binary;
        parameter.Size = byteArray.Length;//heightArray.Length * 4;
        parameter.Value = byteArray;// heightArray;
        dbcmd.Parameters.Add(parameter);

        bool bReturn = ExecuteSQLStatement(dbcmd);//ExecuteSQLStatement(dbcmd.CommandText);

        dbcmd.Dispose();
        dbcmd = null;
        if (bActiveTempConnection) CloseATempConnection();

        return bReturn;
    }

    static public bool UpdateTerrain(int tileIndex, DBTerrain dbTer)
    {
        bActiveTempConnection = CreateATempConnection();
        var heightArray = dbTer.Heights.ToArray();
        var byteArray = new byte[heightArray.Length * 4];
        Buffer.BlockCopy(heightArray, 0, byteArray, 0, byteArray.Length);

        IDbCommand dbcmd = dbconn.CreateCommand();
        //dbcmd.CommandText = $"INSERT INTO Terrain (tileIndex,heightData) VALUES ({tileIndex},@heightData)";
        dbcmd.CommandText = $"UPDATE Terrain SET tileIndex = {tileIndex}, heightData= @heightData";
        var parameter = dbcmd.CreateParameter();
        parameter.ParameterName = "@heightData";
        parameter.DbType = DbType.Binary;
        parameter.Size = heightArray.Length * 4;
        parameter.Value = heightArray;
        dbcmd.Parameters.Add(parameter);

        bool bReturn = ExecuteSQLStatement(dbcmd.CommandText);

        dbcmd.Dispose();
        dbcmd = null;
        if (bActiveTempConnection) CloseATempConnection();

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

    static public bool CheckDBGameObjectExist(string gameGUID)
    {
        bActiveTempConnection = CreateATempConnection();
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = String.Format("SELECT id FROM Objects WHERE gameGUID = \"{0}\"", gameGUID);
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
        if (bActiveTempConnection) CloseATempConnection();

        return bReturn;
    }

    static public bool CheckTerrainExist(int tileIndex)
    {
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = $"SELECT id FROM Terrain WHERE tileIndex = {tileIndex}";
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
                wt.DatabaseRecordId = reader.GetInt32(0);                
                wt.LoadDistance = reader.GetFloat(3);
                wt.DatabaseTileIndex = reader.GetInt32(4);

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
            GetTerrain(wt.DatabaseRecordId, ref wt);
            //load child objects
            GetTileObjects(wt.DatabaseRecordId, ref wt);
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
                
                wt.dbTileTerrain.Heights = heightArray.ToList();

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

    static bool bActiveTempConnection = false;
    static bool CreateATempConnection()
    {
        if (dbconn == null || dbconn.State != ConnectionState.Open)
        {
            dbconn = (IDbConnection)new SqliteConnection(dbFileName);
            dbconn.Open(); //Open connection to the database.
            if (dbconn != null)
            {
                if (dbconn != null && dbconn.State == ConnectionState.Open)
                {
                    return true;
                }
            }
        }
        return false;
    }

    static void CloseATempConnection()
    {
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

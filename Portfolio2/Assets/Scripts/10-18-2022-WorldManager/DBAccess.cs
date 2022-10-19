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

    static public void ExecuteSQLStatement(string connectionFileName, string query)
    {        
        string conn = "URI=file:" + Application.dataPath + "/" + connectionFileName; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();                
        dbcmd.CommandText = query;

        int rowsAffected = dbcmd.ExecuteNonQuery();

        Debug.Log($"{rowsAffected} rows were affected");

        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    static public void WriteTile()
    { 
        
    }

    static public void WriteObject()
    {

    }
    static public void WriteTerrain()
    {

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

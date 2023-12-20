using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Mono.Data.Sqlite;
using System.Collections;
using System;
using System.IO;
using SimpleFileBrowser;

public class CreateDBScript : MonoBehaviour
{

    public Text DebugText;

    public event Action<UserExperiment> OnUserExperimentRetrieved;
    public const int EXEPERIMENT_1 = 1;
    public const int EXEPERIMENT_2 = 2;
    public const int EXEPERIMENT_3 = 3;


    public IEnumerator CreateTableUserPerformances()
    {
        string conn = SetDataBaseClass.SetDataBase("DataBase.db");

        IDbConnection dbcon;
        IDbCommand dbcmd;
        IDataReader reader;

        dbcon = new SqliteConnection(conn);
        dbcon.Open();

        Debug.Log("CREATE TABLE if not exists ARVRexper");
        dbcmd = dbcon.CreateCommand();
        string SqlQuery = "CREATE TABLE if not exists ARVRexper ( " +
            "Experiment_no INTEGER, " + 
            "timestamp INTEGER," +
            "experiment_name TEXT," +
            "user TEXT," +
            "ar_vr_identifier INTEGER," +
            "experiment_done INTEGER)";
        dbcmd.CommandText = SqlQuery;
        reader = dbcmd.ExecuteReader();

        reader.Close();
        dbcon.Close();
        Debug.Log("CREATE TABLE if not exists ARVRexper");

        yield return null;
    }

    private IEnumerator InsertUserPerformance(UserExperiment userExperiment, bool experiment_done, bool isInserted)
    {
        Debug.Log("InsertUserPerformance");
        if (!isInserted)
        {
            string conn = SetDataBaseClass.SetDataBase("DataBase.db");

            IDbConnection dbcon;
            IDbCommand dbcmd;

            dbcon = new SqliteConnection(conn);
            dbcon.Open();

            dbcmd = dbcon.CreateCommand();

            string ar_vr_identifier = "1";

            DateTime dataHoraAtual = DateTime.Now;

            long timestamp = dataHoraAtual.Ticks;

            Debug.Log("insert into ARVRexper");

            string SqlQuery = "insert into ARVRexper " +
                "(Experiment_no , timestamp, experiment_name, user, ar_vr_identifier, experiment_done) values " +
                "(\"" + userExperiment.experimentNo + "\", \"" + timestamp + "\", \"" + userExperiment.experimentName + "\", \"" + userExperiment.userId + "\", \"" + ar_vr_identifier + "\", " + experiment_done + "); ";
            dbcmd.CommandText = SqlQuery;
            dbcmd.ExecuteReader();

            dbcon.Close();
        }
        

        yield return CopyDatabase(userExperiment);

        //yield return ExportDatabase(userExperiment);


    }

    IEnumerator CopyDatabase(UserExperiment userExperiment)
    {
        Debug.Log("copiar databases");
        string sourcePath = Path.Combine(Application.persistentDataPath, "DataBase.db");
        string targetPath = Path.Combine(Application.persistentDataPath, userExperiment.userId + ".db");
        Debug.Log("sourcePath - " + sourcePath);

        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }

        WWW www = new WWW(sourcePath);
        yield return www;

        Debug.Log("sourcePath www - " + www.text + www.bytes);
        File.WriteAllBytes(targetPath, www.bytes);
        Debug.Log("copiou");

    }

    IEnumerator ExportDatabase(string userId)
    {
        string databasePath = Path.Combine(Application.persistentDataPath, "ARVRResult.db"); // Caminho do banco de dados SQLite
        string exportPath = Application.persistentDataPath; // Pasta de exportação padrão

        // Configure o callback do SimpleFileBrowser
        FileBrowser.SetFilters(true, new FileBrowser.Filter(userId, ".db"));
        FileBrowser.SetDefaultFilter(".db");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

        FileBrowser.ShowSaveDialog(successDialog, null, FileBrowser.PickMode.Files, false, null, "ARVRResult.db", "Create a ARVRResult.db file", "Save");

        yield return null;
    }

    private void successDialog(string[] path)
    {
        if (!string.IsNullOrEmpty(path[0]))
        {
            try
            {
                Debug.Log("caminhos - " + path[0]);

                string databasePath = Path.Combine(Application.persistentDataPath, "DataBase.db");
                FileBrowserHelpers.CopyFile(databasePath, path[0]);
                Debug.Log("Banco de dados exportado com sucesso para: " + path);

            }
            catch (Exception e)
            {
                Debug.LogError("Erro ao exportar o banco de dados: " + e.Message);
            }
        }
    }

    public void completedExeperiment(int currentExperimentNo)
    {
        Debug.Log("completedExeperiment");

        StartCoroutine(getUserPerformance(currentExperimentNo));
    }

    public void downloadDataBase()
    {
        string userId = getUserId();
        StartCoroutine(ExportDatabase(userId));
    }

    private string getUserId()
    {
        string userId = "";
        Debug.Log("getUserId");
        string conn = SetDataBaseClass.SetDataBase("DataBase.db");


        IDbConnection dbcon;
        IDbCommand dbcmd;
        IDataReader reader;

        dbcon = new SqliteConnection(conn);
        dbcon.Open();

        dbcmd = dbcon.CreateCommand();

        string SqlQuery = "select user_id from ExperimentToDo limit 1";
        dbcmd.CommandText = SqlQuery;
        reader = dbcmd.ExecuteReader();

        UserExperiment userExperiment = new UserExperiment(0, "", "");

        Debug.Log("select user_id from ExperimentToDo");
        while (reader.Read())
        {
            userId = reader.GetString(0);
        }
        reader.Close();

        return userId;
    }

    private IEnumerator getUserExperiment(int currenctExperimetNo, bool isInserted)
    {
        Debug.Log("getUserExperiment");
        string conn = SetDataBaseClass.SetDataBase("DataBase.db");


        IDbConnection dbcon;
        IDbCommand dbcmd;
        IDataReader reader;

        dbcon = new SqliteConnection(conn);
        dbcon.Open();

        dbcmd = dbcon.CreateCommand();

        string SqlQuery = "select experiment_no, experiment_name, user_id from ExperimentToDo limit 1";
        dbcmd.CommandText = SqlQuery;
        reader = dbcmd.ExecuteReader();

        UserExperiment userExperiment = new UserExperiment(0, "", "");

        Debug.Log("select experiment_no, experiment_name, user_id from ExperimentToDo");
        int experiment_no = -1;
        while (reader.Read())
        {
            experiment_no = reader.GetInt16(0);
            string experiment_name = reader.GetString(1);
            string userId = reader.GetString(2);
            userExperiment = new UserExperiment(experiment_no, experiment_name, userId);
        }
        reader.Close();
        if (currenctExperimetNo == experiment_no)
        {
            yield return InsertUserPerformance(userExperiment, true, isInserted);
        }
        else
        {
            yield return null;
        }

    }

    private IEnumerator getUserPerformance(int userExperimentNo)
    {
        Debug.Log("getUserPerformance");
        string conn = SetDataBaseClass.SetDataBase("DataBase.db");


        IDbConnection dbcon;
        IDbCommand dbcmd;
        IDataReader reader;

        dbcon = new SqliteConnection(conn);
        dbcon.Open();

        dbcmd = dbcon.CreateCommand();

        string SqlQuery = "select user, timestamp, experiment_done from ARVRexper where Experiment_no = " + userExperimentNo + " and experiment_done = 1";
        dbcmd.CommandText = SqlQuery;
        reader = dbcmd.ExecuteReader();


        Debug.Log("select user, timestamp, experiment_done from ARVRexper");
        var isInserted = false;

        while (reader.Read())
        {
            Debug.Log(reader.GetValue(0) + " - " + reader.GetValue(1) + " - " + reader.GetValue(2));
            isInserted = true;
        }
        reader.Close();
        yield return getUserExperiment(userExperimentNo, isInserted);

    }
}
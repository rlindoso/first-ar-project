using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public Text DebugText;
    // Start is called before the first frame update
    public Button dbButton;

    void Start()
    {
        dbButton.onClick.AddListener(OnButtonClick);
        Coroutine coroutine = StartCoroutine(CopyDatabase());
    }

    public void OnButtonClick()
    {
        Debug.Log("O botão foi clicado!");
        var createDBScript = (CreateDBScript)FindObjectOfType(typeof(CreateDBScript));
        createDBScript.downloadDataBase();
        StartSync();
    }

    // Update is called once per frame
    private void StartSync()
    {
        
    }

    IEnumerator CopyDatabase()
    {
        Debug.Log("copiar databases");
        string sourcePath = Path.Combine(Application.streamingAssetsPath, "DataBase.db");
        string targetPath = Path.Combine(Application.persistentDataPath, "DataBase.db");
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

        var createDBScript = (CreateDBScript)FindObjectOfType(typeof(CreateDBScript));
        //var createDBScript = GetComponent<CreateDBScript>();
        yield return createDBScript.CreateTableUserPerformances();
        createDBScript.completedExeperiment(CreateDBScript.EXEPERIMENT_1);
    }
}

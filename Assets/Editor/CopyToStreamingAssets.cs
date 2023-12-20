//using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class CopyToStreamingAssets : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildReport report)
    {
        //string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string sourcePath = Path.Combine("C:/DLwspNGL/dlwspdb", "ARVRlaunch.db"); // Substitua pelo caminho real do seu arquivo.
        string destPath = Path.Combine(Application.dataPath, "StreamingAssets/DataBase.db"); // Substitua pelo nome desejado no StreamingAssets.

        // Certifique-se de que o diretório de destino exista
        Directory.CreateDirectory(Path.GetDirectoryName(destPath));

        // Copie o arquivo
        File.Copy(sourcePath, destPath, true);

        // Atualize a base de dados de ativos (isso é importante para que o arquivo seja incluído no build)
        AssetDatabase.Refresh();

        Debug.Log("Arquivo copiado para StreamingAssets: " + destPath);
    }
}
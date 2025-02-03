using UnityEngine;
using UnityEditor;
using System.IO;

public class ProjectStructureCreator : EditorWindow
{
    [MenuItem("Tools/Create Project Structure")]
    public static void ShowWindow()
    {
        GetWindow<ProjectStructureCreator>("Project Structure Creator");
    }

    private ProjectStructureConfig config;

    private void OnGUI()
    {
        GUILayout.Label("Project Structure Generator", EditorStyles.boldLabel);

        config = (ProjectStructureConfig)EditorGUILayout.ObjectField("Config File", config, typeof(ProjectStructureConfig), false);

        if (config == null)
        {
            EditorGUILayout.HelpBox("Sélectionnez un fichier de configuration pour générer la structure.", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Create Project Structure"))
        {
            CreateFolders();
        }
    }

    private void CreateFolders()
    {
        string rootPath = Application.dataPath;

        if (config == null || config.folders.Count == 0)
        {
            Debug.LogWarning("⚠️ Aucune structure définie dans le fichier de configuration !");
            return;
        }

        foreach (var folder in config.folders)
        {
            string path = Path.Combine(rootPath, folder);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log($"✅ Dossier créé : {folder}");
            }
        }

        AssetDatabase.Refresh();
    }
}
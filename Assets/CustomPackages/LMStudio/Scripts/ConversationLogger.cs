using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyPckg_IskXR.LMStudio;

public class ConversationLogger : MonoBehaviour
{
    [SerializeField] private string logFilePath = ""; // Défini dans l'Inspector
    [SerializeField] private UnityToLama unityToLama;
    [SerializeField] private TMP_InputField userInputField;
    [SerializeField] private Button sendButton;

    private void Awake()
    {
        // Si le chemin est vide, utiliser un chemin par défaut
        if (string.IsNullOrEmpty(logFilePath))
        {
            logFilePath = Path.Combine(Application.persistentDataPath, "conversation_log.txt");
        }

        // Vérifier si le dossier existe et le créer si nécessaire
        string directoryPath = Path.GetDirectoryName(logFilePath);
        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Ajouter le listener pour le bouton "Envoyer"
        sendButton.onClick.AddListener(OnSendMessage);
    }

    public void LogUserMessage(string message)
    {
        WriteToFile($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - User: {message}");
    }

    private void LogResponse(string response)
    {
        WriteToFile($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Assistant: {response}");
    }

    private void WriteToFile(string logEntry)
    {
        try
        {
            using (FileStream fs = new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine(logEntry);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Erreur lors de l'écriture du log : {e.Message}");
        }
    }

    public void OnSendMessage()
    {
        string userMessage = userInputField.text;
        if (!string.IsNullOrEmpty(userMessage))
        {
            LogUserMessage(userMessage);
            userInputField.text = "";
        }
    }

    private void OnEnable()
    {
        if (unityToLama != null)
        {
            unityToLama.OnResponseReceived.AddListener(LogResponse);
        }
    }

    private void OnDisable()
    {
        if (unityToLama != null)
        {
            unityToLama.OnResponseReceived.RemoveListener(LogResponse);
        }
    }
}

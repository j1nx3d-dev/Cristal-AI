using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;  // Toujours nécessaire pour Button
using TMPro;          // Pour TextMeshPro
using MyPckg_IskXR.LMStudio;

public class ConversationLogger : MonoBehaviour
{
    /// <summary>
    /// Logs the user's input message to a text file (called when 'Send' is pressed).
    /// </summary>
    public void LogUserMessage(string message)
    {
        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - User: {message}\n";
        File.AppendAllText(logFilePath, logEntry);
    }

    /// <summary>
    /// This function will be called when the user presses the 'Send' button.
    /// </summary>
    public void OnSendMessage()
    {
        string userMessage = userInputField.text;
        if (!string.IsNullOrEmpty(userMessage))
        {
            LogUserMessage(userMessage); // Enregistrer le message de l'utilisateur
            userInputField.text = "";     // Réinitialiser le champ de texte
        }
    }

    [SerializeField] private UnityToLama unityToLama;  // Assure-toi que tu as bien UnityToLama configuré
    [SerializeField] private TMP_InputField userInputField; // Utilisation de TMP_InputField au lieu de InputField
    [SerializeField] private Button sendButton;         // Le bouton "Envoyer"

    private string logFilePath;

    private void Awake()
    {
        // Initialisation du chemin de log
        logFilePath = Path.Combine(Application.persistentDataPath, "conversation_log.txt");

        // Ajouter un listener pour le bouton "Envoyer"
        sendButton.onClick.AddListener(OnSendMessage);
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

    /// <summary>
    /// Logs the assistant's response to a text file.
    /// </summary>
    private void LogResponse(string response)
    {
        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Assistant: {response}\n";
        File.AppendAllText(logFilePath, logEntry);
    }
}
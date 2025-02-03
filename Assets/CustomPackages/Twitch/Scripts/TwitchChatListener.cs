using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

/// <summary>
/// Connects to Twitch chat via IRC and listens to messages in real-time.
/// </summary>
public class TwitchChatListener : MonoBehaviour
{
    #region Twitch Credentials

#if UNITY_EDITOR

    [Header("Twitch Credentials")]
    [SerializeField, HideInInspector] private string oauthToken = "oauth:xxxxxxxxxxxxxxxxxxxxxxxxxxx";  // OAuth Token for Twitch bot

    [SerializeField, HideInInspector] private string botUsername = "TonBotUsername";  // Bot username linked to OAuth token
    [SerializeField, HideInInspector] private string channelName = "NomDuStream";  // Streamer's channel name (without #)
    [SerializeField, HideInInspector] private string clientID = "VotreClientID"; // Optional, based on use
#endif

    public string OauthToken { get => oauthToken; set => oauthToken = value; }
    public string BotUsername { get => botUsername; set => botUsername = value; }
    public string ChannelName { get => channelName; set => channelName = value; }
    public string ClientID { get => clientID; set => clientID = value; }

    #endregion Twitch Credentials

    #region Private Variables

    private TcpClient twitchClient;
    private StreamReader reader;
    private StreamWriter writer;
    private Thread listenThread;
    private bool isListening;

    #endregion Private Variables

    #region Unity Methods

    /// <summary>
    /// Initializes the connection when the object is created.
    /// </summary>
    private void Start()
    {
        // ConnectToTwitch(); // Optionally, uncomment to connect on start
    }

    /// <summary>
    /// Cleans up resources when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        DisconnectFromTwitch();
    }

    #endregion Unity Methods

    #region Twitch Connection

    /// <summary>
    /// Connects to Twitch IRC chat.
    /// </summary>
    public void ConnectToTwitch()
    {
        try
        {
            twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
            reader = new StreamReader(twitchClient.GetStream());
            writer = new StreamWriter(twitchClient.GetStream());

            writer.WriteLine($"PASS {oauthToken}");
            writer.WriteLine($"NICK {botUsername}");
            writer.WriteLine($"JOIN #{channelName}");
            writer.Flush();

            isListening = true;
            listenThread = new Thread(ListenToChat) { IsBackground = true };
            listenThread.Start();

            Debug.Log("✅ Connecté au chat Twitch !");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Erreur de connexion : {ex.Message}");
        }
    }

    /// <summary>
    /// Disconnects from Twitch IRC chat and releases resources.
    /// </summary>
    private void DisconnectFromTwitch()
    {
        isListening = false;

        // Ensure thread is properly stopped and resources are freed
        listenThread?.Interrupt(); // Preferably use interrupt over abort
        reader?.Close();
        writer?.Close();
        twitchClient?.Close();

        Debug.Log("🔌 Déconnecté du chat Twitch.");
    }

    #endregion Twitch Connection

    #region Chat Listening

    /// <summary>
    /// Listens to incoming messages from Twitch chat and responds to PING requests.
    /// </summary>
    private void ListenToChat()
    {
        try
        {
            while (isListening)
            {
                if (twitchClient.Available > 0)
                {
                    string message = reader.ReadLine();
                    if (!string.IsNullOrEmpty(message))
                    {
                        Debug.Log($"📩 Message reçu: {message}");

                        // Respond to PINGs from Twitch to prevent disconnection
                        if (message.StartsWith("PING"))
                        {
                            writer.WriteLine("PONG :tmi.twitch.tv");
                            writer.Flush();
                        }
                    }
                }
            }
        }
        catch (ThreadInterruptedException) { }  // Gracefully handle thread interruptions
        catch (Exception ex)
        {
            Debug.LogError($"❌ Erreur dans l'écoute du chat : {ex.Message}");
        }
    }

    #endregion Chat Listening

    #region Send Message to Twitch

    /// <summary>
    /// Sends a message to the Twitch chat via the bot.
    /// Can be triggered from the Unity Inspector.
    /// </summary>
    /// <param name="message">Message to send to the chat</param>
    public void SendMessageToChat(string message)
    {
        if (!string.IsNullOrEmpty(message) && twitchClient?.Connected == true)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :{message}");
            writer.Flush();
            Debug.Log($"✅ Message envoyé: {message}");
        }
        else
        {
            Debug.LogWarning("❌ Impossible d'envoyer le message. Vérifie la connexion.");
        }
    }

    #endregion Send Message to Twitch
}

#if UNITY_EDITOR

/// <summary>
/// Custom Editor for TwitchChatListener to display credentials in a foldout.
/// </summary>
[CustomEditor(typeof(TwitchChatListener))]
public class TwitchChatListenerEditor : Editor
{
    private bool showCredentials = false;

    /// <summary>
    /// Customizes the Inspector GUI for TwitchChatListener.
    /// </summary>
    public override void OnInspectorGUI()
    {
        TwitchChatListener listener = (TwitchChatListener)target;

        // Display credentials in a foldout
        showCredentials = EditorGUILayout.Foldout(showCredentials, "Twitch Credentials");

        if (showCredentials)
        {
            // Disable fields for editing in the editor
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Oauth Token", listener.OauthToken);
            EditorGUILayout.TextField("Bot Username", listener.BotUsername);
            EditorGUILayout.TextField("Channel Name", listener.ChannelName);
            EditorGUILayout.TextField("Client ID", listener.ClientID);
            EditorGUI.EndDisabledGroup();
        }

        DrawDefaultInspector();  // Display other fields in the Inspector
    }
}

#endif
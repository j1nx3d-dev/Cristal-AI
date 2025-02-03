using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace MyPckg_IskXR.LMStudio
{
    /// <summary>
    /// Handles WebSocket communication between Unity and LM Studio.
    /// </summary>
    public class UnityToLM : MonoBehaviour
    {
        #region WebSocket Configuration

        [Header("WebSocket Configuration")]
        [Tooltip("URL of the WebSocket server (e.g., ws://127.0.0.1:1234)")]
        public string WebSocketUrl = "ws://127.0.0.1:1234";

        #endregion WebSocket Configuration

        #region Message Parameters

        [Header("Message Parameters")]
        [Tooltip("The model used for processing the message.")]
        public string model = "granite-3.0-2b-instruct";

        [Tooltip("System message setting the assistant's behavior.")]
        public string systemContent = "Always answer in rhymes.";

        [Tooltip("User's input message.")]
        public string userContent = "Introduce yourself.";

        [Tooltip("Temperature setting for randomness in responses.")]
        public float temperature = 0.7f;

        [Tooltip("Maximum tokens for the response (-1 for unlimited).")]
        public int maxTokens = -1;

        [Tooltip("Enable or disable streaming response.")]
        public bool stream = false;

        #endregion Message Parameters

        #region UI Methods

        /// <summary>
        /// Called when a UI button is clicked to initiate the WebSocket connection.
        /// </summary>
        public void OnBtnClicked()
        {
            ToConnectToWS();
        }

        #endregion UI Methods

        #region WebSocket Connection

        /// <summary>
        /// Initiates a WebSocket connection and sends a message.
        /// </summary>
        private async void ToConnectToWS()
        {
            Debug.Log("Starting WebSocket connection...");

            // Validate WebSocket URL
            if (string.IsNullOrWhiteSpace(WebSocketUrl) ||
               (!(WebSocketUrl.StartsWith("ws") || WebSocketUrl.StartsWith("wss"))))
            {
                Debug.LogError("Invalid WebSocket URL.");
                return;
            }

            try
            {
                string jsonMessage = CreateJsonMessage();
                await Task.Run(async () => await SendWebSocketMessage(jsonMessage));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in ToConnectToWS: {ex.GetType()} - {ex.Message}");
                Debug.LogError($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.LogError($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }

        #endregion WebSocket Connection

        #region WebSocket Communication

        /// <summary>
        /// Sends a message to the WebSocket server and handles the response.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        private async Task SendWebSocketMessage(string message)
        {
            using (var ws = new ClientWebSocket())
            {
                try
                {
                    // Tentative de connexion
                    await ws.ConnectAsync(new Uri(WebSocketUrl), CancellationToken.None);
                    Debug.Log("Connected to WebSocket server.");

                    // Convertir le message en bytes et l'envoyer
                    byte[] buffer = Encoding.UTF8.GetBytes(message);
                    await ws.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length),
                                      WebSocketMessageType.Text, true, CancellationToken.None);

                    // Réception de la réponse
                    var receiveBuffer = new byte[1024];
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                        Debug.Log("Received: " + receivedMessage);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"WebSocket error: {ex.GetType()} - {ex.Message}");
                    Debug.LogError($"Stack Trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        Debug.LogError($"Inner Exception: {ex.InnerException.Message}");
                    }
                }
            }
        }

        #endregion WebSocket Communication

        #region JSON Message Handling

        /// <summary>
        /// Constructs a JSON message using the configured parameters.
        /// </summary>
        /// <returns>A formatted JSON string.</returns>
        private string CreateJsonMessage()
        {
            return $"{{\"model\": \"{model}\", \"messages\": [{{\"role\": \"system\", \"content\": \"{systemContent}\"}}, {{\"role\": \"user\", \"content\": \"{userContent}\"}}], \"temperature\": {temperature}, \"max_tokens\": {maxTokens}, \"stream\": {stream.ToString().ToLower()}}}";
        }

        #endregion JSON Message Handling
    }
}
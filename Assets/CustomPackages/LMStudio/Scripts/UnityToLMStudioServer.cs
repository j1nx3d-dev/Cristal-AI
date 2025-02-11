using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System;
using UnityEngine.Events;
using UnityEngine;
using System.Threading.Tasks;

namespace MyPckg_IskXR.LMStudio
{
    /// <summary>
    /// Handles HTTP communication between Unity and LM Studio API, including conversation history tracking.
    /// </summary>
    public class UnityToLMStudioServer : MonoBehaviour
    {
        #region WebSocket Configuration

        [Header("WebSocket Configuration")]
        [Tooltip("URL of the API server (e.g., http://127.0.0.1:1234/v1/chat/completions)")]
        public string ApiUrl = "http://127.0.0.1:1234/v1/chat/completions";

        [Header("Optional: Customize API Key Here")]
        [Tooltip("Place your API Key here if needed.")]
        public string ApiKey = "";

        #endregion WebSocket Configuration

        #region Message Parameters

        [Header("Message Parameters")]
        [Tooltip("The model used for processing the message.")]
        public string modelName = "lmstudio-community/llama-3.2-1b-instruct";

        [Header("System Content")]
        [Tooltip("System message setting the assistant's behavior.")]
        public string systemContent = "Answer user questions naturally.";

        [Header("User Input")]
        [Tooltip("Current user input message.")]
        public string userContent = "";

        [Space(10)]
        [Tooltip("Temperature setting for randomness in responses.")]
        public float temperature = 0.3f;

        [Tooltip("Maximum tokens for the response.")]
        public int maxNewTokens = 2048;

        [Tooltip("Enable or disable streaming response.")]
        public bool stream = true;

        [Tooltip("Top_p parameter for controlling randomness.")]
        public float topP = 1f;

        #endregion Message Parameters

        #region Conversation History

        /// <summary>
        /// Stores the conversation history for context.
        /// </summary>
        private List<object> messageHistory = new();

        #endregion Conversation History

        #region Unity Event for Response Handling

        [Header("Response Event")]
        [Tooltip("Event triggered when a response is received.")]
        public UnityEvent<string> OnResponseReceived = new();

        #endregion Unity Event for Response Handling

        #region UI Methods

        /// <summary>
        /// Called when a UI button is clicked to send a message.
        /// </summary>
        public void OnBtnClicked()
        {
            if (!string.IsNullOrEmpty(userContent))
            {
                ToSendRequestToAPI();
            }
            else
            {
                Debug.LogWarning("User input is empty. Please enter a message.");
            }
        }

        #endregion UI Methods

        #region HTTP Request Handling

        /// <summary>
        /// Sends an HTTP POST request to the API server and handles the response.
        /// </summary>
        private async void ToSendRequestToAPI()
        {
            try
            {
                string jsonMessage = CreateJsonMessage();
                Debug.Log($"Sending request: {jsonMessage}");

                await SendHttpRequest(jsonMessage);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in ToSendRequestToAPI: {ex.GetType()} - {ex.Message}");
            }
        }

        /// <summary>
        /// Sends the HTTP request and processes the response.
        /// </summary>
        private async Task SendHttpRequest(string jsonMessage)
        {
            using (HttpClient client = new())
            {
                try
                {
                    StringContent content = new(jsonMessage, Encoding.UTF8, "application/json");

                    if (!string.IsNullOrEmpty(ApiKey))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
                    }

                    HttpResponseMessage response = await client.PostAsync(ApiUrl, content);
                    Debug.Log($"Response Status Code: {response.StatusCode}");

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Debug.Log($"Received: {responseBody}");

                        var apiData = JsonConvert.DeserializeObject<ApiResponse>(responseBody);
                        var theResponse = apiData.choices[0].message.content;

                        // Add assistant's response to the history
                        messageHistory.Add(new { role = "assistant", content = theResponse });

                        DisplayMessage(theResponse);
                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        Debug.LogError($"Error: {errorResponse}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"HTTP Request error: {ex.GetType()} - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Creates the JSON message for the HTTP request.
        /// </summary>
        private string CreateJsonMessage()
        {
            if (messageHistory.Count == 0)
            {
                messageHistory.Add(new { role = "system", content = systemContent });
            }

            messageHistory.Add(new { role = "user", content = userContent });

            var jsonData = new
            {
                messages = messageHistory.ToArray(),
                model_params = new
                {
                    temperature,
                    top_p = topP,
                    max_new_tokens = maxNewTokens,
                    stream
                },
                model_name = modelName
            };

            return JsonConvert.SerializeObject(jsonData);
        }

        #endregion HTTP Request Handling

        #region Message Display

        /// <summary>
        /// Displays the response message from the bot and triggers the response event.
        /// </summary>
        private void DisplayMessage(string message)
        {
            Debug.Log($"Bot Response: {message}");
            OnResponseReceived.Invoke(message);
        }

        #endregion Message Display

        #region Response Model

        [Serializable]
        public class ApiResponse
        {
            public Choice[] choices;
        }

        [Serializable]
        public class Choice
        {
            public Message message;
        }

        [Serializable]
        public class Message
        {
            public string content;
        }

        #endregion Response Model
    }
}
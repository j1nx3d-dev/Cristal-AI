using UnityEditor;
using UnityEngine;

namespace MyPckg_IskXR.Nmap
{
    /// <summary>
    /// Editor window that allows users to interact with the Nmap tool from Unity's interface.
    /// </summary>
    public class IsekaiXRToolsMenu : EditorWindow
    {
        #region Serialized Fields

        [MenuItem("IsekaiXRTools/Nmap Tool")] // Creates a menu item in the Unity toolbar
        public static void ShowWindow()
        {
            GetWindow<IsekaiXRToolsMenu>("Nmap Tool");
        }

        private string ipAddress = "127.0.0.1"; // Default IP address
        private string port = "1234"; // Default port
        private string scanResult = ""; // Variable to store scan result
        private bool isPortOpen = false; // Store if port is open

        #endregion Serialized Fields

        #region GUI Methods

        /// <summary>
        /// Draws the UI for the window and handles interactions.
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Label("🔍 Nmap Menu", EditorStyles.boldLabel);

            // IP Address and Port Input Fields
            ipAddress = EditorGUILayout.TextField("IP Address:", ipAddress);
            port = EditorGUILayout.TextField("Port:", port);

            if (GUILayout.Button("🔎 Start Nmap"))
            {
                // Validate the port input
                if (int.TryParse(port, out int portInt))
                {
                    UnityEngine.Debug.Log($"Starting Nmap on {ipAddress}:{portInt}...");
                    RunNmapScan(ipAddress, portInt); // Execute the scan
                }
                else
                {
                    UnityEngine.Debug.LogError("❌ Invalid port specified!");
                }
            }

            // Display scan results
            GUILayout.Label("📜 Scan Results:", EditorStyles.boldLabel);

            // Set color based on port status
            GUIStyle resultStyle = new GUIStyle(EditorStyles.textArea);
            resultStyle.normal.textColor = isPortOpen ? Color.green : Color.red;

            // Display result with color
            EditorGUILayout.TextArea(scanResult, resultStyle, GUILayout.Height(300));
        }

        #endregion GUI Methods

        #region Nmap Execution

        /// <summary>
        /// Executes the Nmap scan and updates the UI with the result.
        /// </summary>
        /// <param name="ipAddress">IP address for the Nmap scan.</param>
        /// <param name="port">Port number for the Nmap scan.</param>
        private void RunNmapScan(string ipAddress, int port)
        {
            NmapExecutor.RunNmapScan(ipAddress, port, result =>
            {
                scanResult = result; // Store scan result

                // Vérifie si le port est ouvert dans le résultat
                isPortOpen = result.Contains("open");

                Repaint(); // Refresh the UI to display the result
            });
        }

        #endregion Nmap Execution
    }
}
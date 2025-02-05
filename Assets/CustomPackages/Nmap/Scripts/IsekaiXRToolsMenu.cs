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

        [MenuItem("IsekaiXRTools/Nmap Port Check")]
        /// <summary>
        /// Creates a menu item in the Unity toolbar to show the Nmap Port Check Tool window.
        /// </summary>
        public static void ShowWindow()
        {
            GetWindow<IsekaiXRToolsMenu>("Nmap Port Check Tool");
        }

        private string ipAddress = "127.0.0.1"; // Default IP address
        private string port = "1234"; // Default port
        private string scanResult = ""; // Variable to store scan result
        private bool isPortOpen = false; // Store if port is open
        private bool showScanResults = false; // Controls the foldout group for scan results

        #endregion Serialized Fields

        #region GUI Methods

        /// <summary>
        /// Draws the UI for the window and handles interactions.
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Label("🔍 Nmap Port Check Tool", EditorStyles.boldLabel);

            // Nmap Configuration
            GUILayout.Space(10);
            GUILayout.Label("Scan Configuration", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            // IP Address and Port Input Fields with tooltips
            ipAddress = EditorGUILayout.TextField(new GUIContent("🌐 IP Address:", "Enter the IP address you want to scan"), ipAddress);
            port = EditorGUILayout.TextField(new GUIContent("🔢 Port:", "Enter the port number you want to scan"), port);

            GUILayout.Space(10);

            // Start Nmap Button
            if (GUILayout.Button(new GUIContent("🔎 Start Nmap Scan", "Click to start the Nmap scan")))
            {
                // Validate the port input
                if (int.TryParse(port, out int portInt))
                {
                    Debug.Log($"Starting Port Check on {ipAddress}:{portInt}...");
                    RunNmapScan(ipAddress, portInt); // Execute the scan
                }
                else
                {
                    Debug.LogError("❌ Invalid port specified!");
                }
            }

            GUILayout.Space(10);

            // Display port status
            if (!string.IsNullOrEmpty(scanResult))
            {
                var statusStyle = new GUIStyle(EditorStyles.label)
                {
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = isPortOpen ? Color.green : Color.red }
                };

                string statusText = isPortOpen ? "Port is OPEN ✅" : "Port is CLOSED ❌";
                GUILayout.Label(statusText, statusStyle);
            }

            GUILayout.Space(10);

            // Scan results foldout
            showScanResults = EditorGUILayout.Foldout(showScanResults, "📜 Scan Results");
            if (showScanResults)
            {
                var resultStyle = new GUIStyle(EditorStyles.textArea)
                {
                    normal = { textColor = isPortOpen ? Color.green : Color.red }
                };

                EditorGUILayout.TextArea(scanResult, resultStyle, GUILayout.Height(300));
            }

            GUILayout.Space(10);

            // Path to Nmap executable input field with tooltip
            NmapExecutor.NmapPath = EditorGUILayout.TextField(new GUIContent("📂 Nmap Path:", "Specify the path to the Nmap executable"), NmapExecutor.NmapPath);
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

                // Check if the port is open in the result
                isPortOpen = result.Contains("open");

                Repaint(); // Refresh the UI to display the result
            });
        }

        #endregion Nmap Execution
    }
}

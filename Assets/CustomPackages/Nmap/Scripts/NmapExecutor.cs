using System;
using System.Diagnostics;
using UnityEngine;

namespace J1nx3d_IskXR.Nmap
{
    /// <summary>
    /// Class responsible for executing Nmap scans and returning the results.
    /// </summary>
    public class NmapExecutor : MonoBehaviour
    {
        #region Public Fields

        /// <summary>
        /// Path to the Nmap executable.
        /// </summary>
        public static string NmapPath = @"C:\Program Files (x86)\Nmap\nmap.exe"; // Default path to nmap.exe

        #endregion Public Fields

        #region Public Methods

        /// <summary>
        /// Runs an Nmap scan on a specified IP and port, and returns the result through a callback.
        /// </summary>
        /// <param name="ip">IP address to scan.</param>
        /// <param name="port">Port number to scan.</param>
        /// <param name="onResult">Callback to return the scan results.</param>
        public static void RunNmapScan(string ip, int port, Action<string> onResult)
        {
            ProcessStartInfo startInfo = CreateProcessStartInfo(ip, port);

            // Execute the process and get the output
            string output = ExecuteNmapProcess(startInfo);

            // Display results in Unity's console
            UnityEngine.Debug.Log("Nmap Scan Results:\n" + output);

            // Pass the results to the callback
            onResult?.Invoke(output);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Creates the ProcessStartInfo object for the Nmap command.
        /// </summary>
        /// <param name="ip">IP address to scan.</param>
        /// <param name="port">Port number to scan.</param>
        /// <returns>Configured ProcessStartInfo object.</returns>
        private static ProcessStartInfo CreateProcessStartInfo(string ip, int port)
        {
            return new ProcessStartInfo
            {
                FileName = NmapPath, // Uses the public static NmapPath variable
                Arguments = $"-sT {ip} -p {port}", // Nmap scan command
                RedirectStandardOutput = true, // Redirects the output to capture it
                UseShellExecute = false, // Prevents using the shell
                CreateNoWindow = true // Hides the process window
            };
        }

        /// <summary>
        /// Executes the Nmap process and returns the output.
        /// </summary>
        /// <param name="startInfo">ProcessStartInfo for Nmap.</param>
        /// <returns>The output of the Nmap scan.</returns>
        private static string ExecuteNmapProcess(ProcessStartInfo startInfo)
        {
            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();

                // Read the entire output from Nmap
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return output;
            }
        }

        #endregion Private Methods
    }
}

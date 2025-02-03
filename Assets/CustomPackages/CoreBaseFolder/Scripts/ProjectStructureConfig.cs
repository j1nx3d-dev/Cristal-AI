using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ProjectStructureConfig", menuName = "Tools/Project Structure Config")]
public class ProjectStructureConfig : ScriptableObject
{
    [Tooltip("Liste des dossiers � cr�er")]
    public List<string> folders = new List<string>();
}
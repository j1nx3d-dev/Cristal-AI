using MyPckg_IskXR.LMStudio;
using TMPro;
using UnityEngine;

public class OnBtnSendToLamaHlpr : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Champ de saisie utilisateur")]
    public TMP_InputField userInputField;

    [Tooltip("Référence au script UnityToLama pour envoyer le texte")]
    public UnityToLama unityToLama;

    private void Update()
    {
        if (userInputField != null && unityToLama != null)
        {
            // Met à jour la propriété UserContent de UnityToLama avec le texte du champ de saisie
            unityToLama.userContent = userInputField.text;
        }
    }
}
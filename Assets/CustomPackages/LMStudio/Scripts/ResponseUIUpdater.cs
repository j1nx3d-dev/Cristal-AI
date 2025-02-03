using UnityEngine;
using TMPro;
using MyPckg_IskXR.LMStudio;

public class ResponseUIUpdater : MonoBehaviour
{
    [SerializeField] private UnityToLama unityToLama;
    [SerializeField] private TMP_Text responseText;

    private void OnEnable()
    {
        if (unityToLama != null)
            unityToLama.OnResponseReceived.AddListener(UpdateUI);
    }

    private void OnDisable()
    {
        if (unityToLama != null)
            unityToLama.OnResponseReceived.RemoveListener(UpdateUI);
    }

    private void UpdateUI(string response)
    {
        responseText.text = response;
    }
}
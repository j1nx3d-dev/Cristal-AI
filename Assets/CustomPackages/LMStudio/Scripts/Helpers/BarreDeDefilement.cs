using UnityEngine;
using TMPro;

public class BarreDeDefilement : MonoBehaviour
{
    public float defilement = 100; // Valeur par défaut de la barre

    private void Start()
    {
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (defilement > 0)
        {
            GetComponent<TMP_Text>().text = "Defilement : " + defilement;
        }
        else
        {
            GetComponent<TMP_Text>().text = "";
        }
    }
}
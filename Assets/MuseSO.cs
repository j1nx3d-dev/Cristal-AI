using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "MuseSO", menuName = "Scriptable Objects/MuseSO")]
public class MuseSO : ScriptableObject
{
    #region Basic Properties
    /// <summary>
    /// The name preferred by the user.
    /// </summary>
    public string firstName = "Lizzy";

    /// <summary>
    /// The surname of the character.
    /// </summary>
    public string lastName = "Smith";

    /// <summary>
    /// The gender of the character (optional).
    /// </summary>
    public Gender gender;

    /// <summary>
    /// The age of the character in years.
    /// </summary>
    [Range(0, 120)]
    public int age = 18;

    /// <summary>
    /// The profession or role of the character.
    /// </summary>
    public string occupation = "Adventurer";

    /// <summary>
    /// The appearance of the character (optional).
    /// </summary>
    public Appearance appearance;
    #endregion

    #region LizzyShinyCode Feature
    /// <summary>
    /// Placeholder for the Lizzy shiny code feature logic.
    /// </summary>
    public void ActivateLizzyShinyCode()
    {
        // Summary: This method is triggered when "Lizzy shiny code!" is invoked.
        // Note: Additional logic for the shiny code feature can be added here.

        // Comment: This is a placeholder for future implementation of Lizzy shiny code logic.
        Debug.Log("Lizzy shiny code activated! Performing shiny action...");

        if (age >= 18)
        {
            Debug.Log($"{firstName} {lastName} is now performing a special adult action.");
        }
    }

    /// <summary>
    /// Enum for character gender.
    /// </summary>
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    /// <summary>
    /// Enum for character appearance.
    /// </summary>
    public enum Appearance
    {
        TallAndAthletic,
        ShortAndCute,
        AverageAndDashing
    }
    #endregion
}

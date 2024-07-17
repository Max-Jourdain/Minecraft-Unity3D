using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Settings : MonoBehaviour
{
    private const string Theme_SelectedToggleKey = "Theme_SelectedToggle";
    private const string Difficulty_SelectedToggleKey = "Difficulty_SelectedToggle";

    private void OnEnable()
    {
        ToggleChangeEvent.OnToggleChanged += SaveSettings;
        LoadSettings();
    }

    private void OnDisable()
    {
        ToggleChangeEvent.OnToggleChanged -= SaveSettings;
    }

    private void SaveSettings(ToggleType type, string toggleName)
    {
        if (type == ToggleType.Theme)
        {
            PlayerPrefs.SetString(Theme_SelectedToggleKey, toggleName);
        }
        else if (type == ToggleType.Difficulty)
        {
            PlayerPrefs.SetString(Difficulty_SelectedToggleKey, toggleName);
        }
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        string selectedToggleName = PlayerPrefs.GetString(Theme_SelectedToggleKey, null);

        if (!string.IsNullOrEmpty(selectedToggleName))
        {
            ToggleGroup toggleGroup = FindObjectOfType<ToggleGroup>();
            if (toggleGroup != null)
            {
                Toggle[] toggles = toggleGroup.GetComponentsInChildren<Toggle>();
                foreach (Toggle toggle in toggles)
                {
                    if (toggle.name == selectedToggleName)
                    {
                        toggle.isOn = true;
                        break;
                    }
                }
            }
        }
    }
}

public enum ToggleType
{
    Theme,
    Difficulty
}

public static class ToggleChangeEvent
{
    public static event Action<ToggleType, string> OnToggleChanged;

    public static void ToggleChanged(ToggleType type, string toggleName)
    {
        OnToggleChanged?.Invoke(type, toggleName);
        Debug.Log($"Toggle changed: {toggleName}, Type: {type}");
    }
}
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

    private void SaveSettings(string toggleName)
    {
        PlayerPrefs.SetString(Theme_SelectedToggleKey, toggleName);
        PlayerPrefs.SetString(Difficulty_SelectedToggleKey, toggleName);
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

public static class ToggleChangeEvent
{
    public static event Action<string> OnToggleChanged;

    public static void ToggleChanged(string toggleName)
    {
        OnToggleChanged?.Invoke(toggleName);
        Debug.Log("Toggle changed: " + toggleName);
    }
}

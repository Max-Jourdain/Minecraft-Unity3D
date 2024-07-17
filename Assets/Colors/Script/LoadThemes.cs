using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoadThemes : MonoBehaviour
{
    public ColorPalette[] themes; // Array of themes
    public GameObject themePrefab; // Prefab for the theme object
    public Transform scrollViewContent; // ScrollView content transform
    public GameObject toggleGroup; // Toggle group for the themes
    private const string SelectedThemeKey = "SelectedTheme";

    void Start()
    {
        LoadInThemes();
        LoadSelectedTheme();
    }

    void LoadInThemes()
    {
        foreach (ColorPalette theme in themes)
        {
            GameObject themeObject = Instantiate(themePrefab, scrollViewContent);
            themeObject.transform.Find("ThemeName").GetComponent<TMP_Text>().text = theme.paletteName;

            // Find all children with the name "ColorImage"
            Image[] colorImages = themeObject.GetComponentsInChildren<Image>();
            int colorIndex = 0;

            foreach (Image img in colorImages)
            {
                if (img.gameObject.name == "ColorImage" && colorIndex < theme.colors.Length)
                {
                    img.color = theme.colors[colorIndex];
                    colorIndex++;
                }
            }

            // Assign the theme object to the ToggleGroup
            Toggle toggle = themeObject.GetComponent<Toggle>();
            toggle.group = toggleGroup.GetComponent<ToggleGroup>();

            // Add listener to the toggle to save the selected theme
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    SaveSelectedTheme(theme.paletteName);
                    ToggleChangeEvent.ToggleChanged(theme.paletteName);
                }
            });
        }
    }

    void SaveSelectedTheme(string themeName)
    {
        PlayerPrefs.SetString(SelectedThemeKey, themeName);
        PlayerPrefs.Save();
    }

    void LoadSelectedTheme()
    {
        string selectedThemeName = PlayerPrefs.GetString(SelectedThemeKey, null);
        if (!string.IsNullOrEmpty(selectedThemeName))
        {
            Toggle[] toggles = toggleGroup.GetComponentsInChildren<Toggle>();
            foreach (Toggle toggle in toggles)
            {
                TMP_Text themeNameText = toggle.GetComponentInChildren<TMP_Text>();
                if (themeNameText != null && themeNameText.text == selectedThemeName)
                {
                    toggle.isOn = true;
                    break;
                }
            }
        }
    }
}

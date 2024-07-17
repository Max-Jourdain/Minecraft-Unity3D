using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class LoadThemes : MonoBehaviour
{
    public ColorPalette[] themes; // Array of themes
    public GameObject themePrefab; // Prefab for the theme object
    public Transform scrollViewContent; // ScrollView content transform
    public GameObject ToggleGroup; // Toggle group for the themes

    void Start()
    {
        LoadInThemes();
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
            themeObject.GetComponent<Toggle>().group = ToggleGroup.GetComponent<ToggleGroup>();
        }
    }
}

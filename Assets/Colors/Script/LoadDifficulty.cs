using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoadDifficulty : MonoBehaviour
{
    public Difficulty[] difficulties; 
    public GameObject difficultiesPrefab; 
    public Transform scrollViewContent; 
    public GameObject toggleGroup; 
    private const string SelectedDifficultiesKey = "SelectedDifficulties";
    private const string SelectedDifficultyNameKey = "SelectedDifficultyName";

    void Start()
    {
        LoadInDifficulties();
        LoadSelectedDifficulties();
    }

    void LoadInDifficulties()
    {
        foreach (Difficulty dif in difficulties)
        {
            GameObject themeObject = Instantiate(difficultiesPrefab, scrollViewContent);
            themeObject.transform.Find("DifficultyName").GetComponent<TMP_Text>().text = dif.difficultyName;

            // Assign the theme object to the ToggleGroup
            Toggle toggle = themeObject.GetComponent<Toggle>();
            toggle.group = toggleGroup.GetComponent<ToggleGroup>();

            // Add listener to the toggle to save the selected Difficulty
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    SaveSelectedDifficulties(dif.minesProbability, dif.difficultyName);
                    ToggleChangeEvent.ToggleChanged(ToggleType.Difficulty, dif.difficultyName);
                }
            });
        }
    }

    void SaveSelectedDifficulties(float minesProbability, string difficultyName)
    {
        PlayerPrefs.SetFloat(SelectedDifficultiesKey, minesProbability);
        PlayerPrefs.SetString(SelectedDifficultyNameKey, difficultyName);
        PlayerPrefs.Save();
    }

    void LoadSelectedDifficulties()
    {
        string selectedDifName = PlayerPrefs.GetString(SelectedDifficultyNameKey, null);
        if (!string.IsNullOrEmpty(selectedDifName))
        {
            Toggle[] toggles = toggleGroup.GetComponentsInChildren<Toggle>();
            foreach (Toggle toggle in toggles)
            {
                TMP_Text themeNameText = toggle.GetComponentInChildren<TMP_Text>();
                if (themeNameText != null && themeNameText.text == selectedDifName)
                {
                    toggle.isOn = true;
                    break;
                }
            }
        }
    }
}

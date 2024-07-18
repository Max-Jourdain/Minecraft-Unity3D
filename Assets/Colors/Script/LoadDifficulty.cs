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
                    SaveSelectedDifficulties(dif.minesProbability);
                    ToggleChangeEvent.ToggleChanged(ToggleType.Difficulty, dif.difficultyName);
                }
            });
        }
    }

    void SaveSelectedDifficulties(float minesProbability)
    {
        PlayerPrefs.SetFloat(SelectedDifficultiesKey, minesProbability);
        PlayerPrefs.Save();
    }

    void LoadSelectedDifficulties()
    {
        string selectedDifficulty = PlayerPrefs.GetString(SelectedDifficultiesKey);
        Toggle[] toggles = toggleGroup.GetComponentsInChildren<Toggle>();

        foreach (Toggle toggle in toggles)
        {
            if (toggle.gameObject.transform.Find("DifficultyName").GetComponent<TMP_Text>().text == selectedDifficulty)
            {
                toggle.isOn = true;
            }
        }
    }
}

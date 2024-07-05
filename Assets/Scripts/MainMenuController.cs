using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance { get; private set; }
    public TMP_Dropdown difficultyDropdown;
    public Difficulty selectedDifficulty;
    public string gameSceneName = "Game";

    private bool isInitialized = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!isInitialized)
        {
            difficultyDropdown.onValueChanged.AddListener(delegate { UpdateSelectedDifficulty(); });
            isInitialized = true;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void UpdateSelectedDifficulty()
    {
        selectedDifficulty = (Difficulty)difficultyDropdown.value;
    }
}

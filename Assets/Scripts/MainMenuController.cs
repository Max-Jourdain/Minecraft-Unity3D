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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        difficultyDropdown.onValueChanged.AddListener(delegate { UpdateSelectedDifficulty(); });
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance { get; private set; }
    public Slider difficultySlider;
    public string gameSceneName = "Game";
    private bool isInitialized = false;
    public int selectedDifficulty = 0;
    [SerializeField] private TMP_Text difficultyPercentText;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text difficultyTextValue;

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
            difficultySlider.onValueChanged.AddListener(delegate { UpdateSelectedDifficulty(); });
            isInitialized = true;
        }
    }

    void Start()
    {
        // Load the saved difficulty
        selectedDifficulty = PlayerPrefs.GetInt("Difficulty");
        difficultySlider.value = selectedDifficulty;
        UpdateSelectedDifficulty();
    }

    public void StartGame()
    {
        // Save the current difficulty
        PlayerPrefs.SetInt("Difficulty", selectedDifficulty);
        PlayerPrefs.Save();

        SceneManager.LoadScene(gameSceneName);
    }

    public void UpdateSelectedDifficulty()
    {
        selectedDifficulty = (int)difficultySlider.value;
        float percentage = difficultySlider.normalizedValue;
        Color color = gradient.Evaluate(percentage);

        string difficultyLabel = "";

        if (selectedDifficulty < 13)
        {
            difficultyLabel = "Easy";
        }
        else if (selectedDifficulty >= 13 && selectedDifficulty < 16)
        {
            difficultyLabel = "Intermediate";
        }
        else if (selectedDifficulty >= 16 && selectedDifficulty < 19)
        {
            difficultyLabel = "Hard";
        }
        else
        {
            difficultyLabel = "Expert";
        }
        difficultyTextValue.text = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + difficultyLabel + "</color>";
        difficultyPercentText.text = selectedDifficulty.ToString() + "%";

        fillImage.color = color;
    }
}

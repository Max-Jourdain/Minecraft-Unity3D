using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance { get; private set; }
    public string gameSceneName = "Game";
    private bool isInitialized = false;

    [Header("Difficulty Settings")]
    public int selectedDifficulty = 0;
    public int mineProbability = 0;
    [SerializeField] private ToggleGroup difficultyToggleGroup;
    [SerializeField] private GameObject difficultyTogglePrefab;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unselectedColor;

    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject loadingScreen;

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
            difficultyToggleGroup.SetAllTogglesOff();
            isInitialized = true;
        }
    }

    private void Start()
    {
        CreateDifficultyToggles();
    }

    private void CreateDifficultyToggles()
    {
        for (int i = 0; i < 5; i++)
        {
            Toggle toggle = Instantiate(difficultyTogglePrefab, difficultyToggleGroup.transform).GetComponent<Toggle>();
            toggle.group = difficultyToggleGroup;
            toggle.isOn = i == selectedDifficulty;

            ToggleData toggleData = toggle.GetComponent<ToggleData>();
            toggleData.SetDifficultyText(i);
            toggleData.SetMineProbabilityText(i);
            toggleData.SetScoreMultiplierText(i);
            toggleData.SetMineProbability(i);

            toggle.onValueChanged.AddListener((value) => { OnDifficultySelected(toggle); });
        }

        difficultyToggleGroup.SetAllTogglesOff();
        difficultyToggleGroup.transform.GetChild(0).GetComponent<Toggle>().Select();
    }

    private void OnDifficultySelected(Toggle toggle)
    {
        selectedDifficulty = toggle.transform.GetSiblingIndex();
        mineProbability = toggle.GetComponent<ToggleData>().currentMineProbability;

        ToggleData toggleData = toggle.GetComponent<ToggleData>();
        toggleData.backgroundImages[0].color = toggle.isOn ? selectedColor : unselectedColor;
        toggleData.backgroundImages[1].color = toggle.isOn ? selectedColor : unselectedColor;

    }

    public void StartGame()
    {
        loadingScreen.SetActive(true);
        SceneManager.LoadScene(gameSceneName);
    }



    public void QuitGame()
    {
        Application.Quit();
    }
}

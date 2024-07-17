using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public TerrainModifier terrainModifier;


    [Header("UI Panels")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject scoreScreen;
    [SerializeField] private GameObject settingsScreen;

    [Header("Settings")]
    [SerializeField] private Toggle vibrationToggle;
    [SerializeField] private Toggle soundToggle;

    private const string VibrationPrefKey = "Vibration";
    private const string SoundPrefKey = "Sound";

    void Start()
    {
        // Load saved settings
        vibrationToggle.isOn = PlayerPrefs.GetInt(VibrationPrefKey, 1) == 1;
        soundToggle.isOn = PlayerPrefs.GetInt(SoundPrefKey, 1) == 1;

        // Add listeners to handle toggle changes
        vibrationToggle.onValueChanged.AddListener(OnVibrationToggleChanged);
        soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);
    }

    private void OnVibrationToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt(VibrationPrefKey, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void OnSoundToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt(SoundPrefKey, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    // get state of vibration toggle
    public bool IsVibrationEnabled()
    {
        return vibrationToggle.isOn;
    }

    public void DisableScoreScreen()
    {
        StartCoroutine(FadeScreen(scoreScreen.GetComponent<CanvasGroup>(), 0.5f, false));
    }

    public void ShowSettings()
    {
        settingsScreen.gameObject.SetActive(true);
    }

    public void HideSettings()
    {
        settingsScreen.gameObject.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        ResetGame();
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator LoadSceneAsync()
    {
        // Create an async operation to load the scene
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

        // Update the loading bar while the scene is loading
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        // Hide the loading screen
        loadingScreen.SetActive(false);
    }

    public void ResetGame()
    {
        Block.UpdateTile(BlockType.Mine, Tile.Unplayed);
        Time.timeScale = 1;

        terrainModifier.hasFirstClickOccurred = false;
        terrainModifier.isGameOver = false;

        playerMovement.ResetPlayerPosition();

        // Show the loading screen
        loadingScreen.SetActive(true);
        
        // Start the loading process asynchronously
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator FadeScreen(CanvasGroup canvasGroup, float duration, bool fadeIn)
    {
        float elapsedTime = 0f;
        float startAlpha = fadeIn ? 0f : 1f;
        float targetAlpha = fadeIn ? 1f : 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        if (!fadeIn)
        {
            canvasGroup.gameObject.SetActive(false);
        }
    }
}
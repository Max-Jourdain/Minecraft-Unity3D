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
    [SerializeField] private GameObject restartConfirmationScreen;
    [SerializeField] private GameObject gameOverScreen;

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

    // show the Game over screen
    public void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
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

    public bool IsVibrationEnabled()
    {
        return vibrationToggle.isOn;
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
        // if game is not over show the restart confirmation screen
        if (!terrainModifier.isGameOver)
        {
            restartConfirmationScreen.SetActive(true);
            return;
        }

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

    // hard reset the game
    public void HardResetGame()
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

    // close the restart confirmation screen
    public void CloseRestartConfirmationScreen()
    {
        restartConfirmationScreen.SetActive(false);
    }

}
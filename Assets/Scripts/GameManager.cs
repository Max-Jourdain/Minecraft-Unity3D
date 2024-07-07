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
    [SerializeField] public GameObject gameOverScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject scoreScreen;

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

    public void RewardContinue()
    {
        terrainModifier.RewardContinue();
        gameOverScreen.SetActive(false);
    }

    public void DisableScoreScreen()
    {
        scoreScreen.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
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
}
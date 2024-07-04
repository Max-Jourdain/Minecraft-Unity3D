using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public TerrainModifier terrainModifier;

    [SerializeField] private GameObject loadingScreen;

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TerrainModifier terrainModifier;
    public TerrainGenerator terrainGenerator;
    public PlayerMovement playerMovement;

    public void ResetGame()
    {
        if (terrainModifier != null && terrainGenerator != null)
        {
            terrainModifier.ResetTerrain();
            terrainGenerator.ResetTerrain();

            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

            Debug.Log("Game has been reset.");
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public TerrainModifier terrainModifier;

    public void ResetGame()
    {
        Block.UpdateTile(BlockType.Mine, Tile.Unplayed);
        Block.UpdateTile(BlockType.Flag, Tile.Flag);

        terrainModifier.hasFirstClickOccurred = false;
        terrainModifier.isGameOver = false;

        playerMovement.ResetPlayerPosition();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
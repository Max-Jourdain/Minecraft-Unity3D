using UnityEngine;

public class TileColorChanger : MonoBehaviour
{
    public Texture2D atlas; // Your atlas texture
    public Material baseMaterial; // Material that uses the atlas
    private int tileWidth = 128; // Width of each tile
    private int tileHeight = 128; // Height of each tile

    public ColorPalette[] colorPalettes; // Array of color palettes
    private ColorPalette currentPalette; // Currently active palette

    // Define the tile positions for the color tiles
    private Vector2Int[] colorTiles = new Vector2Int[]
    {
        new Vector2Int(2, 0), // Color1
        new Vector2Int(2, 1), // Color2
        new Vector2Int(2, 2), // Color3
        new Vector2Int(2, 3), // Color4
        new Vector2Int(2, 4)  // Color5
    };

    void Start()
    {
        if (colorPalettes.Length > 0)
        {
            SetColorPalette(colorPalettes[0]); // Initialize with the first palette
        }
    }

    public void SetColorPalette(ColorPalette palette)
    {
        currentPalette = palette;
        ApplyColorPalette();
    }

    private void ApplyColorPalette()
    {
        if (currentPalette == null || currentPalette.colors.Length < colorTiles.Length)
        {
            Debug.LogError("Palette is null or does not have enough colors.");
            return;
        }

        for (int i = 0; i < colorTiles.Length; i++)
        {
            SetTileColor(colorTiles[i].x, colorTiles[i].y, currentPalette.colors[i]);
        }
    }

    private void SetTileColor(int tileX, int tileY, Color color)
    {
        if (atlas == null)
        {
            Debug.LogError("Atlas texture is not set.");
            return;
        }

        // Calculate the starting coordinates of the tile
        int startX = tileX * tileWidth;
        int startY = tileY * tileHeight;

        // Create a color array for the inner 120x120 pixels of the tile
        int innerWidth = 120;
        int innerHeight = 120;
        int innerStartX = startX + 4;
        int innerStartY = startY + 4;
        
        Color[] colors = new Color[innerWidth * innerHeight];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = color; // Set each pixel to the specified color
        }

        // Set the inner pixels of the tile to the specified color
        atlas.SetPixels(innerStartX, innerStartY, innerWidth, innerHeight, colors);
        atlas.Apply();

        // Update the material to use the modified atlas
        if (baseMaterial != null)
        {
            baseMaterial.mainTexture = atlas;
        }
    }

    // Method to switch to the next palette
    public void NextPalette()
    {
        int currentIndex = System.Array.IndexOf(colorPalettes, currentPalette);
        int nextIndex = (currentIndex + 1) % colorPalettes.Length;
        SetColorPalette(colorPalettes[nextIndex]);
    }

    // Method to switch to the previous palette
    public void PreviousPalette()
    {
        int currentIndex = System.Array.IndexOf(colorPalettes, currentPalette);
        int previousIndex = (currentIndex - 1 + colorPalettes.Length) % colorPalettes.Length;
        SetColorPalette(colorPalettes[previousIndex]);
    }
}

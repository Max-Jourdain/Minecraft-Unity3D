using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    // Enum to define cell types
    public enum Type { Empty, Mine, Number }

    // Serialized fields
    [SerializeField] private TMP_Text cellText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color defaultColor, flagColor, mineColor, emptyColor;

    private Game _game;

    // Public properties
    public Type CellType;
    public Vector3Int Position;
    public int AdjacentMineCount { get; private set; }
    public bool IsRevealed { get; private set; }
    public bool IsFlagged { get; private set; }


    private void Awake()
    {
        _game = FindFirstObjectByType<Game>();
    }

    // Initialize cell with type and position
    public void Initialize(Type type, int posX, int posY)
    {
        CellType = type;
        Position = new Vector3Int(posX, posY, 0);
    }

    // Handle cell click action
    public void OnCellClicked()
    {
        if (IsRevealed || IsFlagged) return;

        RevealCell();

        if (CellType == Type.Empty)
        {
            _game.ChordAdjacentCells(Position.x, Position.y);
        }
    }

    // Toggle flag status on cell
    public void ToggleFlag()
    {
        if (IsRevealed) return;

        IsFlagged = !IsFlagged;
        UpdateFlagDisplay();
    }

    private void UpdateFlagDisplay()
    {
        backgroundImage.color = IsFlagged ? flagColor : defaultColor;
    }

    // Set cell as a mine
    public void SetAsMine()
    {
        CellType = Type.Mine;
    }

    // Set number of adjacent mines
    public void SetAdjacentMines(int count)
    {
        CellType = Type.Number;
        AdjacentMineCount = count;
    }

    // Chord empty cells recursively
    public void ChordEmptyCells()
    {
        if (CellType != Type.Empty || IsRevealed) return;

        RevealCell();
        _game.ChordAdjacentCells(Position.x, Position.y);
    }

    // Reveal cell and update its display
    private void RevealCell()
    {
        IsRevealed = true;
        DisplayCellContent();
    }

    // Update cell display based on its status
    private void DisplayCellContent()
    {
        switch (CellType)
        {
            case Type.Mine:
                backgroundImage.color = mineColor;
                break;
            case Type.Number:
                cellText.text = AdjacentMineCount.ToString();
                cellText.color = GetNumberColor(AdjacentMineCount);
                break;
            case Type.Empty:
                backgroundImage.color = emptyColor;
                break;
        }
    }

    // Get color based on mine count
    private Color GetNumberColor(int count)
    {
        switch (count)
        {
            case 1: return new Color(0.0f, 0.0f, 1.0f); // Blue
            case 2: return new Color(0.0f, 0.8f, 0.0f); // Green
            case 3: return new Color(1.0f, 0.0f, 0.0f); // Red
            case 4: return new Color(0.0f, 0.0f, 0.5f); // Dark Blue
            case 5: return new Color(0.5f, 0.0f, 0.0f); // Maroon
            case 6: return new Color(0.0f, 0.5f, 0.5f); // Turquoise
            case 7: return new Color(0.0f, 0.0f, 0.0f); // Black
            case 8: return new Color(0f, 0f, 0f); // Black
            default: return Color.black;
        }
    }
}

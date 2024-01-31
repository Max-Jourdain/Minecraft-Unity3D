using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class Game : MonoBehaviour
{
    private bool isFirstClick = true;
    [SerializeField] private GridManager _gridManager;

    private void Awake()
    {
        _gridManager = FindFirstObjectByType<GridManager>();
    }

    void Start()
    {
        _gridManager.LoadInitialChunks();
    }

    void Update()
    {
        CheckMouseInput();
    }

    private void CheckMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) HandleLeftClick();                 // Left click to reveal cell
        else if (Input.GetMouseButtonDown(1)) HandleRightClick();           // Right click to flag cell
        else if (Input.GetKeyDown(KeyCode.R)) ResetGame();                  // Press R to reset game
        else if (Input.GetKeyDown(KeyCode.Space)) UpdateGeneration();       // Press Space to update generation
    }

    private void ResetGame()
    {
        isFirstClick = true;
        _gridManager.isFirstChunkMinesActive = false;
        _gridManager.chunkNumber = 0;

        foreach (var chunk in _gridManager.chunks.Values)
        {
            Destroy(chunk.ChunkObject);
        }

        _gridManager.chunks.Clear();
        _gridManager.generatedCells.Clear();

        _gridManager.LoadInitialChunks();
    }


    private void UpdateGeneration()
    {
        _gridManager.SpawnChunkAtWorldCoord(new Vector3(0, 0, 100));
        CalculateAdjacentMines();
        _gridManager.DeleteChunks();
    }

    private Cell GetCellUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.GetComponent<Cell>();
        }
        return null;
    }

    private void HandleRightClick()
    {
        Cell clickedCell = GetCellUnderMouse();
        clickedCell?.ToggleFlag();
    }

    private void HandleLeftClick()
    {
        Cell clickedCell = GetCellUnderMouse();
        if (clickedCell != null)
        {
            ProcessCellClick(clickedCell);
        }
    }

    private void ProcessCellClick(Cell cell)
    {
        if (isFirstClick)
        {
            isFirstClick = false;
            _gridManager.AddMinesToFirstChunk(cell);
            CalculateAdjacentMines(); 
        }

        cell.OnCellClicked();
    }


    void CalculateAdjacentMines()
    {
        foreach (var kvp in _gridManager.generatedCells)
        {
            Vector2Int position = kvp.Key;
            Cell cell = kvp.Value;

            if (cell.CellType != Cell.Type.Mine)
            {
                int adjacentMines = GetAdjacentMineCount(position.x, position.y);
                cell.SetAdjacentMines(adjacentMines);

                if (adjacentMines == 0)
                {
                    cell.CellType = Cell.Type.Empty;
                }
            }
        }
    }

    int GetAdjacentMineCount(int x, int y)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int adjacentX = x + i;
                int adjacentY = y + j;

                // Use the GetCellAt method to safely access cells
                Cell adjacentCell = _gridManager.GetCellAt(adjacentX, adjacentY);
                if (adjacentCell != null && adjacentCell.CellType == Cell.Type.Mine)
                {
                    count++;
                }
            }
        }
        return count;
    }

    bool IsInBounds(int x, int y)
    {
        return _gridManager.generatedCells.ContainsKey(new Vector2Int(x, y));
    }

    public void ChordAdjacentCells(int x, int y)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; // Skip the current cell

                int adjacentX = x + i;
                int adjacentY = y + j;

                if (IsInBounds(adjacentX, adjacentY))
                {
                    Cell adjacentCell = _gridManager.generatedCells[new Vector2Int(adjacentX, adjacentY)];

                    // Check if the adjacent cell is not revealed and not flagged
                    if (!adjacentCell.IsRevealed && !adjacentCell.IsFlagged)
                    {
                        if (adjacentCell.CellType == Cell.Type.Empty)
                        {
                            adjacentCell.ChordEmptyCells(); // Recursive call
                        }
                        else if (adjacentCell.CellType == Cell.Type.Number)
                        {
                            adjacentCell.OnCellClicked(); // Reveal the number cell
                        }
                    }
                }
            }
        }
    }
}

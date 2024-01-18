using System;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
using System.Collections.Generic;

public class GameGrid : Singleton<GameGrid>
{
    [SerializeField] private GridScriptables _gridScriptableObject;
    [SerializeField] private GridCellScriptables _gridCellScriptableObject;

    public bool HasCompletedTheGrid;
    public bool HasCompletedFalling;
    public bool ShouldNotPlaySpawnSound;
    public bool DontSpawnPlayer;
    [HideInInspector] public Player Player;

    // Private Variables
    private float currentSpeedMultiplier = 1.0f; 
    private GameObject[,] _gameGrid;
    private GridCell[,] _gridCells;
    private GameManager _gameManager;
    private ObjectPool<GameObject> _pool;
    private float _delay;

    private void Awake() 
    {
        _pool = new ObjectPool<GameObject>(() => 
        {
            return Instantiate(_gridScriptableObject.GridCellPrefab);
        }, gridCell =>
        {
            gridCell.gameObject.SetActive(true);
        }, gridCell =>
        {
            gridCell.gameObject.SetActive(false); 
        }, gridCell =>
        {
            Destroy(gridCell.gameObject); 
        }, false, 30, 60);
    }

    private void Start()
    {
        try
        {
            StartCoroutine(CreateGrid());
            _gameManager = GameManager.Instance;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
        }
    }

    private void Update() 
    {
        if (!_gameManager.HasCompletedGame)
        {
            AnimateAllGridCells();
        }
    }

    private IEnumerator CreateGrid()
    {
        if (_gridScriptableObject.GridCellPrefab == null)
        {
            throw new Exception("Grid Cell Prefab on the Game Grid has not been assigned");
        }

        _gameGrid = new GameObject[_gridScriptableObject.Height, _gridScriptableObject.Width];
        _gridCells = new GridCell[_gridScriptableObject.Height, _gridScriptableObject.Width];
        var _availableGridCells = GenericPool<List<GridCell>>.Get();

        _delay = _gridScriptableObject.InitialDelay;

        for (int y = 0; y < _gridScriptableObject.Height; y++)
        {
            for (int x = 0; x < _gridScriptableObject.Width; x++)
            {
                var newCell = _pool.Get();
                _gridCells[y, x] = newCell.GetComponent<GridCell>();
                newCell.transform.position = new Vector3(x * _gridScriptableObject.GridSpaceSize, 0, y * _gridScriptableObject.GridSpaceSize);
                newCell.transform.parent = transform;
                newCell.name = $"Grid Space (X: {x}, Y: {y})";

                GridCell gridCell  = newCell.GetComponent<GridCell>();
                if (gridCell != null)
                {
                    gridCell.SetPosition(y, x);
                    gridCell.SetColumn(y);
                    _availableGridCells.Add(gridCell);
                }

                if (!ShouldNotPlaySpawnSound) AudioManager.Instance.PlaySfx("OnCreateGrid");
                
                _gameGrid[y, x] = newCell;

                yield return new WaitForSeconds(_delay); 
                _delay -= _gridScriptableObject.SpeedUpFactor;
                _delay = Mathf.Max(_delay, 0f);
            }
        }

        HasCompletedTheGrid = true;
        ColorGridCell(_gridScriptableObject.XFinalCellCoordinate, _gridScriptableObject.YFinalCellCoordinate, _gridScriptableObject.FinalCellColor);
        MakeProhibitedCell(_gridScriptableObject.ProhibitedCellPositions);
        if (!DontSpawnPlayer) SpawnPlayer();
    }

    public IEnumerator DestroyGrid()
    {
        foreach(GridCell cell in _gridCells)
        {
            for (int y = 0; y < _gridScriptableObject.Height; y++)
            {
                if (!cell.SelectedCell)
                {
                    cell.MakeMeFall(_gridScriptableObject.FallingSpeed);
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        HasCompletedFalling = true;
    }

    private void MakeProhibitedCell(List<Vector2Int> prohibitedCellPositions)
    {
        foreach (var position in prohibitedCellPositions)
        {
            int x = position.x;
            int y = position.y;

            if (x >= 0 && y >= 0 && x < _gameGrid.GetLength(1) && y < _gameGrid.GetLength(0))
            {
                GridCell grid = _gridCells[y, x];

                if (grid != null)
                {
                    grid.CantGo = true;
                }
            }
            else
            {
                throw new Exception($"Cell at position ({x}, {y}) does not exist in the grid or is out of bounds.");
            }
        }
    }

    public void ColorGridCell(int x, int y, Color color)
    {
        if (x >= 0 && y >= 0 && x < _gameGrid.GetLength(1) && y < _gameGrid.GetLength(0))
        {
            GameObject cellToColor = _gameGrid[y, x];
            _gameManager._selectedCell = cellToColor.GetComponent<GridCell>();
            _gameManager.ChangeSelectedCellColor(_gridScriptableObject.SelectedCellColorCode);
            
            if (cellToColor != null)
            {
                Renderer cellRenderer = cellToColor.GetComponentInChildren<Renderer>();
                if (cellRenderer != null)
                {
                    cellRenderer.material.color = color;
                }
            }
        }
        else
        {
            throw new Exception($"Cell at position ({x}, {y}) does not exist in the grid or is out of bounds.");
        }
    }

    private void AnimateAllGridCells()
    {
        float time = Time.time;

        currentSpeedMultiplier = Mathf.Lerp(currentSpeedMultiplier, _gridCellScriptableObject.FloatSpeed, _gridCellScriptableObject.FloatSpeedSmoothness);

        for (int y = 0; y < _gridScriptableObject.Height; y++)
        {
            for (int x = 0; x < _gridScriptableObject.Width; x++)
            {
                GameObject cell = _gameGrid[y, x];
                Vector3 initialPosition = new Vector3(x * _gridScriptableObject.GridSpaceSize, 0, y * _gridScriptableObject.GridSpaceSize);
                int columnId = y; 

                float phase = columnId * _gridCellScriptableObject.WavePhaseMultiplier + (x + y) * _gridCellScriptableObject.VariationMultiplier;
                float sineWave = Mathf.Sin((time + phase) * _gridCellScriptableObject.FloatMagnitude * currentSpeedMultiplier);
                float yOffset = sineWave * _gridCellScriptableObject.FloatMagnitude;

                if (cell != null)
                {
                    Vector3 newPosition = initialPosition + new Vector3(0, yOffset, 0f);
                    cell.transform.position = newPosition;
                }
            }
        }
    }

    private void SpawnPlayer()
    {
        if (_gridScriptableObject.SpawnCellXCoordinate >= 0 && _gridScriptableObject.SpawnCellYCoordinate >= 0 && _gridScriptableObject.SpawnCellXCoordinate < _gridScriptableObject.Width && _gridScriptableObject.SpawnCellYCoordinate < _gridScriptableObject.Height)
        {
            GameObject cellObject = _gameGrid[_gridScriptableObject.SpawnCellYCoordinate, _gridScriptableObject.SpawnCellXCoordinate];
            GridCell gridCell = cellObject.GetComponent<GridCell>();

            gridCell.SpawnCell = true;

            GameObject player = Instantiate(_gridScriptableObject.PlayerPrefab, gridCell.SpawnPoint.transform.position, Quaternion.identity);
            Player = player.GetComponent<Player>();
        }
        else
        {
            throw new Exception("Invalid spawn coordinates for the player.");
        }
    }
}
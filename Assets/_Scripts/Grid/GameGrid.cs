using System;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class GameGrid : Singleton<GameGrid>
{
    [SerializeField] private GridScriptables _gridScriptableObject;

    [field: SerializeField] public bool HasCompletedTheGrid;
    [field: SerializeField] public bool HasCompletedFalling;

    // Private Variables
    private GameObject[,] _gameGrid;
    private AudioSource _audioSource;
    private GameManager _gameManager;
    private ObjectPool<GameObject> _pool;
    private float _delay;
    private float _delayFalling;

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
            _audioSource = GetComponent<AudioSource>();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
        }
    }

    private IEnumerator CreateGrid()
    {
        if (_gridScriptableObject.GridCellPrefab == null)
        {
            throw new Exception("Grid Cell Prefab on the Game Grid has not been assigned");
        }

        _gameGrid = new GameObject[_gridScriptableObject.Height, _gridScriptableObject.Width];

        _delay = _gridScriptableObject.InitialDelay;

        for (int y = 0; y < _gridScriptableObject.Height; y++)
        {
            for (int x = 0; x < _gridScriptableObject.Width; x++)
            {
                var newCell = _pool.Get();
                newCell.transform.position = new Vector3(x * _gridScriptableObject.GridSpaceSize, 0, y * _gridScriptableObject.GridSpaceSize);
                newCell.transform.parent = transform;
                newCell.name = $"Grid Space (X: {x}, Y: {y})";

                GridCell gridCell  = newCell.GetComponent<GridCell>();
                gridCell.SetPosition(y, x);
                gridCell.SetColumn(y);

                if (_audioSource != null && _gridScriptableObject.CreateClip != null)
                {
                    _audioSource.PlayOneShot(_gridScriptableObject.CreateClip);
                }

                _gameGrid[y, x] = newCell;

                yield return new WaitForSeconds(_delay); 
                _delay -= _gridScriptableObject.SpeedUpFactor;
                _delay = Mathf.Max(_delay, 0f);
            }
        }

        HasCompletedTheGrid = true;
        ColorGridCell(_gridScriptableObject.XFinalCellCoordinate, _gridScriptableObject.YFinalCellCoordinate, _gridScriptableObject.FinalCellColor);
        SpawnPlayer();
    }

    public IEnumerator DestroyGrid()
    {
        _delayFalling = _gridScriptableObject.InitialDelayFalling;

        for (int y = 0; y < _gridScriptableObject.Height; y++)
        {
            for (int x = 0; x < _gridScriptableObject.Width; x++)
            {
                GridCell _cell = _gameGrid[y, x].GetComponent<GridCell>();

                if (_cell != null && _cell != _gameManager._selectedCell)
                {
                    _cell.MakeMeFall(_gridScriptableObject.FallingSpeed);
                    yield return new WaitForSeconds(_delayFalling);
                }
            }
            _delayFalling -= _gridScriptableObject.SpeedUpFactorFalling;
        }

        HasCompletedFalling = true;
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



    private void SpawnPlayer()
    {
        if (_gridScriptableObject.SpawnCellXCoordinate >= 0 &&_gridScriptableObject.SpawnCellYCoordinate >= 0 && _gridScriptableObject.SpawnCellXCoordinate < _gridScriptableObject.Width &&_gridScriptableObject.SpawnCellYCoordinate < _gridScriptableObject.Height)
        {
            GameObject cellObject = _gameGrid[_gridScriptableObject.SpawnCellYCoordinate, _gridScriptableObject.SpawnCellXCoordinate];
            GridCell gridCell = cellObject.GetComponent<GridCell>();

            Instantiate(_gridScriptableObject.PlayerPrefab, gridCell.SpawnPoint.transform.position, Quaternion.identity);
        }
        else
        {
            throw new Exception("Invalid spawn coordinates for the player.");
        }
    }
}
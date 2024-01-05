using System;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class GameGrid : Singleton<GameGrid>
{
    [Header("Grid Settings")]
    [SerializeField] private GameObject _gridCellPrefab;
    [field: SerializeField] public int Height {get; private set;}
    [field: SerializeField] public int Width {get; private set;}
    [SerializeField] private float _gridSpaceSize = 5;
    [Space(5)]
    [SerializeField] private int _xFinalCellCordinate;
    [SerializeField] private int _yFinalCellCordinate;
    [SerializeField] private Color _finalCellColor;
    [field: SerializeField] public ColorCode SelectedCellColorCode {get; private set;}
    [Space(5)]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private int _spawnCellXCordinate; 
    [SerializeField] private int _spawnCellYCordinate;

    [Space(10)]
    [Header("Creation Settings")]
    [SerializeField] private float _initialDelay = 0.15f;
    [SerializeField] private float _speedUpFactor = 0.0075f;
    [field: SerializeField] public bool HasCompletedTheGrid { get; private set; }

    [Space(10)]
    [Header("Audio Settings")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _createClip;
    [Space(10)]
    [Header("Falling Settings")]
    [SerializeField] private float _fallingSpeed = 20f;
    [SerializeField] private float _initialDelayFalling = 0.15f;
    [SerializeField] private float _speedUpFactorFalling = 0.0075f;
    [field: SerializeField] public bool HasCompletedFalling { get; private set; }

    // Private Variables
    private GameObject[,] _gameGrid;
    private GameManager _gameManager;
    private ObjectPool<GameObject> _pool;
    private float _delay;
    private float _delayFalling;

    private void Awake() 
    {
        _pool = new ObjectPool<GameObject>(() => 
        {
            return Instantiate(_gridCellPrefab);
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

    private IEnumerator CreateGrid()
    {
        if (_gridCellPrefab == null)
        {
            throw new Exception("Grid Cell Prefab on the Game Grid has not been assigned");
        }

        _gameGrid = new GameObject[Height, Width];

        _delay = _initialDelay;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var newCell = _pool.Get();
                newCell.transform.position = new Vector3(x * _gridSpaceSize, 0, y * _gridSpaceSize);
                newCell.transform.parent = transform;
                newCell.name = $"Grid Space (X: {x}, Y: {y})";

                GridCell gridCell  = newCell.GetComponent<GridCell>();
                gridCell.SetPosition(y, x);
                gridCell.SetColumn(y);

                if (_audioSource != null && _createClip != null)
                {
                    _audioSource.PlayOneShot(_createClip);
                }

                _gameGrid[y, x] = newCell;

                yield return new WaitForSeconds(_delay); 
                _delay -= _speedUpFactor;
                _delay = Mathf.Max(_delay, 0f);
            }
        }

        HasCompletedTheGrid = true;
        ColorGridCell(_xFinalCellCordinate, _yFinalCellCordinate, _finalCellColor);
        SpawnPlayer();
    }

    public IEnumerator DestroyGrid()
    {
        _delayFalling = _initialDelayFalling;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                GridCell _cell = _gameGrid[y, x].GetComponent<GridCell>();

                if (_cell != null && _cell != _gameManager._selectedCell)
                {
                    _cell.MakeMeFall(_fallingSpeed);
                    yield return new WaitForSeconds(0.03f);
                    yield return new WaitForSeconds(_delayFalling);
                }
            }
            _delayFalling -= _speedUpFactorFalling;
        }

        HasCompletedFalling = true;
    }

    public void ColorGridCell(int x, int y, Color color)
    {
        if (x >= 0 && y >= 0 && x < _gameGrid.GetLength(1) && y < _gameGrid.GetLength(0))
        {
            GameObject cellToColor = _gameGrid[y, x];
            _gameManager._selectedCell = cellToColor.GetComponent<GridCell>();
            _gameManager.ChangeSelectedCellColor(SelectedCellColorCode);
            
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
        if (_spawnCellXCordinate >= 0 &&_spawnCellYCordinate >= 0 && _spawnCellXCordinate < Width &&_spawnCellYCordinate < Height)
        {
            GameObject cellObject = _gameGrid[_spawnCellYCordinate, _spawnCellXCordinate];
            GridCell gridCell = cellObject.GetComponent<GridCell>();

            Instantiate(_playerPrefab, gridCell.SpawnPoint.transform.position, Quaternion.identity);
        }
        else
        {
            throw new Exception("Invalid spawn coordinates for the player.");
        }
    }
}
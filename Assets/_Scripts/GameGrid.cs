using System;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class GameGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private GameObject _gridCellPrefab;
    [SerializeField] private int _height = 6;
    [SerializeField] private int _width = 3;
    [SerializeField] private int _xCordinate;
    [SerializeField] private int _yCordinate;
    [SerializeField] private Color _finalGridColor;
    [SerializeField] private float _gridSpaceSize = 5;

    [Header("Creation Settings")]
    [SerializeField] private float _initialDelay = 0.15f;
    [SerializeField] private float _speedUpFactor = 0.0075f;
    [field: SerializeField] public bool HasCompletedTheGrid { get; private set; }

    [Header("Audio Settings")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;

    // Private Variables
    private GameObject[,] _gameGrid;
    private ObjectPool<GameObject> _pool;
    private float _delay;

    private void Awake() 
    {
        _pool = new ObjectPool<GameObject>(() => {
            return Instantiate(_gridCellPrefab);
        }, gridCell =>{
            gridCell.gameObject.SetActive(true);
        }, gridCell =>{
            gridCell.gameObject.SetActive(false); 
        }, gridCell =>{
            Destroy(gridCell.gameObject); 
        }, false, 30, 60);
    }

    private void Start()
    {
        try
        {
            StartCoroutine(CreateGrid());
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

        _gameGrid = new GameObject[_height, _width];

        _delay = _initialDelay;

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                var newCell = _pool.Get();
                newCell.transform.position = new Vector3(x * _gridSpaceSize, 0, y * _gridSpaceSize);
                newCell.transform.parent = transform;
                newCell.name = $"Grid Space (X: {x}, Y: {y})";

                GridCell gridCell  = newCell.GetComponent<GridCell>();
                gridCell.SetPosition(y, x);
                gridCell.SetColumn(y);

                if (_audioSource != null && _audioClip != null)
                {
                    _audioSource.PlayOneShot(_audioClip);
                }

                _gameGrid[y, x] = newCell;

                yield return new WaitForSeconds(_delay); 
                _delay -= _speedUpFactor;
                _delay = Mathf.Max(_delay, 0f);
            }
        }

        ColorGridCell(_xCordinate, _yCordinate, _finalGridColor);
        HasCompletedTheGrid = true;
    }

    public void ColorGridCell(int x, int y, Color color)
    {
        if (x >= 0 && y >= 0 && x < _gameGrid.GetLength(1) && y < _gameGrid.GetLength(0))
        {
            GameObject cellToColor = _gameGrid[y, x];
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
            Debug.LogWarning($"Cell at position ({x}, {y}) does not exist in the grid or is out of bounds.");
        }
    }

    public Vector2Int GetGridPosFromWorld(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / _gridSpaceSize);
        int y = Mathf.FloorToInt(worldPosition.z / _gridSpaceSize);

        x = Mathf.Clamp(x, 0, _width - 1);
        y = Mathf.Clamp(y, 0, _height - 1);

        return new Vector2Int(x, y);
    }

    public Vector3 GetWorldPosFromGrid(Vector2Int gridPos)
    {
        int x = gridPos.x * Mathf.FloorToInt(_gridSpaceSize);
        int y = gridPos.y * Mathf.FloorToInt(_gridSpaceSize);

        return new Vector3(x, 0, y);
    }
}
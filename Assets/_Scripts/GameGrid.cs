using System;
using UnityEngine;
using System.Collections;

public class GameGrid : MonoBehaviour
{
    [SerializeField] private GameObject gridCellPrefab;

    [SerializeField] private int height = 6;
    [SerializeField] private int width = 3;
    [SerializeField] private float gridSpaceSize = 5;
    [SerializeField] private float initialDelay = 0.5f;
    [SerializeField] private float speedUpFactor = 0.95f;
    [field:SerializeField] public bool hasCompletedTheGrid {get; private set;}
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    private GameObject[,] gameGrid;
    private float delay;

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
        if (gridCellPrefab == null)
        {
            throw new Exception("Grid Cell Prefab on the Game Grid has not been assigned");
        }

        gameGrid = new GameObject[height, width];

        delay = initialDelay;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject newCell = Instantiate(gridCellPrefab, new Vector3(x * gridSpaceSize, 0, y * gridSpaceSize), Quaternion.identity);
                GridCell gridCell  = newCell.GetComponent<GridCell>();
                gridCell.SetPosition(y, x);
                newCell.transform.parent = transform;
                gridCell.SetColumn(y);
                newCell.name = $"Grid Space (X: {x}, Y: {y})";

                if (audioSource != null && audioClip != null)
                {
                    audioSource.PlayOneShot(audioClip);
                }

                gameGrid[y, x] = newCell;

                yield return new WaitForSeconds(delay); 
                delay -= speedUpFactor;
                delay = Mathf.Max(delay, 0f);
            }
        }

        hasCompletedTheGrid = true;
    }

    public Vector2Int GetGridPosFromWorld(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / gridSpaceSize);
        int y = Mathf.FloorToInt(worldPosition.z / gridSpaceSize);

        x = Mathf.Clamp(x, 0, width - 1);
        y = Mathf.Clamp(y, 0, height - 1);

        return new Vector2Int(x, y);
    }

    public Vector3 GetWorldPosFromGrid(Vector2Int gridPos)
    {
        int x = gridPos.x * Mathf.FloorToInt(gridSpaceSize);
        int y = gridPos.y * Mathf.FloorToInt(gridSpaceSize);

        return new Vector3(x, 0, y);
    }
}
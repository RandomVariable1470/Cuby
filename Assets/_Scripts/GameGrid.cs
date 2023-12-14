using System;
using UnityEngine;
using System.Collections;

public class GameGrid : MonoBehaviour
{
    [SerializeField] private GameObject gridCellPrefab;

    [SerializeField] private int height = 6;
    [SerializeField] private int width = 3;
    [SerializeField] private float gridSpaceSize = 5f;
    [SerializeField] private float delayTime = 0.01f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    private GameObject[,] gameGrid;

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

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject newCell = Instantiate(gridCellPrefab, new Vector3(x * gridSpaceSize, 0, y * gridSpaceSize), Quaternion.identity);
                newCell.GetComponent<GridCell>().SetPosition(x, y);
                newCell.transform.parent = transform;
                newCell.name = $"Grid Space (X: {x}, Y: {y})";

                if (audioSource != null && audioClip != null)
                {
                    audioSource.PlayOneShot(audioClip);
                }

                gameGrid[y, x] = newCell;

                yield return new WaitForSeconds(delayTime);
            }
        }
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
        int x = gridPos.x * (int)gridSpaceSize;
        int y = gridPos.y * (int)gridSpaceSize;

        return new Vector3(x, 0, y);
    }
}
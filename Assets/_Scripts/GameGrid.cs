using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField] private GameObject gridCellPrefab;

    [SerializeField] private int height = 6;
    [SerializeField] private int width = 3;
    [SerializeField] private int gridSpaceSize = 5;
    [SerializeField] private float delayTime = 0.01f; 
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    private GameObject[,] gameGrid;

    private void Start() 
    {
        StartCoroutine(CreateGrid());
    }

    private IEnumerator CreateGrid()
    {
        gameGrid = new GameObject[height, width];

        if (gridCellPrefab == null)
        {
            Debug.LogError("ERROR: Grid Cell Prefab on the Game Grid has not been assigned");
            yield return null;
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                gameGrid[y, x] = Instantiate(gridCellPrefab, new Vector3(x * gridSpaceSize, 0, y * gridSpaceSize), Quaternion.identity);
                gameGrid[y, x].GetComponent<GridCell>().SetPosition(x, y);
                gameGrid[y, x].transform.parent = transform; 
                gameGrid[y, x].gameObject.name = "Grid Space ( X: " + x.ToString() + ", Y: " + y.ToString() + ") ";

                // Ensure audio source and clip are not null before playing audio
                if (audioSource != null && audioClip != null)
                {
                    audioSource.PlayOneShot(audioClip);
                    yield return new WaitForSeconds(delayTime);
                }
            }
        }
    }

    // Gets Grid Position From World Position
    public Vector2Int GetGridPosFromWorld(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / gridSpaceSize);
        int y = Mathf.FloorToInt(worldPosition.z / gridSpaceSize);

        x = Mathf.Clamp(x, 0, width - 1);
        y = Mathf.Clamp(y, 0, height - 1);

        return new Vector2Int(x, y);
    }

    // Gets The World Position Of A Grid Position
    public Vector3 GetWorldPosFromGrid(Vector2Int gridPos)
    {
        int x = gridPos.x * gridSpaceSize;
        int y = gridPos.y * gridSpaceSize;

        return new Vector3(x, 0, y);
    }
}

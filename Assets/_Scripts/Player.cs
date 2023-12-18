using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float value = 9.81f; 

    private GameGrid gameGrid;
    private Vector2Int currentPlayerPosition;
    private Transform playerTransform;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameGrid = GameGrid.Instance;
        currentPlayerPosition = new Vector2Int(0, 0);
        playerTransform = transform;
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(0, -1.0f, 0) * rb.mass * value, ForceMode.Acceleration);
    }

    public void OnSwipeDetected(SwipeDirection direction)
    {
        Vector2Int targetPosition = GetTargetGridCell(direction);

        if (IsValidGridCell(targetPosition))
        {
            StartCoroutine(MovePlayerToGridCell(targetPosition));
            RotatePlayerCube(direction);
        }
    }

    private Vector2Int GetTargetGridCell(SwipeDirection direction)
{
    switch (direction)
    {
        case SwipeDirection.Left:
            return new Vector2Int(currentPlayerPosition.x + 1, currentPlayerPosition.y);
        case SwipeDirection.Right:
            return new Vector2Int(currentPlayerPosition.x - 1, currentPlayerPosition.y);
        case SwipeDirection.Up:
            return new Vector2Int(currentPlayerPosition.x, currentPlayerPosition.y - 1);
        case SwipeDirection.Down:
            return new Vector2Int(currentPlayerPosition.x, currentPlayerPosition.y + 1);
        default:
            return currentPlayerPosition;
    }
}

    private bool IsValidGridCell(Vector2Int position)
    {
        return position.x >= 0 && position.x < gameGrid._width && position.y >= 0 && position.y < gameGrid._height;
    }

    private IEnumerator MovePlayerToGridCell(Vector2Int position)
    {
        Vector3 targetSpawnPoint = gameGrid.GetWorldPosFromGrid(position);
        Vector3 targetPlayerPosition = targetSpawnPoint + new Vector3(2.5f, 8.5f, 2.5f); // Adjust the height offset as needed

        Vector3 startPosition = transform.position;
        float journeyLength = Vector3.Distance(startPosition, targetPlayerPosition);
        float journeyTime = 0.5f; // Adjust this duration as needed

        float startTime = Time.time;
        while (Time.time < startTime + journeyTime)
        {
            float distanceCovered = (Time.time - startTime) * journeyLength / journeyTime;
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, targetPlayerPosition, fractionOfJourney);
            yield return null;
        }

        transform.position = targetPlayerPosition;
        currentPlayerPosition = position;
    }

    private void RotatePlayerCube(SwipeDirection direction)
    {
        switch (direction)
        {
            case SwipeDirection.Left:
                playerTransform.rotation = Quaternion.Euler(0, -90, 0); // Rotate on Y-axis (-90 degrees)
                break;
            case SwipeDirection.Right:
                playerTransform.rotation = Quaternion.Euler(0, 90, 0); // Rotate on Y-axis (90 degrees)
                break;
            case SwipeDirection.Up:
                playerTransform.rotation = Quaternion.Euler(-90, 0, 0); // Rotate on X-axis (-90 degrees)
                break;
            case SwipeDirection.Down:
                playerTransform.rotation = Quaternion.Euler(90, 0, 0); // Rotate on X-axis (90 degrees)
                break;
            default:
                break;
        }
    }
}
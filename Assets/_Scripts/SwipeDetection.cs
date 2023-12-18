using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetection : Singleton<SwipeDetection>
{
    [SerializeField] private float _minimumDistance = 0.2f;
    [SerializeField] private float _maximumTime = 1f;
    [SerializeField, Range(0f,1f)] private float _directionThreshold = .9f;
    [field:SerializeField] public SwipeDirection swipeDirection {get; private set;}

    private InputManager _inputManager;

    private Vector2 _startPosition;
    private float _startTime;

    private Vector2 _endPosition;
    private float _endTime;
    
    [HideInInspector] public Player _player;

    private bool isSwipeDetected = false;

    private void Awake() 
    {
        _inputManager = InputManager.Instance;
    }

    private void OnEnable() 
    {
        _inputManager.OnStartTouch += SwipeStart;
        _inputManager.OnEndTouch += SwipeEnd;
    }

    private void OnDisable() 
    {
        _inputManager.OnStartTouch -= SwipeStart;
        _inputManager.OnEndTouch -= SwipeEnd;
    }

    private void SwipeStart(Vector2 position, float time)
    {
        _startPosition = position;
        _startTime = time;
        isSwipeDetected = false; // Reset the flag when a new swipe starts
    }

    private void SwipeEnd(Vector2 position, float time)
    {
        if (!isSwipeDetected) // Check if a swipe hasn't been detected yet
        {
            _endPosition = position;
            _endTime = time;
            DetectSwipe();
        }
    }

    private void DetectSwipe()
    {
        if (Vector3.Distance(_startPosition, _endPosition) >= _minimumDistance && (_endTime - _startTime) <= _maximumTime)
        {
            Vector3 direction = _endPosition - _startPosition;
            Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
            SwipeDirection(direction2D);
            isSwipeDetected = true; // Set the flag to indicate a swipe was detected
        }
    }

    private void SwipeDirection(Vector2 dir)
    {
        float up = Vector2.Dot(Vector2.up, dir);
        float down = Vector2.Dot(Vector2.down, dir);
        float left = Vector2.Dot(Vector2.left, dir);
        float right = Vector2.Dot(Vector2.right, dir);

        if (up > _directionThreshold && up >= down && up >= left && up >= right)
        {
            swipeDirection = global::SwipeDirection.Up;
        }
        else if (down > _directionThreshold && down >= up && down >= left && down >= right)
        {
            swipeDirection = global::SwipeDirection.Down;
        }
        else if (left > _directionThreshold && left >= up && left >= down && left >= right)
        {
            swipeDirection = global::SwipeDirection.Right;
        }
        else if (right > _directionThreshold && right >= up && right >= down && right >= left)
        {
            swipeDirection = global::SwipeDirection.Left;
        }
        
        _player.OnSwipeDetected(swipeDirection);
    }
}

public enum SwipeDirection
{
    Up,
    Down,
    Right,
    Left
}
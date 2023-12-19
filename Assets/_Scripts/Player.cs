using System.Collections;
using System.Collections.Generic;
using GG.Infrastructure.Utils.Swipe;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _range = 10f;
    [SerializeField] private Vector3 _leftOffset;
    [SerializeField] private Vector3 _rightOffset;
    [SerializeField] private Vector3 _backOffset;
    [SerializeField] private Vector3 _frontOffset;
    [SerializeField] private float _gravity = 40f;

    private Rigidbody _rb;

    private SwipeListener swipeListener;
    private GameGrid _gameGrid;
    private GridCell _gridCellRight;
    private GridCell _gridCellLeft;
    private GridCell _gridCellBack;
    private GridCell _gridCellFront;

    private void Awake()
    {
        swipeListener = SwipeListener.Instance;
        _gameGrid = GameGrid.Instance;
    }

    private void Start() 
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        swipeListener.OnSwipe.AddListener(OnSwipe);
    }

    private void OnDisable()
    {
        swipeListener.OnSwipe.RemoveListener(OnSwipe);
    }

    private void FixedUpdate() 
    {
        _rb.AddForce(Vector3.down * _gravity, ForceMode.Force);
    }

    private void Update()
    {
        RaycastWork();
    }

    private void RaycastWork()
    {
        RaycastHit hitLeft, hitRight, hitBack, hitFront;
        
        bool leftHit = Physics.Raycast(transform.position + _leftOffset, Vector3.down, out hitLeft, _range);
        bool rightHit = Physics.Raycast(transform.position + _rightOffset, Vector3.down, out hitRight, _range);
        bool backHit = Physics.Raycast(transform.position + _backOffset, Vector3.down, out hitBack, _range);
        bool frontHit = Physics.Raycast(transform.position + _frontOffset, Vector3.down , out hitFront, _range);
        
        if (leftHit)
        {
            _gridCellLeft = hitLeft.transform.gameObject.GetComponent<GridCell>();
        }
        if (rightHit)
        {
            _gridCellRight = hitRight.transform.gameObject.GetComponent<GridCell>();
        }
        if (backHit)
        {
            _gridCellBack = hitBack.transform.gameObject.GetComponent<GridCell>();
        }
        if (frontHit)
        {
            _gridCellFront = hitFront.transform.gameObject.GetComponent<GridCell>();
        }
    }

    private void OnSwipe(string swipe)
    {
        switch (swipe)
        {
            case "Up":
                break;

            case "Down":
                break;

            case "Right":
                break;

            case "Left":
                break;
            
            default:
                break;
        }
    }
}
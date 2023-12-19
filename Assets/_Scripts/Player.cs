using System.Collections;
using System.Collections.Generic;
using GG.Infrastructure.Utils.Swipe;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _range = 10f;
    [SerializeField] private AnimationCurve _bounceCurve;
    [SerializeField] private Vector3 _leftOffset;
    [SerializeField] private Vector3 _rightOffset;
    [SerializeField] private Vector3 _backOffset;
    [SerializeField] private Vector3 _frontOffset;
    [SerializeField] private float _gravity = 40f;
    [SerializeField] private LayerMask _gridCellLayerMask;

    private Rigidbody _rb;

    private SwipeListener swipeListener;

    private GridCell _gridCellRight;
    private GridCell _gridCellLeft;
    private GridCell _gridCellBack;
    private GridCell _gridCellFront;

    private Transform _rightCellPoint;
    private Transform _leftCellPoint;
    private Transform _backCellPoint;
    private Transform _frontCellPoint;

    private bool isMoving = false;
    private float jumpDuration = 0.5f;

    private void Awake()
    {
        swipeListener = SwipeListener.Instance;
    }

    private void Start() 
    {
        _rb = GetComponent<Rigidbody>();
        ApplyExtraGravity();
    }

    private void OnEnable()
    {
        swipeListener.OnSwipe.AddListener(OnSwipe);
    }

    private void OnDisable()
    {
        swipeListener.OnSwipe.RemoveListener(OnSwipe);
    }

    private void Update()
    {
        RaycastWork();
        AssignCellPoints();
    }

    private void RaycastWork()
    {
        RaycastHit hitLeft, hitRight, hitBack, hitFront;
        
        bool leftHit = Physics.Raycast(transform.position + _leftOffset, Vector3.down, out hitLeft, _range, _gridCellLayerMask);
        bool rightHit = Physics.Raycast(transform.position + _rightOffset, Vector3.down, out hitRight, _range, _gridCellLayerMask);
        bool backHit = Physics.Raycast(transform.position + _backOffset, Vector3.down, out hitBack, _range, _gridCellLayerMask);
        bool frontHit = Physics.Raycast(transform.position + _frontOffset, Vector3.down , out hitFront, _range, _gridCellLayerMask);
        
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

    private void AssignCellPoints()
    {
        if (_gridCellLeft != null)
        {
            _leftCellPoint = _gridCellLeft.SpawnPoint.transform;
        }
        if (_gridCellRight != null)
        {
            _rightCellPoint = _gridCellRight.SpawnPoint.transform;
        }
        if (_gridCellFront != null)
        {
            _frontCellPoint = _gridCellFront.SpawnPoint.transform;
        }
        if (_gridCellBack != null)
        {
            _backCellPoint = _gridCellBack.SpawnPoint.transform;
        }
    }

    private void MakePlayerJumpToCell(Transform cell)
    {
        if (cell != null && !isMoving)
        {
            StartCoroutine(JumpToCell(cell.position));
        }
    }

    private IEnumerator JumpToCell(Vector3 targetPosition)
    {
        isMoving = true;
        Vector3 initialPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;
            transform.position = Vector3.Lerp(initialPosition, targetPosition, _bounceCurve.Evaluate(t));
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }

    private void ApplyExtraGravity()
    {
        _rb.AddForce(Vector3.down * _gravity, ForceMode.Force);
    }

    private void OnSwipe(string swipe)
    {
        switch (swipe)
        {
            case "Up":
                if (_gridCellBack != null)
                {
                    MakePlayerJumpToCell(_backCellPoint);
                }
                break;

            case "Down":
                if (_gridCellFront != null)
                {
                    MakePlayerJumpToCell(_frontCellPoint);
                }
                break;

            case "Right":
                if (_gridCellLeft != null)
                {
                    MakePlayerJumpToCell(_leftCellPoint);
                }
                break;

            case "Left":
                if (_gridCellRight != null)
                {
                    MakePlayerJumpToCell(_rightCellPoint);
                }
                break;

            default:
                break;
        }
    }
}
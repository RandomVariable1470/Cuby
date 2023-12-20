using System.Collections;
using System.Collections.Generic;
using GG.Infrastructure.Utils.Swipe;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f; 
    [SerializeField] private float _jumpHeight = 2f;
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
    private Vector3 currentRotation = Vector3.zero;
    private bool isRotating = false;


    private void Awake()
    {
        swipeListener = SwipeListener.Instance;
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

    private void Update()
    {
        RaycastWork();
        AssignCellPoints();
    }

    private void FixedUpdate() 
    {
        ApplyExtraGravity();
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(transform.localPosition + new Vector3(0f, -1.3f, 0f), .2f, _gridCellLayerMask);
    }

    private void ApplyExtraGravity()
    {
        if (!IsGrounded())
        {
            _rb.AddForce(Vector3.down * _gravity, ForceMode.Force);
        }
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
        else
        {
            _gridCellLeft = null;
        }
        if (rightHit)
        {
            _gridCellRight = hitRight.transform.gameObject.GetComponent<GridCell>();
        }
        else
        {
            _gridCellRight = null;
        }
        if (backHit)
        {
            _gridCellBack = hitBack.transform.gameObject.GetComponent<GridCell>();
        }
        else
        {
            _gridCellBack = null;
        }
        if (frontHit)
        {
            _gridCellFront = hitFront.transform.gameObject.GetComponent<GridCell>();
        }
        else
        {
            _gridCellFront = null;
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
            StartCoroutine(MoveToCell(cell.position));
        }
    }

    private IEnumerator MoveToCell(Vector3 targetPosition)
    {
        isMoving = true;
        Vector3 initialPosition = transform.position;

        float jumpHeight = _jumpHeight;
        float jumpDuration = Mathf.Sqrt(2f * jumpHeight / _gravity);

        float distance = Vector3.Distance(initialPosition, targetPosition);
        float moveDuration = distance / _moveSpeed;

        float duration = Mathf.Max(jumpDuration, moveDuration);
        float elapsedTime = 0f;

        Vector3 jumpVelocity = Vector3.up * Mathf.Sqrt(2f * _gravity * jumpHeight);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / duration);

            Vector3 newPos = Vector3.Lerp(initialPosition, targetPosition, t);

            if (elapsedTime < jumpDuration)
            {
                float yOffset = jumpVelocity.y * elapsedTime - 0.5f * _gravity * elapsedTime * elapsedTime;
                newPos.y = initialPosition.y + yOffset;
            }

            transform.position = newPos;

            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }


    private void OnSwipe(string swipe)
    {
        if (!IsGrounded()) return;

        Vector3 rotation = Vector3.zero;

        switch (swipe)
        {
            case "Up":
                if (_gridCellFront != null)
                {
                    MakePlayerJumpToCell(_frontCellPoint);
                    rotation = new Vector3(-90f, 0f, 0f);
                }
                break;

            case "Down":

                 if (_gridCellBack != null)
                {
                    MakePlayerJumpToCell(_backCellPoint);
                    rotation = new Vector3(90f, 0f, 0f);
                }
                break;

            case "Right":
                if (_gridCellLeft != null)
                {
                    MakePlayerJumpToCell(_leftCellPoint);
                    rotation = new Vector3(0f, 0f, -90f);
                }
                break;

            case "Left":
                if (_gridCellRight != null)
                {
                    MakePlayerJumpToCell(_rightCellPoint);
                    rotation = new Vector3(0f, 0f, 90f);
                }
                break;

            default:
                break;
        }

        currentRotation += rotation;

        StartCoroutine(RotateTo(Quaternion.Euler(currentRotation), 0.5f));
    }

    private IEnumerator RotateTo(Quaternion targetRotation, float duration)
{
    Quaternion initialRotation = transform.rotation;
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);
        transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);
        yield return null;
    }

    transform.rotation = targetRotation;
}
}
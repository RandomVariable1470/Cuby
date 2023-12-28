using System.Collections;
using System.Collections.Generic;
using GG.Infrastructure.Utils.Swipe;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public enum ColorCode
{
    Red,
    Green,
    Blue,
    Yellow,
    Cyan,
    Orange
}

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpHeight = 2f;
    [SerializeField] private float _range = 10f;
    [SerializeField] private float _groundCheck;

    [Space(5)]
    [Header("Audio & Effects")]
    [SerializeField] private AudioClip _spawnSound;
    [SerializeField] private AudioClip _swipSound;
    [Space(2)]
    [SerializeField] private AnimationCurve _bounceCurve;
    [Space(2)]
    [SerializeField] private GameObject _jumpParticle;
    [SerializeField] private GameObject _landParticle;

    [Space(5)]
    [Header("Offsets")]
    [SerializeField] private Vector3 _leftOffset;
    [SerializeField] private Vector3 _rightOffset;
    [SerializeField] private Vector3 _backOffset;
    [SerializeField] private Vector3 _frontOffset;

    [Space(5)]
    [Header("Ground Check and Physics")]
    [SerializeField] private float _gravity = 40f;
    [SerializeField] private LayerMask _gridCellLayerMask;

    //PrivateVariables
    [HideInInspector] public ColorCode colorBelow;

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

    private bool _isMoving = false;
    private bool _isRotating = false;
    private bool _wasMoving;
    private Quaternion _targetRotation;
    private AudioSource _audioSource;
    private CinemachineShake _shake;
    private GameManager _gameManager;

    #region Initilization

    private void Awake()
    {
        swipeListener = SwipeListener.Instance;
        _gameManager = GameManager.Instance;
    }

    private void Start() 
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _shake = CinemachineShake.Instance;
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
        SwipeEffects();

        _gameManager.colorText.text = $"Color: {colorBelow}";
    }

    private void FixedUpdate() 
    {
        ApplyExtraGravity();
    }

    #endregion

    #region Movement Handling

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _groundCheck, _gridCellLayerMask);
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
        if (cell != null && !_isMoving)
        {
            _wasMoving = true;
            StartCoroutine(MoveToCell(cell.position));
        }
    }

    private IEnumerator MoveToCell(Vector3 targetPosition)
    {
        _isMoving = true;

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

            yield return new WaitForFixedUpdate();
        }

        transform.position = targetPosition;

        _isMoving = false;
    }

    #endregion

    #region Swipe Handling

    private void OnSwipe(string swipe)
    {
        if (!IsGrounded()) return;

        switch (swipe)
        {
            case "Up":
                if (_gridCellFront != null)
                {
                    MakePlayerJumpToCell(_frontCellPoint);
                    RotateCubeSmoothly(Quaternion.Euler(90, 0, 0));
                }
                break;

            case "Down":
                if (_gridCellBack != null)
                {
                    MakePlayerJumpToCell(_backCellPoint);
                    RotateCubeSmoothly(Quaternion.Euler(-90, 0, 0));
                }
                break;

            case "Right":
                if (_gridCellLeft != null)
                {
                    MakePlayerJumpToCell(_leftCellPoint);
                    RotateCubeSmoothly(Quaternion.Euler(0, 0, 90));
                }
                break;

            case "Left":
                if (_gridCellRight != null)
                {
                    MakePlayerJumpToCell(_rightCellPoint);
                    RotateCubeSmoothly(Quaternion.Euler(0, 0, -90));
                }
                break;

            default:
                break;
        }
    }

    #endregion

    #region Rotation Handling

    private void RotateCubeSmoothly(Quaternion targetRotation)
    {
        this._targetRotation = targetRotation;
        StartCoroutine(SmoothRotation());
    }

    private IEnumerator SmoothRotation()
    {
        if (_isRotating)
        {
            yield break;
        }

        _isRotating = true;

        Quaternion startRotation = transform.rotation;
        Quaternion previousTarget = transform.rotation;

        float elapsedTime = 0f;
        float rotationDuration = 0.3f;

        while (elapsedTime < rotationDuration)
        {
            if (!_isRotating)
            {
                yield break;
            }

            transform.rotation = Quaternion.Slerp(startRotation, _targetRotation * previousTarget, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        transform.rotation = _targetRotation * previousTarget;
        _isRotating = false;
    }

    #endregion

    #region Audio & Effects

    private void SwipeEffects()
    {
        if (_wasMoving && IsGrounded() && !_isMoving)
        {
            _audioSource.PlayOneShot(_swipSound);

            _wasMoving = false;

            _shake.ShakeCamera(1f, 0.1f);

            //GameObject jumpParticle = Instantiate(_jumpParticle, transform.position, Quaternion.identity);
            //GameObject landParticle = Instantiate(_landParticle, transform.position + new Vector3(0f, -2.5f, 0f), Quaternion.identity);
        }
    }

    public void OnSpawnSound()
    {
        _audioSource.PlayOneShot(_spawnSound);
    }

    #endregion

    #region ColorHandling

    public void ChangeColor(ColorCode _color)
    {
        colorBelow = _color;
    }

    #endregion
    
}  
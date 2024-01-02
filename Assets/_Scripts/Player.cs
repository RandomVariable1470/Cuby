using System.Collections;
using GG.Infrastructure.Utils.Swipe;
using UnityEngine;
using UnityEngine.Pool;

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
    [SerializeField] private float _range = 10f;
    [SerializeField] private float _groundCheck;

    [Space(5)]
    [Header("Audio & Effects")]
    [SerializeField] private AudioClip _spawnSound;
    [SerializeField] private AudioClip _swipSound;
    [Space(2)]
    [SerializeField] private GameObject[] _jumpParticle;
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

    [Space(5)]
    [Header("Color")]
    [SerializeField] private Color _redColor;
    [SerializeField] private Color _cyanColor;
    [SerializeField] private Color _yellowColor;
    [SerializeField] private Color _blueColor;
    [SerializeField] private Color _greenColor;
    [SerializeField] private Color _orangeColor;
    [Space(5)]
    [SerializeField] private float _downDistanceColor;
    [SerializeField] private LayerMask _colorCellMask;
    public ColorCode colorBelow;

    //Private Variables

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
    private bool _canSwipe = true;
    private float _swipeCooldownTime = 0.65f;
    private float _swipeCooldownTimer = 0f;

    private Quaternion _targetRotation;
    private AudioSource _audioSource;
    private Rigidbody _rb;
    private CinemachineShake _shake;
    private ParticleSystem.MinMaxGradient _currentParticleGradient;

    #region Initilization

    private void Awake()
    {
        swipeListener = SwipeListener.Instance;
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
        SwipeCounter();
    }

    private void FixedUpdate() 
    {
        ApplyExtraGravity();
    }

    #endregion

    #region Movement Handling & Raycast Work

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _groundCheck, _gridCellLayerMask);
    }

    private void ApplyExtraGravity()
    {
        if (!IsGrounded())
        {
            _rb.AddForce(_gravity * Vector3.down * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }

    private void RaycastWork()
    {
        RaycastHit hit;

        bool hitLeft = Physics.Raycast(transform.position + _leftOffset, Vector3.down, out hit, _range, _gridCellLayerMask);
        _gridCellLeft = hitLeft ? hit.transform.gameObject.GetComponent<GridCell>() : null;

        bool hitRight = Physics.Raycast(transform.position + _rightOffset, Vector3.down, out hit, _range, _gridCellLayerMask);
        _gridCellRight = hitRight ? hit.transform.gameObject.GetComponent<GridCell>() : null;

        bool hitBack = Physics.Raycast(transform.position + _backOffset, Vector3.down, out hit, _range, _gridCellLayerMask);
        _gridCellBack = hitBack ? hit.transform.gameObject.GetComponent<GridCell>() : null;

        bool hitFront = Physics.Raycast(transform.position + _frontOffset, Vector3.down, out hit, _range, _gridCellLayerMask);
        _gridCellFront = hitFront ? hit.transform.gameObject.GetComponent<GridCell>() : null;
    }

    private void AssignCellPoints()
    {
        _leftCellPoint = _gridCellLeft?.JumpPoint?.transform;
        _rightCellPoint = _gridCellRight?.JumpPoint?.transform;
        _frontCellPoint = _gridCellFront?.JumpPoint?.transform;
        _backCellPoint = _gridCellBack?.JumpPoint?.transform;
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
        float distance = Vector3.Distance(initialPosition, targetPosition);
        float moveDuration = distance / _moveSpeed;

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            Vector3 newPos = Vector3.Lerp(initialPosition, targetPosition, t);
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
        if (!_canSwipe || !IsGrounded()) return;

        _canSwipe = false;
        _swipeCooldownTimer = 0f; 

        SpawnJumparticle();

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

    private void SwipeCounter()
    {
        if (!_canSwipe)
        {
            _swipeCooldownTimer += Time.deltaTime;

            if (_swipeCooldownTimer >= _swipeCooldownTime)
            {
                _canSwipe = true;
                _swipeCooldownTimer = 0f; 
            }
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
        float rotationDuration = 0.35f;

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
            Invoke(nameof(SpawnLandParticle), 0.1f);
        }
    }

    public void OnSpawnSound()
    {
        _audioSource.PlayOneShot(_spawnSound);
    }

    private void SpawnLandParticle()
    {
        DetectColor();

        GameObject landParticle = Instantiate(_landParticle, transform.position + new Vector3(0f, -1.75f, 0f), _landParticle.transform.rotation);

        ParticleSystem ps = landParticle.GetComponent<ParticleSystem>();

        SetColor(ps);

        ps.Play();

        Destroy(landParticle, 1.0f);
    }

    private void SpawnJumparticle()
    {
        DetectColor();

        foreach(GameObject ps in _jumpParticle)
        {
            GameObject _ps = Instantiate(ps, transform.position + new Vector3(0f, -1.75f, 0f), ps.transform.rotation);

            ParticleSystem particleSystem = _ps.GetComponent<ParticleSystem>();
            particleSystem.Play();

            SetColor(particleSystem);

            Destroy(_ps, 1.0f);
        }
    }

    private void DetectColor()
    {
        switch (colorBelow)
        {
            case ColorCode.Red:
                var color = _redColor;
                _currentParticleGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
                break;
            
            case ColorCode.Green:
                color = _greenColor;
                _currentParticleGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
                break;
            
            case ColorCode.Cyan:
                color = _cyanColor;
                _currentParticleGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
                break;
            
            case ColorCode.Blue:
                color = _blueColor;
                _currentParticleGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
                break;
            
            case ColorCode.Orange:
                color = _orangeColor;
                _currentParticleGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
                break;
            
            case ColorCode.Yellow:
                color = _yellowColor;
                _currentParticleGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
                break;

            default:
                break;
        }
    }

    private void SetColor(ParticleSystem ps)
    {
        var main = ps.main;
        main.startColor = _currentParticleGradient;
    }

    #endregion

    #region ColorHandling

    private void ChangeColor(ColorCode _color)
    {
        colorBelow = _color;
    }

    public void ChangeColor(string name)
    {
        switch(name)
        {
            case GREEN_TAG:
                ChangeColor(ColorCode.Green);
                break;
            case CYAN_TAG:
                ChangeColor(ColorCode.Cyan);
                break;
            case RED_TAG:
                ChangeColor(ColorCode.Red);
                break;
            case BLUE_TAG:
                ChangeColor(ColorCode.Blue);
                break;
            case YELLOW_TAG:
                ChangeColor(ColorCode.Yellow);
                break;
            
            case ORANGE_TAG:
                ChangeColor(ColorCode.Orange);
                break;
            default:
                break;
        }
    }

    #endregion
    
    #region Cached Properties

    private const string GREEN_TAG = "Green";
    private const string CYAN_TAG = "Cyan";
    private const string RED_TAG = "Red";
    private const string BLUE_TAG = "Blue";
    private const string YELLOW_TAG = "Yellow";
    private const string ORANGE_TAG = "Orange";

    #endregion
}  
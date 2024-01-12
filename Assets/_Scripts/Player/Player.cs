using System.Collections;
using GG.Infrastructure.Utils.Swipe;
using UnityEngine;

public enum ColorCode
{
    Red,
    Green,
    Blue,
    Yellow,
    Cyan,
    Orange,
    None
}

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerScriptable _player;

    [HideInInspector] public Animator Animator;

    //Private Variables
    private SwipeListener swipeListener;

    private GridCell _gridCellRight, _gridCellLeft, _gridCellBack, _gridCellFront;

    private Transform _rightCellPoint, _leftCellPoint, _backCellPoint, _frontCellPoint;

    private bool _isMoving = false;
    private bool _isRotating = false;
    private bool _wasMoving;
    private bool _canSwipe = true;
    private float _swipeCooldownTimer = 0f;

    private Quaternion _targetRotation;
    private AudioSource _audioSource;
    private Rigidbody _rb;
    private Collider _coll;
    private GameManager _gameManager;
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
        _gameManager = GameManager.Instance;
        Animator = GetComponent<Animator>();
        _coll = GetComponent<Collider>();
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
        return Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _player.GroundCheck, _player.GridCellLayerMask);
    }

    private void ApplyExtraGravity()
    {
        if (!IsGrounded())
        {
            _rb.AddForce(_player.Gravity * Vector3.down * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }

    private void RaycastWork()
    {
        RaycastHit hit;

        bool hitLeft = Physics.Raycast(transform.position + _player.LeftOffset, Vector3.down, out hit, _player.Range, _player.GridCellLayerMask);
        _gridCellLeft = hitLeft ? hit.transform.gameObject.GetComponent<GridCell>() : null;

        bool hitRight = Physics.Raycast(transform.position + _player.RightOffset, Vector3.down, out hit, _player.Range, _player.GridCellLayerMask);
        _gridCellRight = hitRight ? hit.transform.gameObject.GetComponent<GridCell>() : null;

        bool hitBack = Physics.Raycast(transform.position + _player.BackOffset, Vector3.down, out hit, _player.Range, _player.GridCellLayerMask);
        _gridCellBack = hitBack ? hit.transform.gameObject.GetComponent<GridCell>() : null;

        bool hitFront = Physics.Raycast(transform.position + _player.FrontOffset, Vector3.down, out hit, _player.Range, _player.GridCellLayerMask);
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
        float moveDuration = distance / _player.MoveSpeed;

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
        if (!_canSwipe || !IsGrounded() || _gameManager.HasCompletedGame) return;

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

            if (_swipeCooldownTimer >= _player.SwipeCooldownTime)
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
            AudioManager.Instance.PlaySfx("OnLand");

            _wasMoving = false;
            Invoke(nameof(SpawnLandParticle), 0.1f);
        }
    }

    public void OnSpawnSound()
    {
        AudioManager.Instance.PlaySfx("Spawn");
    }

    public void OnDeSpawnSound()
    {
        AudioManager.Instance.PlaySfx("DeSpawn");
    }

    public void DisableColl()
    {
        _coll.enabled = false;
    }

    private void SpawnLandParticle()
    {
        DetectColor();

        GameObject landParticle = Instantiate(_player.LandParticle, transform.position + new Vector3(0f, -1.75f, 0f), _player.LandParticle.transform.rotation);

        ParticleSystem ps = landParticle.GetComponent<ParticleSystem>();

        SetColor(ps);

        ps.Play();

        Destroy(landParticle, 1.0f);
    }

    private void SpawnJumparticle()
    {
        DetectColor();

        foreach(GameObject ps in _player.JumpParticle)
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
        switch (_gameManager.ColorCode)
        {
            case ColorCode.Red:
                var color = _player.RedColor;
                _currentParticleGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
                break;
            
            case ColorCode.Green:
                color = _player.GreenColor;
                _currentParticleGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
                break;
            
            case ColorCode.Cyan:
                color = _player.CyanColor;
                _currentParticleGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
                break;
            
            case ColorCode.Blue:
                color = _player.BlueColor;
                _currentParticleGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
                break;
            
            case ColorCode.Orange:
                color = _player.OrangeColor;
                _currentParticleGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
                break;
            
            case ColorCode.Yellow:
                color = _player.YellowColor;
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

    public void ChangeColor(string name)
    {
        switch(name)
        {
            case GREEN_TAG:
                _gameManager.ChangeColor(ColorCode.Green);
                break;
            case CYAN_TAG:
                _gameManager.ChangeColor(ColorCode.Cyan);
                break;
            case RED_TAG:
                _gameManager.ChangeColor(ColorCode.Red);
                break;
            case BLUE_TAG:
                _gameManager.ChangeColor(ColorCode.Blue);
                break;
            case YELLOW_TAG:
                _gameManager.ChangeColor(ColorCode.Yellow);
                break;
            
            case ORANGE_TAG:
                _gameManager.ChangeColor(ColorCode.Orange);
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
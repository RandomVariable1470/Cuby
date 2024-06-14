using System.Collections;
using System.Collections.Generic;
using RV.Systems;
using RV.Systems.AudioSystem;
using RV.Grid;
using RV.Configs;
using RV.Util.Extensions;
using UnityEngine;

namespace RV.Player
{
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
        [SerializeField] private PlayerScriptable playerSO;
    
        [HideInInspector] public Animator animator;
    
        //Private Variables
    
        private GridCell _gridCellRight, _gridCellLeft, _gridCellBack, _gridCellFront;

        private bool _isMoving;
        private bool _isRotating;
        private bool _wasMoving;
        private bool _canSwipe = true;
        private bool _isGrounded;
        private float _swipeCooldownTimer;
    
        private Quaternion _targetRotation;
        private Rigidbody _rb;
        private Collider _coll;
        private ParticleSystem.MinMaxGradient _currentParticleGradient;
        
        private static readonly Dictionary<string, ColorCode> colorMap = new Dictionary<string, ColorCode>
        {
            { GREEN_TAG, ColorCode.Green },
            { CYAN_TAG, ColorCode.Cyan },
            { RED_TAG, ColorCode.Red },
            { BLUE_TAG, ColorCode.Blue },
            { YELLOW_TAG, ColorCode.Yellow },
            { ORANGE_TAG, ColorCode.Orange }
        };

    
        #region Initilization
    
        private void Start() 
        {
            _rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            _coll = GetComponent<Collider>();
        }
    
        private void Update()
        {
            SwipeCounter();
        }
    
        private void FixedUpdate() 
        {
            bool grounded = IsGrounded();
    
            if (grounded != _isGrounded)
            {
                _isGrounded = grounded;
            }
    
            ApplyExtraGravity();
        }
    
        #endregion
    
        #region Movement Handling & Raycast Work
    
        private bool IsGrounded()
        {
            bool grounded = Physics.Raycast(transform.position, Vector3.down, playerSO.GroundCheck, playerSO.GridCellLayerMask);
            return grounded;
        }
    
        private void HandleGroundChanged(bool isGrounded)
        {
            if (isGrounded)
            {
                RaycastWork();
                SwipeEffects();
            }
        }
    
        private void ApplyExtraGravity()
        {
            if (!IsGrounded())
            {
                _rb.AddForce(Vector3.down * (playerSO.Gravity * Time.fixedDeltaTime), ForceMode.VelocityChange);
            }
        }
    
        private void RaycastWork()
        {
            RaycastHit hit;
    
            bool hitLeft = Physics.Raycast(transform.position + playerSO.LeftOffset, Vector3.down, out hit, playerSO.Range, playerSO.GridCellLayerMask);
            _gridCellLeft = hitLeft ? hit.transform.gameObject.GetOrAdd<GridCell>() : null;
    
            bool hitRight = Physics.Raycast(transform.position + playerSO.RightOffset, Vector3.down, out hit, playerSO.Range, playerSO.GridCellLayerMask);
            _gridCellRight = hitRight ? hit.transform.gameObject.GetOrAdd<GridCell>() : null;
    
            bool hitBack = Physics.Raycast(transform.position + playerSO.BackOffset, Vector3.down, out hit, playerSO.Range, playerSO.GridCellLayerMask);
            _gridCellBack = hitBack ? hit.transform.gameObject.GetOrAdd<GridCell>() : null;
    
            bool hitFront = Physics.Raycast(transform.position + playerSO.FrontOffset, Vector3.down, out hit, playerSO.Range, playerSO.GridCellLayerMask);
            _gridCellFront = hitFront ? hit.transform.gameObject.GetOrAdd<GridCell>() : null;
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
            float moveDuration = distance / playerSO.MoveSpeed;
    
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
            if (!_canSwipe || !IsGrounded() || GameManager.Instance.HasCompletedGame || GameManager.Instance.IsPaused) return;
    
            _canSwipe = false;
            _swipeCooldownTimer = 0f; 
    
            SpawnJumparticle();
    
            // TODO Integrate new input system && implement a better way to handle such mess
           /* switch (swipe)
            {
                case DirectionId.ID_UP:
                    if (_gridCellFront != null && !_gridCellFront.CantGo)
                    {
                        MakePlayerJumpToCell(_frontCellPoint);
                        RotateCubeSmoothly(Quaternion.Euler(90, 0, 0));
                    }
                    else
                    {
                        AudioManager.Instance.PlaySfx("CantGo");
                    }
                    break;
    
                case DirectionId.ID_UP_LEFT:
                    if (_gridCellFront != null && !_gridCellFront.CantGo)
                    {
                        MakePlayerJumpToCell(_frontCellPoint);
                        RotateCubeSmoothly(Quaternion.Euler(90, 0, 0));
                    }
                    else
                    {
                        AudioManager.Instance.PlaySfx("CantGo");
                    }
                    break;
    
                case DirectionId.ID_DOWN:
                    if (_gridCellBack != null && !_gridCellBack.CantGo)
                    {
                        MakePlayerJumpToCell(_backCellPoint);
                        RotateCubeSmoothly(Quaternion.Euler(-90, 0, 0));
                    }
                    else
                    {
                        AudioManager.Instance.PlaySfx("CantGo");
                    }
                    break;
    
                case DirectionId.ID_DOWN_RIGHT:
                    if (_gridCellBack != null && !_gridCellBack.CantGo)
                    {
                        MakePlayerJumpToCell(_backCellPoint);
                        RotateCubeSmoothly(Quaternion.Euler(-90, 0, 0));
                    }
                    else
                    {
                        AudioManager.Instance.PlaySfx("CantGo");
                    }
                    break;
    
                case DirectionId.ID_RIGHT:
                    if (_gridCellLeft != null && !_gridCellLeft.CantGo)
                    {
                        MakePlayerJumpToCell(_leftCellPoint);
                        RotateCubeSmoothly(Quaternion.Euler(0, 0, 90));
                    }
                    else
                    {
                        AudioManager.Instance.PlaySfx("CantGo");
                    }
                    break;
    
                case DirectionId.ID_UP_RIGHT:
                    if (_gridCellLeft != null && !_gridCellLeft.CantGo)
                    {
                        MakePlayerJumpToCell(_leftCellPoint);
                        RotateCubeSmoothly(Quaternion.Euler(0, 0, 90));
                    }
                    else
                    {
                        AudioManager.Instance.PlaySfx("CantGo");
                    }
                    break;
    
                case DirectionId.ID_LEFT:
                    if (_gridCellRight != null && !_gridCellRight.CantGo)
                    {
                        MakePlayerJumpToCell(_rightCellPoint);
                        RotateCubeSmoothly(Quaternion.Euler(0, 0, -90));
                    }
                    else
                    {
                        AudioManager.Instance.PlaySfx("CantGo");
                    }
                    break;
    
                case DirectionId.ID_DOWN_LEFT:
                    if (_gridCellRight != null && !_gridCellRight.CantGo)
                    {
                        MakePlayerJumpToCell(_rightCellPoint);
                        RotateCubeSmoothly(Quaternion.Euler(0, 0, -90));
                    }
                    else
                    {
                        AudioManager.Instance.PlaySfx("CantGo");
                    }
                    break;
    
                default:
                    break;
            }*/
        }
    
        private void SwipeCounter()
        {
            if (!_canSwipe)
            {
                _swipeCooldownTimer += Time.deltaTime;
    
                if (_swipeCooldownTimer >= playerSO.SwipeCooldownTime)
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
    
            GameObject landParticle = Instantiate(playerSO.LandParticle, transform.position + new Vector3(0f, -1.75f, 0f), playerSO.LandParticle.transform.rotation);
    
            ParticleSystem ps = landParticle.GetComponent<ParticleSystem>();
    
            SetColor(ps);
    
            ps.Play();
    
            Destroy(landParticle, 1.0f);
        }
    
        private void SpawnJumparticle()
        {
            DetectColor();
    
            foreach(GameObject jumpParticle in playerSO.JumpParticle)
            {
                GameObject ps = Instantiate(jumpParticle, transform.position + new Vector3(0f, -1.75f, 0f), jumpParticle.transform.rotation);
    
                ParticleSystem particleSystem = ps.GetComponent<ParticleSystem>();
                particleSystem.Play();
    
                SetColor(particleSystem);
    
                Destroy(ps, 1.0f);
            }
        }
    
        private void DetectColor()
        {
            switch (GameManager.Instance.ColorCode)
            {
                case ColorCode.Red:
                    var color = playerSO.RedColor;
                    _currentParticleGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
                    break;
                
                case ColorCode.Green:
                    color = playerSO.GreenColor;
                    _currentParticleGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
                    break;
                
                case ColorCode.Cyan:
                    color = playerSO.CyanColor;
                    _currentParticleGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
                    break;
                
                case ColorCode.Blue:
                    color = playerSO.BlueColor;
                    _currentParticleGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
                    break;
                
                case ColorCode.Orange:
                    color = playerSO.OrangeColor;
                    _currentParticleGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
                    break;
                
                case ColorCode.Yellow:
                    color = playerSO.YellowColor;
                    _currentParticleGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
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
            if (colorMap.TryGetValue(name, out ColorCode colorCode))
            {
                GameManager.Instance.ChangeColor(colorCode);
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
}
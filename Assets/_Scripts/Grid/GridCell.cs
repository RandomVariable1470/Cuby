using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GridCell : MonoBehaviour
{
    [field:Space(10)]
    [field:Header("References")]
    [field:SerializeField] public GameObject SpawnPoint { get; private set; }
    [field:SerializeField] public GameObject JumpPoint { get; private set; }
    [SerializeField] private GridCellScriptables _gridCellScriptable;

    [HideInInspector] public bool SelectedCell {get; set;}

    private int _posX;
    private int _posY;

    private Vector3 _initialPosition;
    private int _columnId;
    private float _offset;

    private GameManager _manager;
    private UIManager _uiManager;
    private AudioSource _audioSource;
    private Animator _anim;
    private Rigidbody _rb;

    private void Start()
    {
        _initialPosition = transform.position;
        _manager = GameManager.Instance;
        _uiManager = UIManager.Instance;
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void SetColumn(int column)
    {
        _columnId = column;
    }

    private void Update() 
    {
        if (!_manager.HasCompletedGame) WaveAnimation();
    }

    private void WaveAnimation()
    {
        float phase = _columnId * _gridCellScriptable.WavePhaseMultiplier + (_posX + _posY) * 0.1f;
        float sineWave = Mathf.Sin((Time.time + phase) * _gridCellScriptable.FloatMagnitude);
        float yOffset = sineWave * _gridCellScriptable.FloatMagnitude;

        _offset = Mathf.Lerp(_offset, yOffset, Time.deltaTime * _gridCellScriptable.FloatSpeed * 0.5f);

        Vector3 newPosition = _initialPosition + new Vector3(0, _offset, 0);
        transform.position = newPosition;
    }

    public void MakeMeFall(float _fallSpeed)
    {
        _anim.CrossFade(DESTROY_TAG, 0f);
        _rb.isKinematic = false;
        _rb.AddForce(_fallSpeed * Vector3.down * Time.deltaTime, ForceMode.Impulse);
    }

    public void SetPosition(int x, int y)
    {
        _posX = x;
        _posY = y;
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(_posX, _posY);
    }

    public void PlayDestorySound()
    {
        _audioSource.PlayOneShot(_gridCellScriptable.DestroyClip);
    }

    private void OnCollisionStay(Collision other) 
    {
        if (other.gameObject != null && other.gameObject.layer != 6 && SelectedCell)
        {
            _manager.CheckForLevelCompletion();
        }
    }

    private void OnCollisionExit(Collision other) 
    {
        if (other.gameObject != null && other.gameObject.layer != 6 && SelectedCell)
        {
            _uiManager.ChangeHasInitilized();
        }
    }

    #region Cached Properteis

    private readonly string DESTROY_TAG = "Destroy";

    #endregion
}
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
    [SerializeField] private GameObject _cantGoSign;

    [HideInInspector] public bool SelectedCell {get; set;}
    public bool SpawnCell {get; set;}
    [HideInInspector] public bool CantGo {get; set;}

    private int _posX;
    private int _posY;

    private int _columnId;

    private GameManager _manager;
    private Animator _anim;
    private Rigidbody _rb;

    private void Start()
    {
        _manager = GameManager.Instance;
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
    }

    public void SetColumn(int column)
    {
        _columnId = column;
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

    public void SetAsProhibitedCell()
    {
        _cantGoSign.SetActive(true);
        CantGo = true;
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(_posX, _posY);
    }

    public void PlayDestorySound()
    {
        AudioManager.Instance.PlaySfx("OnDestroyGrid");
    }

    private void OnCollisionStay(Collision other) 
    {
        if (other.gameObject != null && other.gameObject.layer != 6 && SelectedCell)
        {
            _manager.CheckForLevelCompletion();
        }
    }

    #region Cached Properties

    private readonly string DESTROY_TAG = "Destroy";

    #endregion
}
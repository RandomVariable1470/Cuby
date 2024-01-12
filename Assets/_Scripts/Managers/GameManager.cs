using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [field: Header("References")]
    [field: SerializeField] public ColorCode ColorCode {get; private set;}
    [field: SerializeField] public ColorCode SelectedCellColorCode {get; private set;}
    [field: Space(5)]
    [field: SerializeField] public string ThisLevelName { get; private set;}
    [field: SerializeField] public string NextLevelName { get; private set;}
    [field: SerializeField] public string MainMenuLevelName { get; private set;}

    [HideInInspector] public bool HasCompletedGame {get; set;}
    [HideInInspector] public GridCell _selectedCell {get; set;}

    private GameGrid _gameGrid;
    private UIManager _uiManager;

    #region Initilization

    private void Start() 
    {
        _gameGrid = GameGrid.Instance;
        _uiManager = UIManager.Instance;

        AudioManager.Instance.PlayMusic("Theme1");
    }

    private void Update() 
    {
        SetSelectedCell();
    }

    #endregion


    public void ChangeColor(ColorCode _color)
    {
        ColorCode = _color;
    }

    public void ChangeSelectedCellColor(ColorCode _color)
    {
        SelectedCellColorCode = _color;
    }

    public void CheckForLevelCompletion()
    {
        if (ColorCode == SelectedCellColorCode)
        {
            LevelCompletion();
        }
    }

    private async void LevelCompletion()
    {
        HasCompletedGame = true;
        _uiManager.TurnOffPauseBtn();
        StartCoroutine(_gameGrid.DestroyGrid());

        if (_gameGrid.HasCompletedFalling)
        {
            _gameGrid.Player.DisableColl();
            _gameGrid.Player.Animator.CrossFade(UNLOADING_TAG, 0f);

            await Task.Delay(500);
            
            _uiManager.TurnOnLevelCompletionMenu();
        }
    }

    private void SetSelectedCell()
    {
        if (_selectedCell != null)
        {
            _selectedCell.SelectedCell = true;
        }
    }

    private readonly string CORRECTCOLOR_TAG = "Correct Color";
    private readonly string UNLOADING_TAG = "UnLoading";
}
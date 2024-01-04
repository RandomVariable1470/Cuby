using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [field: Header("References")]
    [field: SerializeField] public ColorCode ColorCode {get; private set;}
    [field: SerializeField] public ColorCode SelectedCellColorCode {get; private set;}

    [HideInInspector] public bool HasCompleted {get; set;}
    [HideInInspector] public GridCell _selectedCell {get; set;}

    private GameGrid _gameGrid;

    #region Initilization

    private void Start() 
    {
        _gameGrid = GameGrid.Instance;
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
            HasCompleted = true;
            StartCoroutine(_gameGrid.DestroyGrid());
        }
    }

    private void SetSelectedCell()
    {
        if (_selectedCell != null)
        {
            _selectedCell.SelectedCell = true;
        }
    }
}
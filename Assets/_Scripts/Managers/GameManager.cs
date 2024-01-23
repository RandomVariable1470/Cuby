using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class GameManager : Singleton<GameManager>
{
    [field: Header("References")]
    [field: SerializeField] public ColorCode ColorCode {get; private set;}
    [field: SerializeField] public ColorCode SelectedCellColorCode {get; private set;}
    [SerializeField] private PauseMenuUI _pauseMenuUI;
    [SerializeField] private LevelCompletionUI _levelCompletionMenuUI;
    [SerializeField] private TextMeshProUGUI _buildNo;
    [SerializeField] private TextMeshProUGUI _levelIndicator;
    [field: Space(5)]
    [field: SerializeField] public string ThisLevelName { get; private set;}
    [field: SerializeField] public string NextLevelName { get; private set;}
    [field: SerializeField] public string MainMenuLevelName { get; private set;}
    [field: SerializeField] public string SongName { get; private set;}
    [field: Space(5)]
    [field: SerializeField] public bool IsMainMenu { get; private set;}
    [field: SerializeField] public bool IsPaused { get; set;}
    [field: SerializeField] public int LevelToUnlock { get; private set;}
    [field: Space(5)]
    [field: SerializeField] public AudioMixer _audioMixer { get; private set;}

    [HideInInspector] public bool HasCompletedGame {get; set;}
    [HideInInspector] public GridCell SelectedCell {get; set;}

    private GameGrid _gameGrid;

    #region Initilization

    private void Start() 
    {
        _gameGrid = GameGrid.Instance;

        Application.targetFrameRate = 60;

        AudioManager.Instance.PlayMusic(SongName);

        if (_buildNo != null)
        {
            _buildNo.text = $"Version: {Application.version}";
        }
    
        if (_levelIndicator != null) 
        {
            _levelIndicator.text = ThisLevelName;
        }
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
        _pauseMenuUI.TurnOffPauseBtn();
        AudioManager.Instance.StopMusic(SongName);
        StartCoroutine(_gameGrid.DestroyGrid());

        if (_gameGrid.HasCompletedFalling)
        {
            _gameGrid.Player.DisableColl();
            _gameGrid.Player.Animator.CrossFade(UNLOADING_TAG, 0f);

            await Task.Delay(500);
            
            _levelCompletionMenuUI.OpenLevelMenu();
            int levelReached = PlayerPrefs.GetInt("levelReached");
            if (levelReached <= LevelToUnlock)
            {
                PlayerPrefs.SetInt("levelReached", LevelToUnlock);
            }
        }
    }
    
    private readonly string UNLOADING_TAG = "UnLoading";
}
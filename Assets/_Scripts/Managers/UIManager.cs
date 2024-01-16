using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
	[Header("FpsCounter")]
	[SerializeField] private TextMeshProUGUI _fpsText;
	[SerializeField] private TextMeshProUGUI _levelIndicatorText;
	private float _pollingTime = 1f;
	private float _time;
	private int _frameCount;

	[Header("Pause Menu")]
	[SerializeField] private GameObject _pauseBtn;
	[SerializeField] private GameObject _pauseMenu;
	[SerializeField] private GameObject _pauseMenuMenu;
	[SerializeField] private GameObject _pauseOptionMenu;
	[SerializeField] private Animator _pauseMenuAnimator;
	[SerializeField] private Animator _pauseMenuOptionAnimator;

	[Header("Level Completion")]
	[SerializeField] private GameObject _levelCompletionMenu;
	[SerializeField] private Animator _levelCompletionMenuAnim;

	[Header("MainMenu")]
	[SerializeField] private GameObject _mainMenu;
	[SerializeField] private GameObject _mainMenuMenu;
	[SerializeField] private Animator _mainMenuAnimator;
	[SerializeField] private GameObject _optionsMenu;
	[SerializeField] private Animator _optionsMenuAnimator;
	[SerializeField] private GameObject _levelSelection;
	[SerializeField] private Animator _levelSelectionAnimator;
	[SerializeField] private Slider _musicSliderMainMenu;
	[SerializeField] private Slider _sfxSliderMainMenu;

	private GameManager _gameManager;

	#region Initilization

	private void Start() 
	{
		_gameManager = GameManager.Instance;

		if (_levelIndicatorText != null)
		{
			_levelIndicatorText.text = _gameManager.ThisLevelName;
		}

		if (PlayerPrefs.HasKey("musicVolumeSlider"))
		{
			float musicVolume = PlayerPrefs.GetFloat("musicVolumeSlider");
			_gameManager._audioMicer.SetFloat("music", Mathf.Log10(musicVolume)*20f);
			_musicSliderMainMenu.value = musicVolume;
		}
		if (PlayerPrefs.HasKey("sfxVolumeSlider"))
		{
			float sfxVolume = PlayerPrefs.GetFloat("sfxVolumeSlider");
			_gameManager._audioMicer.SetFloat("sfx", Mathf.Log10(sfxVolume)*20f);
			_sfxSliderMainMenu.value = sfxVolume;
		}
	}

	private void Update() 
	{
		if (_fpsText != null)
		{
			FpsCounter();
		}
	}

	#endregion

	#region Fps Counter

	private void FpsCounter()
	{
		_time += Time.deltaTime;

		_frameCount++;

		if (_time >= _pollingTime) 
		{
			int frameRate = Mathf.RoundToInt((float)_frameCount / _time);
			_fpsText.text = frameRate.ToString() + FPS_TAG;

			_time -= _pollingTime;
			_frameCount = 0;
		}
	}

	#endregion

	#region PauseMenu

	public void TurnOnPauseMenu()
	{
		Time.timeScale = 0f;
		AudioManager.Instance.StopMusic("Theme1");
		_pauseBtn.SetActive(false);
		_pauseMenu.SetActive(true);
		_pauseMenuAnimator.CrossFade(IN_TAG, 0f);
	}

	public async void TurnOffPauseMenu()
	{
		_pauseMenuAnimator.CrossFade(OUT_TAG, 0f);
		await Task.Delay(500);
		_pauseBtn.SetActive(true);
		_pauseMenu.SetActive(false);
		AudioManager.Instance.PlayMusic("Theme1");
		Time.timeScale = 1f;
	}

	public async void TurnOnPauseOptionsMenu()
	{
		_pauseMenuAnimator.CrossFade(OUTPAUSE_TAG, 0f);
		await Task.Delay(500);
		_pauseMenuMenu.SetActive(false);
		_pauseOptionMenu.SetActive(true);
		_pauseMenuOptionAnimator.CrossFade(IN_TAG, 0f);
	}

	public async void TurnOffPauseOptionsMenu()
	{
		_pauseMenuOptionAnimator.CrossFade(OUT_TAG, 0f);
		await Task.Delay(500);
		_pauseOptionMenu.SetActive(false);
		_pauseMenuMenu.SetActive(true);
		_pauseMenuAnimator.CrossFade(INPAUSE_TAG, 0f);
	}

	public async void ExitScenePauseMenu()
	{
		_pauseMenuAnimator.CrossFade(OUT_TAG, 0f);
		await Task.Delay(500);
		_pauseBtn.SetActive(true);
		_pauseMenu.SetActive(false);
		AudioManager.Instance.StopMusic("Theme1");
		SceneTransitioner.Instance.LoadScene(_gameManager.MainMenuLevelName, SceneTransitionMode.Circle);
	}

    #endregion

    #region LevelCompletionMenu

	public void TurnOnLevelCompletionMenu()
	{
		_levelCompletionMenu.SetActive(true);
		AudioManager.Instance.StopMusic("Theme1");
		_levelCompletionMenuAnim.CrossFade(IN_TAG, 0f);
	}

	public async void TurnOffLevelCompletionMenu()
	{
		_levelCompletionMenuAnim.CrossFade(OUT_TAG, 0f);
		await Task.Delay(1000);
		AudioManager.Instance.StopMusic("Theme1");
		_levelCompletionMenu.SetActive(false);
	}

	public async void ExitLevelCompletionMenu()
	{
		TurnOffLevelCompletionMenu();
		await Task.Delay(1000);
		SceneTransitioner.Instance.LoadScene(_gameManager.MainMenuLevelName, SceneTransitionMode.Circle);
	}

	public async void NextLevelLevelCompletionMenu()
	{
		TurnOffLevelCompletionMenu();
		await Task.Delay(1000);
		SceneTransitioner.Instance.LoadScene(_gameManager.NextLevelName, SceneTransitionMode.Circle);
	}

	public async void RestartLevelCompletionMenu()
	{
		TurnOffLevelCompletionMenu();
		await Task.Delay(1000);
		SceneTransitioner.Instance.LoadScene(_gameManager.ThisLevelName, SceneTransitionMode.Circle);
	}

	public void TurnOffPauseBtn()
	{
		_pauseBtn.SetActive(false);
	}

    #endregion

	#region MainMenu

	public void TurnOnMainMenu()
	{
		_mainMenu.SetActive(true);
		_mainMenuAnimator.CrossFade(IN_TAG, 0f);
	}

	public async void TurnOffMainMenu()
	{
		_mainMenuAnimator.CrossFade(OUT_TAG, 0f);
		await Task.Delay(500);
		_mainMenu.SetActive(false);
	}

	public async void TurnOnOptionsMenu()
	{	
		_mainMenuAnimator.CrossFade(OUTMENU_TAG, 0f);
		await Task.Delay(500);
		_mainMenuMenu.SetActive(false);
		_optionsMenu.SetActive(true);
		_optionsMenuAnimator.CrossFade(IN_TAG, 0f);
	}

	public async void TurnOffOptionsMenu()
	{
		_optionsMenuAnimator.CrossFade(OUT_TAG, 0f);
		await Task.Delay(500);
		_optionsMenu.SetActive(false);
		_mainMenuMenu.SetActive(true);
		_mainMenuAnimator.CrossFade(INMENU_TAG, 0f);
	}

	public async void TurnOnLevelSelection()
	{
		_mainMenuAnimator.CrossFade(OUTMENU_TAG, 0f);
		await Task.Delay(500);
		_mainMenuMenu.SetActive(false);
		_levelSelection.SetActive(true);
		_levelSelectionAnimator.CrossFade(IN_TAG, 0f);
	}

	public async void TurnOffLevelSelection()
	{
		_levelSelectionAnimator.CrossFade(OUT_TAG, 0f);
		await Task.Delay(500);
		_levelSelection.SetActive(false);
		_mainMenuMenu.SetActive(true);
		_mainMenuAnimator.CrossFade(INMENU_TAG, 0f);
	}

	public void SetMusicVolume()
	{
		float volume = _musicSliderMainMenu.value;
		_gameManager._audioMicer.SetFloat("music", Mathf.Log10(volume)*20f);

		PlayerPrefs.SetFloat("musicVolumeSlider", volume);
	}
	public void SetSFXVolume()
	{
		float volume = _sfxSliderMainMenu.value;
		_gameManager._audioMicer.SetFloat("sfx", Mathf.Log10(volume)*20f);

		PlayerPrefs.SetFloat("sfxVolumeSlider", volume);
	}

	#endregion

	#region Cached Properties

	private readonly string IN_TAG = "In";
	private readonly string OUT_TAG = "Out";
	private readonly string INPAUSE_TAG = "InPause";
	private readonly string OUTPAUSE_TAG = "OutPause";
	private readonly string INMENU_TAG = "InMenu";
	private readonly string OUTMENU_TAG = "OutMenu";
	private readonly string FPS_TAG = "FPS";

	#endregion
}
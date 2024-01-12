using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class UIManager : Singleton<UIManager>
{
	[Header("FpsCounter")]
	[SerializeField] private TextMeshProUGUI _fpsText;
	private float _pollingTime = 1f;
	private float _time;
	private int _frameCount;

	[Header("Pop Up For Cell Color")]
	[SerializeField] private GameObject _popCellColor;
	[SerializeField] private TextMeshProUGUI _popCellColorText;
	[SerializeField] private Animator _popCellColorAnimator;
	private bool _hasInitilized;

	[Header("Pause Menu")]
	[SerializeField] private GameObject _pauseBtn;
	[SerializeField] private GameObject _pauseMenu;
	[SerializeField] private GameObject _pauseMenuMenu;
	[SerializeField] private GameObject _pauseOptionMenu;
	[SerializeField] private Animator _pauseMenuAnimator;
	[SerializeField] private Animator _pauseMenuOptionAnimator;

	#region Initilization

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

	#region PopUp

	public async void PopUpColorCode(int delayTime)
	{
		if (!_hasInitilized)
		{
			_hasInitilized = true;
			_popCellColor.SetActive(true);
			_popCellColorAnimator.CrossFade(IN_TAG, 0f);

			await Task.Delay(delayTime);

			_popCellColorAnimator.CrossFade(OUT_TAG, 0f);

			await Task.Delay(300);

			_popCellColor.SetActive(false);
		}
	}

	public void ChangePopColorCodeText(string text)
	{
		_popCellColorText.text = text;
	}

	public void ChangeHasInitilized()
	{
		_hasInitilized = false;
	}

	#endregion

	#region PauseMenu

	public void TurnOnPauseMenu()
	{
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

	public async void ExitScenePauseMenu(string name)
	{
		TurnOffPauseMenu();
		await Task.Delay(500);
		LevelManager.Instance.LevelLoad(name);
	}

    #endregion

    #region LevelCompletionMenu

    #endregion

	#region Cached Properties

	private readonly string IN_TAG = "In";
	private readonly string OUT_TAG = "Out";
	private readonly string INPAUSE_TAG = "InPause";
	private readonly string OUTPAUSE_TAG = "OutPause";
	private readonly string FPS_TAG = "FPS";
	private readonly string NEWSTATE_TAG = "New State";

	#endregion
}
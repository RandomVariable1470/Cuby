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
	[SerializeField] private GameObject _pauseMenu;
	[SerializeField] private Animator _pauseMenuAnimator;

	private void Update() 
	{
		if (_fpsText != null)
		{
			FpsCounter();
		}
	}

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


    #endregion

    #region LevelCompletionMenu

    #endregion

	#region Cached Properties

	private readonly string IN_TAG = "In";
	private readonly string OUT_TAG = "Out";
	private readonly string FPS_TAG = "FPS";

	#endregion
}
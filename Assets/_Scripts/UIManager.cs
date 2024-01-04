using UnityEngine;
using TMPro;
 
public class UIManager : MonoBehaviour 
{

	[Header("FpsCounter")]
	[SerializeField] private TextMeshProUGUI _fpsText;
	private float _pollingTime = 1f;
	private float _time;
	private int _frameCount;

	[Header("Pop Up For Cell Color")]
	[SerializeField] private GameObject _popCellColor;

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
			_fpsText.text = frameRate.ToString() + " fps";

			_time -= _pollingTime;
			_frameCount = 0;
		}
	}

	public void PopUpColorCode()
	{
		
	}
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasUI : MonoBehaviour
{   
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private TextMeshProUGUI _fpsText;
    [SerializeField] private TextMeshProUGUI _levelCompletionText;
    
    private GameManager _gameManager;

	private float pollingTime = 1f;
	private float time;
	private int frameCount;


    private void Awake() 
    {
        _gameManager = GameManager.Instance;
        
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
        }

        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetSFXVolume();
        }
    }

    private void Start() 
    {
        if (_levelCompletionText != null ) _levelCompletionText.text = _gameManager.ThisLevelName + " Completed";
    }

    private void Update() 
    {
		time += Time.deltaTime;

		frameCount++;

		if (time >= pollingTime) 
        {
			int frameRate = Mathf.RoundToInt((float)frameCount / time);
			_fpsText.text = frameRate.ToString() + " fps";

			time -= pollingTime;
			frameCount = 0;
		}
	}

    public void ClickSound()
    {
        AudioManager.Instance.PlaySfx("UIClick");
    }

    public void SetMusicVolume()
    {
        float volume = _musicSlider.value;
		_gameManager._audioMixer.SetFloat("music", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = _sfxSlider.value;
		_gameManager._audioMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    private void LoadVolume()
    {
        _musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        _sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");

        SetSFXVolume();
        SetMusicVolume();
    }
}
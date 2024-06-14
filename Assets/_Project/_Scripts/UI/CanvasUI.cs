using System.Collections;
using System.Collections.Generic;
using RV.Systems;
using RV.Systems.AudioSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RV.UI
{
    public class CanvasUI : MonoBehaviour
    {   
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private TextMeshProUGUI levelCompletionText;
    
        private void Awake() 
        {
            if (PlayerPrefs.HasKey("musicVolume") || PlayerPrefs.HasKey("sfxVolume"))
            {
                LoadVolume();
                return;
            }
            
            SetMusicVolume();
            SetSFXVolume();
        }
    
        private void Start() 
        {
            if (levelCompletionText != null ) levelCompletionText.text = GameManager.Instance.ThisLevelName + " Completed";
        }
        
        public void ClickSound()
        {
            AudioManager.Instance.PlaySfx("UIClick");
        }
    
        public void SetMusicVolume()
        {
            float volume = musicSlider.value;
            GameManager.Instance.AudioMixer.SetFloat("music", Mathf.Log10(volume)*20);
            PlayerPrefs.SetFloat("musicVolume", volume);
        }
    
        public void SetSFXVolume()
        {
            float volume = sfxSlider.value;
            GameManager.Instance.AudioMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("sfxVolume", volume);
        }
    
        private void LoadVolume()
        {
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
            sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
    
            SetSFXVolume();
            SetMusicVolume();
        }
    }
}
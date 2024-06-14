using System.Collections;
using System.Collections.Generic;
using RV.Systems;
using RV.Systems.SceneTransitioning;
using RV.Systems.AudioSystem;
using UnityEngine;
using UnityEngine.UI;

namespace RV.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject _pauseMenuBtn;
        [SerializeField] private GameObject _optionsMenu;
        [Space(15)]
        [SerializeField] private CanvasGroup _pauseMenuBG;
        [Space(15)]
        [SerializeField] private RectTransform _pauseMenuTransform;
        [SerializeField] private RectTransform _optionsMenuTransform;
        [Space(15)]
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;
    
        private void OnEnable() 
        {
            _pauseMenuBG.alpha = 0f;
            _pauseMenuBG.LeanAlpha(1f, 0.5f).setIgnoreTimeScale(true);
    
            _pauseMenuTransform.LeanMoveX(40f, .5f).setEaseInCubic().setIgnoreTimeScale(true).delay = 0.1f;
            Time.timeScale = 0f;
            GameManager.Instance.IsPaused = true;
            AudioManager.Instance.StopMusic(GameManager.Instance.SongName);
        }
    
        public void ClosePauseMenu()
        {
            _pauseMenuBG.LeanAlpha(0f, 0.5f).setIgnoreTimeScale(true); 
    
            LTDescr _second = _pauseMenuTransform.LeanMoveX(-1000f, .5f).setEaseOutCubic().setDelay(0.1f).setIgnoreTimeScale(true);
            _second.setOnComplete(() =>
            {
                _pauseMenuBtn.SetActive(true);
                transform.gameObject.SetActive(false);
                Time.timeScale = 1f;
                AudioManager.Instance.PlayMusic(GameManager.Instance.SongName);
                GameManager.Instance.IsPaused = false;
            });
            
        }
    
        public void RestartPauseMenu()
        {
            _pauseMenuBG.LeanAlpha(0f, 0.5f).setIgnoreTimeScale(true);
    
            LTDescr _ = _pauseMenuTransform.LeanMoveX(-1000f, 0.5f).setEaseOutExpo().setDelay(0.1f).setIgnoreTimeScale(true);
            _.setOnComplete(() =>
            {
                _pauseMenuBtn.SetActive(true);
                transform.gameObject.SetActive(false);
                Time.timeScale = 1f;
                GameManager.Instance.IsPaused = false;
                SceneTransitioner.Instance.LoadScene(GameManager.Instance.ThisLevelName, SceneTransitionMode.Circle);
            });
        }
    
        public void OpenSettingsMenu()
        {
            LTDescr _ = _pauseMenuTransform.LeanMoveX(-1000f, 0.5f).setEaseOutExpo().setDelay(0.1f).setIgnoreTimeScale(true);
            _.setOnComplete(() =>
            {
                _optionsMenu.SetActive(true);
                _optionsMenuTransform.LeanScale(new Vector3(1f, 1f, 1f), 0.5f).setEaseOutSine().setIgnoreTimeScale(true);
            });
        }
    
        public void CloseSettingsMenu()
        {
            LTDescr _ = _optionsMenuTransform.LeanScale(new Vector3(0f, 0f, 0f), 0.5f).setEaseInSine().setIgnoreTimeScale(true);
            _.setOnComplete(() =>
            {
                _optionsMenu.SetActive(false);
                _pauseMenuTransform.LeanMoveX(40f, 0.5f).setEaseInExpo().setIgnoreTimeScale(true);
            });
        }
    
        public void BackToMainMenu()
        {
            _pauseMenuBG.LeanAlpha(0f, 0.5f);
    
            LTDescr _ = _pauseMenuTransform.LeanMoveX(-1000f, 0.5f).setEaseOutExpo().setDelay(0.1f).setIgnoreTimeScale(true);
            _.setOnComplete(() =>
            {
                _pauseMenuBtn.SetActive(true);
                transform.gameObject.SetActive(false);
                Time.timeScale = 1f;
                GameManager.Instance.IsPaused = false;
                SceneTransitioner.Instance.LoadScene(GameManager.Instance.MainMenuLevelName, SceneTransitionMode.Circle);
            });
        }
    
        public void TurnOffPauseBtn()
        {
            _pauseMenuBtn.SetActive(false);
        }
    }
}

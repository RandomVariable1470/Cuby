using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private RectTransform _mainMenuTransform;
    [Space(15)]
    [SerializeField] private CanvasGroup _mainMenuBG;
    [Space(15)]
    [SerializeField] private TextMeshProUGUI _titleScreenText;
    [SerializeField] private RectTransform _titleScreenTextTransform;
    [Space(15)]
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private RectTransform _settingsMenuTransform;
    [Space(15)]
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;
    [Space(15)]
    [SerializeField] private GameObject _levelSelector;
    [SerializeField] private RectTransform _levelSelectorTransform;
    [Space(15)]
    [SerializeField] private GameObject _infoMenu;
    [SerializeField] private RectTransform _infoMenuTransform;
    [Space(15)]
    [SerializeField] private Button[] _levelbuttons;
    [Space(15)]
    [SerializeField] private CanvasGroup _howToPlayTextCanvasGroup;
    [SerializeField] private GameObject _howToPlayGame;
    [SerializeField] private RectTransform _howToPlayGameTransform;

    private void Start() 
    {
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);

        for (int i = 0; i < _levelbuttons.Length; i++)
        {
            if (i + 1 > levelReached)
            {
                _levelbuttons[i].interactable = false;
            }
        }
    }

    private void OnEnable() 
    {
        _mainMenuBG.LeanAlpha(1f, 0.3f);
        _mainMenuTransform.LeanScale(new Vector3(1f, 1f, 1f), 0.5f).setEaseOutBounce().delay = 0.2f;
        _titleScreenText.transform.LeanScale(new Vector3(1f, 1f, 1f), 0.5f).setEaseOutBounce().delay = 0.2f;
        _howToPlayTextCanvasGroup.LeanAlpha(1f, 0.3f);
    }

    public void GoToNextLevel(string levelName)
    {
        _levelSelectorTransform.LeanMoveY(-989.5f, 0.5f).setEaseOutSine();
        LTDescr _ = _titleScreenTextTransform.LeanScale(new Vector3(0f, 0f, 0f), 0.25f).setEaseInSine();
        _.setOnComplete(() =>
        {
            _levelSelector.SetActive(false);
            SceneTransitioner.Instance.LoadScene(levelName, SceneTransitionMode.Circle);
        });
    }
    
    public void OpenSettingsMenu()
    {
        _titleScreenText.transform.LeanScale(new Vector3(0.75f, 0.75f, 0.75f), 0.5f).setEaseInSine();
        _howToPlayTextCanvasGroup.LeanAlpha(0f, 0.3f);
        LTDescr _ = _mainMenuTransform.LeanScale(new Vector3(0f, 0f, 0f), 0.5f).setEaseInBounce();
        _.setOnComplete(() =>
        {
            _mainMenu.SetActive(false);
            _settingsMenu.SetActive(true);
            _settingsMenuTransform.LeanScale(new Vector3(1f,1f,1f), 0.5f).setEaseOutCubic();
        });
    }

    public void CloseSettingsMenu()
    {
        LTDescr _ =_settingsMenuTransform.LeanScale(new Vector3(0f,0f,0f), 0.5f).setEaseInCubic();    
        _.setOnComplete(() =>
        {
            _settingsMenu.SetActive(false);
            _mainMenu.SetActive(true);
            _titleScreenText.transform.LeanScale(new Vector3(1f, 1f, 1f), 0.5f).setEaseOutSine();
            _howToPlayTextCanvasGroup.LeanAlpha(1f, 0.3f);
            _mainMenuTransform.LeanScale(new Vector3(1f, 1f, 1f), 0.5f).setEaseOutBounce();
        });
    }

    public void OpenLevelSelector()
    {
        _titleScreenText.transform.LeanScale(new Vector3(0.75f, 0.75f, 0.75f), 0.5f).setEaseInSine();
        _howToPlayTextCanvasGroup.LeanAlpha(0f, 0.3f);
        LTDescr _ = _mainMenuTransform.LeanScale(new Vector3(0f, 0f, 0f), 0.5f).setEaseInBounce();
        _.setOnComplete(() =>
        {
            _mainMenu.SetActive(false);
           _levelSelector.SetActive(true);
           _levelSelectorTransform.LeanMoveY(-95f, 0.5f).setEaseInSine().delay = 0.1f;
        });
    }

    public void CloseLevelSelector()
    {
        _levelSelectorTransform.LeanMoveY(-989.5f, 0.5f).setEaseOutSine();
        LTDescr _ = _titleScreenTextTransform.LeanMoveX(0f, 0.2f).setEaseOutSine();
        _.setOnComplete(() =>
        {
            _levelSelector.SetActive(false);
            _mainMenu.SetActive(true);
            _titleScreenText.transform.LeanScale(new Vector3(1f, 1f, 1f), 0.5f).setEaseOutSine();
            _howToPlayTextCanvasGroup.LeanAlpha(1f, 0.3f);
            _mainMenuTransform.LeanScale(new Vector3(1f, 1f, 1f), 0.5f).setEaseOutBounce();
        });
    }

    public void OpenInfoMenu()
    {
        _titleScreenText.transform.LeanScale(new Vector3(0.75f, 0.75f, 0.75f), 0.5f).setEaseInSine();
        _howToPlayTextCanvasGroup.LeanAlpha(0f, 0.3f);
        LTDescr _ = _mainMenuTransform.LeanScale(new Vector3(0f, 0f, 0f), 0.5f).setEaseInBounce();
        _.setOnComplete(() =>
        {
            _mainMenu.SetActive(false);
            _infoMenu.SetActive(true);
            _infoMenuTransform.LeanMoveY(-376.5f, 0.5f).setEaseOutSine();
        });
    }

    public void CloseInfoMenu()
    {
        LTDescr _ = _infoMenuTransform.LeanMoveY(-1242.5f, 0.5f).setEaseInSine();    
        _.setOnComplete(() =>
        {
            _infoMenu.SetActive(false);
            _mainMenu.SetActive(true);
            _titleScreenText.transform.LeanScale(new Vector3(1f, 1f, 1f), 0.5f).setEaseOutSine();
            _howToPlayTextCanvasGroup.LeanAlpha(1f, 0.3f);
            _mainMenuTransform.LeanScale(new Vector3(1f, 1f, 1f), 0.5f).setEaseOutBounce();
        });
    }

    public void OpenHowToPlayMenu()
    {
        _titleScreenText.transform.LeanScale(new Vector3(0.5f, 0.45f, 0.5f), 0.5f).setEaseInSine();
        _howToPlayTextCanvasGroup.LeanAlpha(0f, 0.3f);
        LTDescr _ = _mainMenuTransform.LeanScale(new Vector3(0f, 0f, 0f), 0.5f).setEaseInBounce();
        _.setOnComplete(() =>
        {
            _mainMenu.SetActive(false);
            _howToPlayGame.SetActive(true);
            _howToPlayGameTransform.LeanScale(new Vector3(1f, 1f, 1f), 0.5f).setEaseOutCirc();
        });
    }

    public void CloseHowToPlayMenu()
    {
        LTDescr _ = _howToPlayGameTransform.LeanScale(new Vector3(0f, 0f, 0f), 0.5f).setEaseInCirc();
        _.setOnComplete(() =>
        {
            _howToPlayGame.SetActive(false);
            _mainMenu.SetActive(true);
            _titleScreenText.transform.LeanScale(new Vector3(1f, 1f, 1f), 0.5f).setEaseOutSine();
            _howToPlayTextCanvasGroup.LeanAlpha(1f, 0.3f);
            _mainMenuTransform.LeanScale(new Vector3(1f, 1f, 1f), 0.5f).setEaseOutBounce();
        });
    }

    public void OpenGithubLink()
    {
        Application.OpenURL("https://github.com/RandomVariable1470/Cuby");
    }

    public void OpenItchLink()
    {
        Application.OpenURL("");
    }
}
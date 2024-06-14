using System.Collections;
using System.Collections.Generic;
using RV.Systems;
using RV.Systems.SceneTransitioning;
using UnityEngine;

namespace RV.UI
{
    public class LevelCompletionUI : MonoBehaviour
    {
        [SerializeField] private GameObject _levelMenu;
        [SerializeField] private CanvasGroup _levelMenuBG;
        [SerializeField] private RectTransform _levelMenuTransform;

        public void OpenLevelMenu()
        {
            _levelMenu.SetActive(true);
            _levelMenuBG.LeanAlpha(1f, 0.3f);
            _levelMenuTransform.LeanScale(new Vector3(1f, 1f, 1f), 0.5f).setEaseOutQuint().setDelay(0.2f);
        }

        public void ContinueLevelMenu()
        {
            _levelMenuBG.LeanAlpha(0f, 0.3f);
            LTDescr _ = _levelMenuTransform.LeanScale(new Vector3(0f, 0f, 0f), 0.5f).setEaseInQuint().setDelay(0.2f);
            _.setOnComplete(() =>
            {
                SceneTransitioner.Instance.LoadScene(GameManager.Instance.NextLevelName, SceneTransitionMode.Circle);
            });
        }

        public void RestartLevelMenu()
        {
            _levelMenuBG.LeanAlpha(0f, 0.3f);
            LTDescr _ = _levelMenuTransform.LeanScale(new Vector3(0f, 0f, 0f), 0.5f).setEaseInQuint().setDelay(0.2f);
            _.setOnComplete(() =>
            {
                SceneTransitioner.Instance.LoadScene(GameManager.Instance.ThisLevelName, SceneTransitionMode.Circle);
            });
        }

        public void BackToMenuLevelMenu()
        {
            _levelMenuBG.LeanAlpha(0f, 0.3f);
            LTDescr _ = _levelMenuTransform.LeanScale(new Vector3(0f, 0f, 0f), 0.5f).setEaseInQuint().setDelay(0.2f);
            _.setOnComplete(() =>
            {
                SceneTransitioner.Instance.LoadScene(GameManager.Instance.MainMenuLevelName, SceneTransitionMode.Circle);
            });
        }
    }
}

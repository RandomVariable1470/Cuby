using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : SingletonPersistent<LevelManager>
{
    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private Image _progressBar;

    private float _target;

    private void Update() 
    {
        _progressBar.fillAmount = Mathf.MoveTowards(_progressBar.fillAmount, _target, 3f * Time.deltaTime);
    }

    public async void LevelLoad(string levelName)
    {
        _target = 0f;
        _progressBar.fillAmount = 0f;

        var scene = SceneManager.LoadSceneAsync(levelName);

        scene.allowSceneActivation = false;

        _loaderCanvas.SetActive(true);

        Time.timeScale = 1f;

        do
        {
            await Task.Delay(100);
            _target = scene.progress;
        }while(scene.progress < 0.9f);

        await Task.Delay(1000);

        scene.allowSceneActivation = true;
        _loaderCanvas.SetActive(false);
    }
}
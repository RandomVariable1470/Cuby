using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private Button[] _levelbuttons;

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

    public void GoToLevel(string levelName)
    {
        SceneTransitioner.Instance.LoadScene(levelName, SceneTransitionMode.Circle);
    }
}
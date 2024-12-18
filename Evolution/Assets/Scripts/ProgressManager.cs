using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager instance;

    [SerializeField] private List<string> levelNames = new List<string>();
    private int currentLevel = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadSpecificLevel(int level)
    {
        currentLevel = level;
        LoadLevel();
    }

    public void LoadNextLevel()
    {
        currentLevel++;
        LoadLevel();
    }

    private void LoadLevel()
    {
        if (currentLevel == levelNames.Count - 1)
        {
            AudioManager.instance.End();
        }
        AudioManager.instance.PlayMusic(currentLevel);
        SceneManager.LoadScene(levelNames[currentLevel]);
    }
}

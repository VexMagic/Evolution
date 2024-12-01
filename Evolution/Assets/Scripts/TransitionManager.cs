using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject undo;

    private void Awake()
    {
        instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void EnableUndo(bool enable)
    {
        undo.SetActive(enable);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        animator.SetTrigger("Start");
    }

    public void FinishLevel()
    {
        animator.SetTrigger("Finish");
    }

    public void NextLevel()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        ProgressManager.instance.LoadNextLevel();
    }
}

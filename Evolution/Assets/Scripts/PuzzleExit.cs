using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleExit : MonoBehaviour
{
    public GameObject playerRef;
    public string sceneName;
    public List<KeyCode> requiredKeySacrifices ;
    public int pointsToComplete;
    bool isComplete = false;
    
    

    public void CheckIsFinished(int score,List<KeyCode> sacrificedKeys)
    {
        int keysCounter = 0;
        if (score >= pointsToComplete)
        {
            isComplete = true;
        }
        else if (requiredKeySacrifices.Count > 0 )
        {
            foreach (KeyCode key in sacrificedKeys)
            {
                foreach (KeyCode reqKey in requiredKeySacrifices)
                {
                    if (key == reqKey)
                    {
                        keysCounter++;
                    }
                }
            }
            if (keysCounter == requiredKeySacrifices.Count)
            {
                isComplete = true;
            }
        }  
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isComplete == true)
        {
            if (collision.gameObject == playerRef)
            {
                SceneManager.LoadScene(sceneName);
            }
        }
        
    }
}
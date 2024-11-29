using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    bool isPlaying = false;
    float currentTime;
    // Start is called before the first frame update
    void Start()
    {
        isPlaying = true;
        currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        while (isPlaying)
        {
            currentTime = currentTime + Time.deltaTime;
        }
    }
}

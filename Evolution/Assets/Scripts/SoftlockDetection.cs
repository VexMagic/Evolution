using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftlockDetection : MonoBehaviour
{
    [SerializeField] private int badSegmentAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("trigger enter");
        if (collision.gameObject == PlayerMovement.instance.gameObject)
        {
            if (PlayerMovement.instance.GetSegmentAmount() == badSegmentAmount)
            {
                TransitionManager.instance.EnableUndo(true);
            }
        }
    }
}

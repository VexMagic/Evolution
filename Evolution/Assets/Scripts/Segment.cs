using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected SpriteRenderer renderer;

    public Rigidbody2D RB {  get { return rb; } }

    private bool isTail;

    public void SetTail(bool isTail)
    {
        this.isTail = isTail;
    }

    public virtual void UpdateSprite()
    {
        if (isTail)
        {
            renderer.color = Color.gray;
        }
        else 
        {
            renderer.color = Color.white;
        }
    }
}

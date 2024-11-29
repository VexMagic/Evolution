using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected SpriteRenderer renderer;
    [SerializeField] private Sprite tail;
    [SerializeField] private Sprite straight, turnL, turnR;

    public Rigidbody2D RB {  get { return rb; } }
    public bool IsTail {  get { return isTail; } }

    private bool isTail;
    protected Direction rotation;
    private Direction previousRotation;

    public Direction Rotation { get { return rotation; } }

    public void SetTail(bool isTail)
    {
        this.isTail = isTail;
    }

    public void SetValues(Segment segment)
    {
        transform.position = segment.RB.position;
        previousRotation = rotation;
        rotation = segment.Rotation;
        UpdateSprite();
    }

    public void SetDirection(Direction direction)
    {
        previousRotation = rotation;
        rotation = direction;
        UpdateSprite();
    }

    public virtual void UpdateSprite()
    {
        UpdateRotation();

        if (isTail)
        {
            renderer.sprite = tail;
        }
        else
        {
            if (rotation == previousRotation)
            {
                renderer.sprite = straight;
            }
            else if ((int)rotation == ((int)previousRotation + 1) % 4)
            {
                renderer.sprite = turnL;
            }
            else
            {
                renderer.sprite = turnR;
            }
        }
    }

    protected void UpdateRotation()
    {
        switch (rotation)
        {
            case Direction.Up:
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case Direction.Down:
                transform.eulerAngles = new Vector3(0, 0, 180);
                break;
            case Direction.Left:
                transform.eulerAngles = new Vector3(0, 0, 90);
                break;
            case Direction.Right:
                transform.eulerAngles = new Vector3(0, 0, -90);
                break;
        }
    }
}

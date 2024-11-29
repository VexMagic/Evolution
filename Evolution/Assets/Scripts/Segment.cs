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
    public Direction PreviousRotation { get { return previousRotation; } }

    public void SetTail(bool isTail)
    {
        this.isTail = isTail;
    }

    public void SetValues(Segment segment, bool forward)
    {
        transform.position = segment.RB.position;

        if (forward)
        {
            previousRotation = rotation;
            rotation = segment.Rotation;
        }
        else
        {
            rotation = previousRotation;
            if (segment.isTail)
                previousRotation = segment.Rotation;
            else
                previousRotation = segment.PreviousRotation;
        }

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

    protected Vector2Int DirectionToVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return new Vector2Int(0, 1);
            case Direction.Down:
                return new Vector2Int(0, -1);
            case Direction.Left:
                return new Vector2Int(-1, 0);
            case Direction.Right:
                return new Vector2Int(1, 0);
        }
        return new Vector2Int(0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTail)
            return;

        Collectable collectable = collision.GetComponent<Collectable>();
        if (collectable != null)
        {
            Direction tempDirection = (Direction)(((int)rotation + 2) % 4);

            collectable.Move(DirectionToVector(tempDirection));
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected SpriteRenderer renderer;
    [SerializeField] private Sprite tail;
    [SerializeField] private Sprite straight, turnL, turnR;

    private bool isTail;
    protected Direction rotation;
    private Direction previousRotation;

    protected List<SegmentMoveData> segmentMoves = new List<SegmentMoveData>();

    public Direction Rotation { get { return rotation; } }
    public Direction PreviousRotation { get { return previousRotation; } }
    public Rigidbody2D RB { get { return rb; } }
    public SpriteRenderer Renderer { get { return renderer; } }
    public bool IsTail { get { return isTail; } }

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

    public virtual void SetShakeOffset(Vector2 offeset)
    {
        renderer.transform.localPosition = offeset;
    }

    public virtual void StoreMoveData()
    {
        segmentMoves.Add(new SegmentMoveData(transform.position, rotation, previousRotation, isTail));
    }

    public virtual void LoadLastMoveData()
    {
        if (segmentMoves.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        renderer.color = Color.white;

        transform.position = segmentMoves[^1].Position;
        rotation = segmentMoves[^1].Rotation;
        previousRotation = segmentMoves[^1].PreviousRotation;
        isTail = segmentMoves[^1].IsTail;

        segmentMoves.RemoveAt(segmentMoves.Count - 1);
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
                renderer.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case Direction.Down:
                renderer.transform.eulerAngles = new Vector3(0, 0, 180);
                break;
            case Direction.Left:
                renderer.transform.eulerAngles = new Vector3(0, 0, 90);
                break;
            case Direction.Right:
                renderer.transform.eulerAngles = new Vector3(0, 0, -90);
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
            if (collectable.IsDead)
                return;
            
            Direction tempDirection = (Direction)(((int)rotation + 2) % 4);

            collectable.Move(DirectionToVector(tempDirection));
        }
    }

    private void OnDestroy()
    {
        PlayerMovement.instance.RemoveSegment(this);
    }
}

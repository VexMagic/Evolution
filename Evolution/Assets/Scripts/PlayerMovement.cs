using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : Segment
{
    public static PlayerMovement instance;

    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private int startSegments;

    [SerializeField] private InputActionReference up, down, left, rigth;
    [SerializeField] private InputActionReference undo, reset;

    [SerializeField] private bool canMoveUp, canMoveDown, canMoveLeft, canMoveRight;

    [SerializeField] private Direction startRotation;

    private List<CanMoveData> canMoves = new List<CanMoveData>();

    private List<GameObject> segmentObjects = new List<GameObject>();
    private List<Segment> segmentData = new List<Segment>();

    private Vector2 lastPosition;
    private Direction lastRotation;

    private bool hasRecievedInput;
    private bool isShaking;
    private bool isDead;
    private bool reseting;

    private Coroutine shakeCoroutine;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rotation = startRotation;

        Vector2 offset = -DirectionToVector(rotation);

        lastPosition = rb.position;
        lastRotation = rotation;
        for (int i = 0; i < startSegments; i++)
        {
            lastPosition += offset;
            AddSegment();
        }
    }

    private void OnEnable()
    {
        up.action.performed += MoveUp;
        down.action.performed += MoveDown;
        left.action.performed += MoveLeft;
        rigth.action.performed += MoveRight;
        undo.action.performed += Undo;
        reset.action.performed += Reset;
    }

    private void OnDisable()
    {
        up.action.performed -= MoveUp;
        down.action.performed -= MoveDown;
        left.action.performed -= MoveLeft;
        rigth.action.performed -= MoveRight;
        undo.action.performed -= Undo;
        reset.action.performed -= Reset;
    }

    public void RemoveSegment(Segment segment)
    {
        for (int i = 0; i < segmentData.Count; i++)
        {
            if (segmentData[i] == segment)
            {
                segmentData.RemoveAt(i);
                segmentObjects.RemoveAt(i);
                return;
            }
        }
        UpdateSprite();
    }

    private void CantMove()
    {
        AudioManager.instance.PlaySFX("no move");
        shakeCoroutine = StartCoroutine(ShakeAnimation());
    }

    IEnumerator ShakeAnimation()
    {
        isShaking = true;

        float shakeSpeed = 0.05f;

        for (int i = 0; i < 6; i++)
        {
            Vector2 shake = new Vector2(Random.Range(-shakeSpeed, shakeSpeed), Random.Range(-shakeSpeed, shakeSpeed));
            SetShakeOffset(shake);
            yield return new WaitForSeconds(0.025f);
        }

        SetShakeOffset(Vector2.zero);

        isShaking = false;
    }

    public override void SetShakeOffset(Vector2 offset)
    {
        base.SetShakeOffset(offset);
        foreach (var segment in segmentData)
        {
            segment.SetShakeOffset(offset);
        }
    }

    IEnumerator InputBuffer()
    {
        yield return new WaitForFixedUpdate();
        hasRecievedInput = false;
    }

    private void Move( Direction newRotation)
    {
        if (hasRecievedInput || isDead || reseting)
            return;

        if (isShaking)
        {
            StopCoroutine(shakeCoroutine);
            SetShakeOffset(Vector2.zero);
            isShaking = false;
        }

        Vector2Int direction = DirectionToVector(newRotation);

        hasRecievedInput = true;
        StartCoroutine(InputBuffer());

        Vector2 newPos = rb.position + direction;

        if (CheckForSegment(newPos))
        {
            if ((int)rotation == ((int)newRotation + 2) % 4)
                MoveBackwards();
            else
                CantMove();
            return;
        }

        if (GridManager.instance.IsPointInsideWall(newPos))
        {
            CantMove();
            return;
        }

        AudioManager.instance.PlaySFX("move");
        CollectableManager.instance.StoreData();

        for (int i = segmentObjects.Count - 1; i >= 0; i--)
        {
            segmentData[i].StoreMoveData();
            if (i == 0)
            {
                segmentData[0].transform.position = RB.position;
                segmentData[0].SetDirection(rotation);
            }
            else
            {
                segmentData[i].SetValues(segmentData[i - 1], true);
                if (i == segmentObjects.Count - 1)
                {
                    lastPosition = segmentData[i].RB.position;
                    lastRotation = segmentData[i].Rotation;
                }
            }
        }
        StoreMoveData();
        transform.position = newPos;
        rotation = newRotation;
        segmentData[0].SetDirection(rotation);

        UpdateSprite();
        CheckForHole();
    }

    public override void StoreMoveData()
    {
        base.StoreMoveData();
        canMoves.Add(new CanMoveData(canMoveUp, canMoveDown, canMoveLeft, canMoveRight));
    }

    private void MoveBackwards()
    {
        AudioManager.instance.PlaySFX("move");

        Vector2 offset = DirectionToVector((Direction)(((int)segmentData[^1].Rotation + 2) % 4));
        Vector2 newPos = segmentData[^1].RB.position + offset;

        Vector2 tempPos = segmentData[^1].RB.position;

        do
        {
            tempPos += offset;
            if (GridManager.instance.IsPointInsideWall(tempPos))
            {
                CantMove();
                return;
            }
        } while (CollectableManager.instance.CollectableAtPos(tempPos) != null);
        
        CollectableManager.instance.StoreData();

        StoreMoveData();

        transform.position = segmentData[0].RB.position;
        
        if (segmentData[0].IsTail)
            rotation = segmentData[0].Rotation;
        else
            rotation = segmentData[0].PreviousRotation;

        for (int i = 0; i < segmentObjects.Count; i++)
        {
            segmentData[i].StoreMoveData();
            if (i == segmentObjects.Count - 1)
            {
                segmentData[i].transform.position = newPos;
                segmentData[i].UpdateSprite();
            }
            else
            {
                segmentData[i].SetValues(segmentData[i + 1], false);
            }
        }

        UpdateRotation();
        CheckForHole();
    }

    private bool CheckForSegment(Vector2 pos)
    {
        foreach (var segment in segmentData)
        {
            if (segment.RB.position == pos && (!segment.IsTail || segmentData.Count == 1))
            {
                return true;
            }
        }
        return false;
    }

    private void CheckForHole()
    {
        if (GridManager.instance.IsHole(transform.position))
        {
            foreach(var segment in segmentData)
            {
                if (!GridManager.instance.IsHole(segment.transform.position))
                    return;
            }
            Die();
        }
    }

    private void Die()
    {
        TransitionManager.instance.EnableUndo(true);
        AudioManager.instance.PlaySFX("fall");
        isDead = true;
        renderer.color = Color.gray;
        foreach(var segment in segmentData)
        {
            segment.Renderer.color = Color.gray;
        }
    }

    private void AddSegment()
    {
        foreach (var existingSegment in segmentData)
        {
            existingSegment.SetTail(false);
        }

        GameObject segmentObject = Instantiate(segmentPrefab);
        Segment segment = segmentObject.GetComponent<Segment>();

        segment.transform.position = lastPosition;

        //do this twice so the segments aren't curved
        segment.SetDirection(lastRotation);
        segment.SetDirection(lastRotation);

        segment.SetTail(true);

        segmentObjects.Add(segmentObject);
        segmentData.Add(segment);
        UpdateSprite();
    }

    public override void UpdateSprite()
    {
        UpdateRotation();
        foreach (var segment in segmentData)
        {
            segment.UpdateSprite();
        }
    }

    private void MoveUp(InputAction.CallbackContext context)
    {
        if (!canMoveUp)
        {
            CantMove();
            return;
        }

        Move(Direction.Up);
    }

    private void MoveDown(InputAction.CallbackContext context)
    {
        if (!canMoveDown)
        {
            CantMove();
            return;
        }

        Move(Direction.Down);
    }

    private void MoveLeft(InputAction.CallbackContext context)
    {
        if (!canMoveLeft)
        {
            CantMove();
            return;
        }

        Move(Direction.Left);
    }

    private void MoveRight(InputAction.CallbackContext context)
    {
        if (!canMoveRight)
        {
            CantMove();
            return;
        }

        Move(Direction.Right);
    }

    private void Undo(InputAction.CallbackContext context)
    {
        if (segmentMoves.Count == 0)
            return;

        UndoLastMove();
    }

    private void UndoLastMove()
    {
        TransitionManager.instance.EnableUndo(false);
        AudioManager.instance.PlaySFX("undo");

        isDead = false;

        LoadLastMoveData();
        foreach (var segment in segmentData)
        {
            segment.LoadLastMoveData();
        }

        CollectableManager.instance.Undo();
    }

    public override void LoadLastMoveData()
    {
        base.LoadLastMoveData();

        canMoveUp = canMoves[^1].Up;
        canMoveDown = canMoves[^1].Down;
        canMoveLeft = canMoves[^1].Left;
        canMoveRight = canMoves[^1].Right;

        canMoves.RemoveAt(canMoves.Count - 1);
    }

    private void Reset(InputAction.CallbackContext context)
    {
        StartCoroutine(ResetToStart());
    }

    IEnumerator ResetToStart()
    {
        reseting = true;

        float resetSpeed = 0.1f;

        while (segmentMoves.Count > 0)
        {
            UndoLastMove();
            yield return new WaitForSeconds(resetSpeed);
            if (resetSpeed > 0.02f)
            {
                resetSpeed -= 0.002f;
            }
        }

        reseting = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Collectable collectable = collision.GetComponent<Collectable>();
        if (collectable != null)
        {
            if (collectable.IsDead)
                return;

            AddSegment();

            //if (segmentData.Count == 1)
            {
                segmentData[^1].transform.position = segmentData[^2].RB.position + DirectionToVector((Direction)(((int)segmentData[^2].PreviousRotation + 2) % 4));
                segmentData[^1].SetDirection(segmentData[^2].PreviousRotation);
            }

            if (collectable.HasArrow)
            EnableDirection(collectable.Direction, false);
            collectable.PickUp();
        }
    }

    private void EnableDirection(Direction direction, bool enable)
    {
        switch (direction)
        {
            case Direction.Up:
                canMoveUp = enable;
                break;
            case Direction.Down:
                canMoveDown = enable;
                break;
            case Direction.Left:
                canMoveLeft = enable;
                break; 
            case Direction.Right:
                canMoveRight = enable;
                break;
        }
    }
}

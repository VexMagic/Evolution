using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : Segment
{
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private int startSegments;

    [SerializeField] private InputActionReference up, down, left, rigth;
    [SerializeField] private bool canMoveUp, canMoveDown, canMoveLeft, canMoveRight;

    private List<GameObject> segmentObjects = new List<GameObject>();
    private List<Segment> segmentData = new List<Segment>();

    private Vector2 lastPosition;

    private void Start()
    {
        lastPosition = rb.position;
        for (int i = 0; i < startSegments; i++)
        {
            AddSegment();
        }
    }

    private void OnEnable()
    {
        up.action.performed += MoveUp;
        down.action.performed += MoveDown;
        left.action.performed += MoveLeft;
        rigth.action.performed += MoveRight;
    }

    private void OnDisable()
    {
        up.action.performed -= MoveUp;
        down.action.performed -= MoveDown;
        left.action.performed -= MoveLeft;
        rigth.action.performed -= MoveRight;
    }

    private void CantMove()
    {
        Debug.Log("Can't Move");
    }

    private void Move(Vector2Int direction)
    {
        Vector2 newPos = rb.position + direction;

        if (CheckForSegment(newPos))
        {
            CantMove();
            return;
        }

        for (int i = segmentObjects.Count - 1; i >= 0; i--)
        {
            if (i == 0)
            {
                segmentData[0].RB.MovePosition(RB.position);
            }
            else
            {
                if (i == segmentObjects.Count - 1)
                {
                    lastPosition = segmentData[i].RB.position;
                }
                segmentData[i].RB.MovePosition(segmentData[i - 1].RB.position);
            }
        }
        rb.MovePosition(newPos);
        UpdateSprite();
    }

    private bool CheckForSegment(Vector2 pos)
    {
        foreach (var segment in segmentData)
        {
            if (segment.RB.position == pos)
            {
                return true;
            }
        }
        return false;
    }

    private void AddSegment()
    {
        foreach (var existingSegment in segmentData)
        {
            existingSegment.SetTail(false);
        }

        GameObject segmentObject = Instantiate(segmentPrefab);
        Segment segment = segmentObject.GetComponent<Segment>();

        Debug.Log(lastPosition);
        segment.RB.MovePosition(lastPosition);

        segment.SetTail(true);

        segmentObjects.Add(segmentObject);
        segmentData.Add(segment);
        UpdateSprite();
    }

    public override void UpdateSprite()
    {
        renderer.color = Color.black;
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

        Move(new Vector2Int(0, 1));
    }

    private void MoveDown(InputAction.CallbackContext context)
    {
        if (!canMoveDown)
        {
            CantMove();
            return;
        }

        Move(new Vector2Int(0, -1));
    }

    private void MoveLeft(InputAction.CallbackContext context)
    {
        if (!canMoveLeft)
        {
            CantMove();
            return;
        }

        Move(new Vector2Int(-1, 0));
    }

    private void MoveRight(InputAction.CallbackContext context)
    {
        if (!canMoveRight)
        {
            CantMove();
            return;
        }

        Move(new Vector2Int(1, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Collectable collectable = collision.GetComponent<Collectable>();
        if (collectable != null)
        {
            AddSegment();
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

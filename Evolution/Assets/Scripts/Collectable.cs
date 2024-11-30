using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private Direction direction;
    [SerializeField] private bool hasArrow = true;
    [SerializeField] private Transform arrow;

    private bool hasMoved;
    private Vector2Int lastMovedDirection;

    public Direction Direction { get { return direction; } }
    public bool HasArrow { get { return hasArrow; } }
    public Vector2Int LastDirection { get { return lastMovedDirection; } }


    private void Start()
    {
        CollectableManager.instance.AddCollectable(this);

        hasMoved = false;

        if (!hasArrow)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        switch (direction)
        {
            case Direction.Up:
                arrow.eulerAngles = new Vector3(0, 0, 0);
                break;
            case Direction.Down:
                arrow.eulerAngles = new Vector3(0, 0, 180);
                break;
            case Direction.Left:
                arrow.eulerAngles = new Vector3(0, 0, 90);
                break;
            case Direction.Right:
                arrow.eulerAngles = new Vector3(0, 0, -90);
                break;
        }
    }

    public void PickUp()
    {
        Destroy(gameObject);
    }

    public void Move(Vector2Int movement)
    {
        lastMovedDirection = movement;
        transform.position += (Vector3)(Vector2)movement;
        hasMoved = true;
        StartCoroutine(MovementBuffer());
    }

    IEnumerator MovementBuffer()
    {
        for (int i = 0; i < 3; i++) 
            yield return new WaitForFixedUpdate();

        hasMoved = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasMoved)
            return;

        Collectable collectable = collision.GetComponent<Collectable>();
        if (collectable != null)
        {
            Move(collectable.LastDirection);
        }
    }

    private void OnDestroy()
    {
        CollectableManager.instance.RemoveCollectable(this);
    }
}

public enum Direction { Up, Left, Down, Right }

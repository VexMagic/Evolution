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
    private bool isDead;

    private List<CollectableMoveData> collectableMoves = new List<CollectableMoveData>();

    public Direction Direction { get { return direction; } }
    public bool HasArrow { get { return hasArrow; } }
    public Vector2Int LastDirection { get { return lastMovedDirection; } }
    public bool IsDead { get { return isDead; } }

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

    public void StoreMoveData()
    {
        collectableMoves.Add(new CollectableMoveData(transform.position, isDead, !gameObject.activeSelf));
    }

    public void LoadLastMoveData()
    {
        transform.position = collectableMoves[^1].Position;
        isDead = collectableMoves[^1].IsDead;
        gameObject.SetActive(!collectableMoves[^1].IsEaten);

        if (IsDead)
            GetComponent<SpriteRenderer>().color = Color.gray;
        else
            GetComponent<SpriteRenderer>().color = Color.white;

        collectableMoves.RemoveAt(collectableMoves.Count - 1);
    }

    public void PickUp()
    {
        AudioManager.instance.PlaySFX("eat");
        gameObject.SetActive(false);
    }

    public void Move(Vector2Int movement)
    {
        lastMovedDirection = movement;
        transform.position += (Vector3)(Vector2)movement;
        hasMoved = true;
        if (GridManager.instance.IsHole(transform.position))
        {
            isDead = true;
            GetComponent<SpriteRenderer>().color = Color.gray;
            GetComponent<SpriteRenderer>().sortingOrder--;
        }
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
        if (hasMoved || isDead)
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

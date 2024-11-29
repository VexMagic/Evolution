using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private Direction direction;
    [SerializeField] private bool hasArrow = true;
    [SerializeField] private Transform arrow;

    public Direction Direction { get { return direction; } }
    public bool HasArrow { get { return hasArrow; } }

    private void Start()
    {
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
}

public enum Direction { Up, Left, Down, Right }

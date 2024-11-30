using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    public static CollectableManager instance;

    private List<Collectable> collectables = new List<Collectable>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }

    public void AddCollectable(Collectable collectable)
    {
        collectables.Add(collectable);
    }

    public void RemoveCollectable(Collectable collectable)
    {
        collectables.Remove(collectable);
    }

    public Collectable CollectableAtPos(Vector2 pos)
    {
        foreach (Collectable collectable in collectables)
        {
            if (collectable.GetComponent<Collider2D>().bounds.Contains((Vector3)pos))
            {
                return collectable;
            }
        }
        return null;
    }

    public void StoreData()
    {
        foreach (var collectable in collectables)
        {
            collectable.StoreMoveData();
        }
    }

    public void Undo()
    {
        foreach (var collectable in collectables)
        {
            collectable.LoadLastMoveData();
        }
    }
}

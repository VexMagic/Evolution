using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableMoveData
{
    public Vector2 Position;
    public bool IsDead;
    public bool IsEaten;

    public CollectableMoveData(Vector2 pos, bool dead, bool eaten) 
    {
        Position = pos;
        IsDead = dead;
        IsEaten = eaten;
    }
}

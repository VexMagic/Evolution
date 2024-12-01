using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanMoveData
{
    public bool Up; 
    public bool Down; 
    public bool Left; 
    public bool Right;

    public CanMoveData(bool up, bool down, bool left, bool right)
    {
        Up = up; 
        Down = down; 
        Left = left; 
        Right = right;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentMoveData
{
    public Vector2 Position;
    public Direction Rotation;
    public Direction PreviousRotation;
    public bool IsTail;

    public SegmentMoveData(Vector2 pos, Direction rot, Direction prevRot, bool isTail)
    {
        Position = pos;
        Rotation = rot;
        PreviousRotation = prevRot;
        IsTail = isTail;
    }
}

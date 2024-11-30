using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Tile Info", menuName = "Tile Info")]
public class TileInfo : ScriptableObject
{
    public enum TileEffect { None, Hole }

    public TileBase[] allTiles;
    public TileEffect effect;
}

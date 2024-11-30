using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    [SerializeField] private Collider2D tilesetCollider;
    [SerializeField] private Tilemap grid;

    private Dictionary<TileBase, TileInfo> dataFromTiles;
    private TileInfo[] allTileData;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        allTileData = Resources.LoadAll<TileInfo>("");
        CreateTileDataDictionary();
    }

    private void CreateTileDataDictionary() //Create the dictionary that's used for getting the tile code
    {
        dataFromTiles = new Dictionary<TileBase, TileInfo>();

        foreach (var tileData in allTileData)
        {
            foreach (var tile in tileData.allTiles)
            {
                if (dataFromTiles.ContainsKey(tile))
                {
                    Debug.LogError(tile.name + " exists in " + dataFromTiles[tile] + " & " + tileData);
                }
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    public bool IsPointInsideWall(Vector2 pos)
    {
        Vector3Int cell = grid.WorldToCell(pos);

        return grid.GetTile<Tile>(cell).colliderType != Tile.ColliderType.None;
    }

    public TileInfo.TileEffect GetPointCellEffect(Vector2 pos)
    {
        Vector3Int cell = grid.WorldToCell(pos);
        TileBase tile = grid.GetTile<TileBase>(cell);

        if (dataFromTiles.ContainsKey(tile))
        {
            return dataFromTiles[tile].effect;
        }
        return TileInfo.TileEffect.None;
    }

    public bool IsHole(Vector2 pos)
    {
        bool isFilled = false;
        Collectable collectable = CollectableManager.instance.CollectableAtPos(pos);
        if (collectable != null)
        {
            isFilled = collectable.IsDead;
        }

        return GetPointCellEffect(pos) == TileInfo.TileEffect.Hole && !isFilled;
    }
}
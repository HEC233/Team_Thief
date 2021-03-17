using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileCoordClass
{
    private static Grid _grid;

    public static void SetGrid(Grid grid)
    {
        _grid = grid;
    }

    public static Vector2Int TileCoord(this Transform tr)
    {
        return TileCoord(tr.position);
    }

    public static Vector2Int TileCoord(this Vector3 vector)
    {
        return new Vector2Int(Mathf.FloorToInt((vector.x - _grid.transform.position.x) / _grid.cellSize.x),
            Mathf.FloorToInt((vector.y - _grid.transform.position.y) / _grid.cellSize.y));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// 찾아보니 Grid 안에 셀좌표와 맵 좌표를 바꿔주는 함수가 이미 존재한다. 그걸로 바꿀까?
// 일단 확장은 유지할 생각이다. 이게 Grid를 몰라도 셸좌표를 알수 있게 패킹해 주니까.
// 그런데 확장하다보니 Vector2Int가 너무 더러워지는 느낌이 있다. 이걸 다른 클래스로 만들어서 뺴야할까?

namespace PS.Util.Tile
{
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
            if (_grid == null)
                return Vector2Int.zero;

            return new Vector2Int(Mathf.FloorToInt((vector.x - _grid.transform.position.x) / _grid.cellSize.x),
                Mathf.FloorToInt((vector.y - _grid.transform.position.y) / _grid.cellSize.y));
        }

        public static Vector3 TileCoordToPosition3(this Vector2Int tileCoord)
        {
            if (_grid == null)
                return Vector3.zero;

            return new Vector3(_grid.transform.position.x + tileCoord.x + (_grid.cellSize.x / 2),
                _grid.transform.position.y + tileCoord.y + (_grid.cellSize.y / 2), 0);
        }
        public static Vector2 TileCoordToPosition(this Vector2Int tileCoord)
        {
            if (_grid == null)
                return Vector2.zero;

            return new Vector2(_grid.transform.position.x + tileCoord.x + (_grid.cellSize.x / 2),
                _grid.transform.position.y + tileCoord.y + (_grid.cellSize.y / 2));
        }

        public static bool CheckObjectExist(this Vector2Int tileCoord, LayerMask layerMask, int deltaX = 0, int deltaY = 0)
        {
            if (_grid == null)
                return false;

            Vector2Int coord = new Vector2Int(tileCoord.x + deltaX, tileCoord.y + deltaY);

            BoxCollider2D collider = new BoxCollider2D();
            bool result = Physics2D.BoxCast(coord.TileCoordToPosition(), _grid.cellSize * 0.9f, 0, Vector2.zero, 0, layerMask).collider != null;

            //if (result)
            //    Debug.Log(coord + " 뭔가 있음");
            //else
            //    Debug.Log(coord + " 뭔가 없음");

            return result;
        }
    }
}
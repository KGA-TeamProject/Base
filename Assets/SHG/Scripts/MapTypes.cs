using System;
using UnityEngine;

static public class MapTypes
{
  public enum MapObjectSize
  {
    Small,
    Medium,
    Large
  }

  public enum TileType
  {
    None,
    Floor,
    Wall,
    Obstacle
  }

  ///<summary> 모든 인접한 타일의 방향 (8방향)</summary>
  public enum TileDirection
  {
    Top,
    Left,
    Right,
    Bottom,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
  }
  
  ///<summary> 모든 인접한 타일의 좌표 상대값 </summary>
  static public Vector2Int[] AllTileDirections;

  static MapTypes()
  {
    var allDirections = (TileDirection[])Enum.GetValues(typeof(TileDirection));
    MapTypes.AllTileDirections = new Vector2Int[allDirections.Length];
    MapTypes.AllTileDirections[(int)TileDirection.Top] = new (0, 1);
    MapTypes.AllTileDirections[(int)TileDirection.Left] = new (-1, 0);
    MapTypes.AllTileDirections[(int)TileDirection.Right] = new (1, 0);
    MapTypes.AllTileDirections[(int)TileDirection.Bottom] = new (0, -1);
    MapTypes.AllTileDirections[(int)TileDirection.TopLeft] = new (-1, 1);
    MapTypes.AllTileDirections[(int)TileDirection.TopRight] = new (1, 1);
    MapTypes.AllTileDirections[(int)TileDirection.BottomLeft] = new (-1, -1);
    MapTypes.AllTileDirections[(int)TileDirection.BottomRight] = new (1, -1);
  }
}

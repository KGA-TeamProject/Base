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
  public static MapObjectSize[] AllObjectSizes = (MapObjectSize[])Enum.GetValues(typeof(MapTypes.MapObjectSize));

  public enum TileType
  {
    None,
    Floor,
    Wall,
    Obstacle
  }

  public static TileType[] AllTileTypes = (TileType[])Enum.GetValues(
      typeof(TileType));

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
  
  ///<summary> 모든 인접한 타일들의 좌표 상대값 </summary>
  static public Vector2Int[] AllTileDirections;
  ///<summary> 모든 한칸 떨어진 타일들의 좌표 상대값 (16 방향)  </summary>
  static public Vector2Int[] AllTileDirectionsTwoStep;

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
    MapTypes.AllTileDirectionsTwoStep = new Vector2Int[MapTypes.AllTileDirections.Length * 2]; 
    int index = 0;
    for (; index < MapTypes.AllTileDirections.Length; ++index) {
      MapTypes.AllTileDirectionsTwoStep[index] = MapTypes.AllTileDirections[index] * 2; 
    }
    /* C: center, X: target pos, o: filled by doubling one step direction
     * [o][X][ ][x][o]
     * [x][ ][ ][ ][x]
     * [o][ ][C][ ][o]
     * [x][ ][ ][ ][x]
     * [o][x][o][x][o]
     */
    // top
    MapTypes.AllTileDirectionsTwoStep[index++] = new(-1, 2);
    MapTypes.AllTileDirectionsTwoStep[index++] = new(1, 2);
    // bottom
    MapTypes.AllTileDirectionsTwoStep[index++] = new(-1, -2);
    MapTypes.AllTileDirectionsTwoStep[index++] = new(1, -2);
    // left 
    MapTypes.AllTileDirectionsTwoStep[index++] = new(-2, 1);
    MapTypes.AllTileDirectionsTwoStep[index++] = new(-2, -1);
    //right
    MapTypes.AllTileDirectionsTwoStep[index++] = new(2, 1);
    MapTypes.AllTileDirectionsTwoStep[index++] = new(2, -1);
     
  }
}

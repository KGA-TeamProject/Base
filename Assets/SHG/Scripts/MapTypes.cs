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
  static public TileDirection[] AllTileDirection = (TileDirection[])Enum.GetValues(typeof(TileDirection));

  static public TileDirection[] AllPerpendicularDirections = {
    TileDirection.Top,
    TileDirection.Bottom,
    TileDirection.Left,
    TileDirection.Right,
  };

  static public bool IsPerpendicular(TileDirection dir)
  {
    switch (dir) 
    {
      case TileDirection.Top:
      case TileDirection.Bottom:
      case TileDirection.Left:
      case TileDirection.Right:
        return (true);
      default:
        return (false);
    }
  }

  static public TileDirection GetOppositeDir(TileDirection dir)
  {
    switch (dir) {
      case TileDirection.Top:
        return (TileDirection.Bottom);
      case TileDirection.Bottom:
        return (TileDirection.Top);
      case TileDirection.Left:
        return (TileDirection.Right);
      case TileDirection.Right:
        return (TileDirection.Left);
      case TileDirection.TopLeft:
        return (TileDirection.BottomRight);
      case TileDirection.TopRight:
        return (TileDirection.BottomLeft);
      case TileDirection.BottomLeft:
        return (TileDirection.TopRight);
      case TileDirection.BottomRight:
        return (TileDirection.TopLeft);
      default:
        throw (new ApplicationException("GetOppositeDir"));
    }
  }
  
  ///<summary> 모든 인접한 타일들의 좌표 상대값 </summary>
  static public Vector2Int[] AllTileDirectionsOneStep;
  ///<summary> 모든 한칸 떨어진 타일들의 좌표 상대값 (16 방향)  </summary>
  static public Vector2Int[] AllTileDirectionsTwoStep;

  static MapTypes()
  {
    var allDirections = (TileDirection[])Enum.GetValues(typeof(TileDirection));
    MapTypes.AllTileDirectionsOneStep = new Vector2Int[allDirections.Length];
    MapTypes.AllTileDirectionsOneStep[(int)TileDirection.Top] = new (0, 1);
    MapTypes.AllTileDirectionsOneStep[(int)TileDirection.Left] = new (-1, 0);
    MapTypes.AllTileDirectionsOneStep[(int)TileDirection.Right] = new (1, 0);
    MapTypes.AllTileDirectionsOneStep[(int)TileDirection.Bottom] = new (0, -1);
    MapTypes.AllTileDirectionsOneStep[(int)TileDirection.TopLeft] = new (-1, 1);
    MapTypes.AllTileDirectionsOneStep[(int)TileDirection.TopRight] = new (1, 1);
    MapTypes.AllTileDirectionsOneStep[(int)TileDirection.BottomLeft] = new (-1, -1);
    MapTypes.AllTileDirectionsOneStep[(int)TileDirection.BottomRight] = new (1, -1);
    MapTypes.AllTileDirectionsTwoStep = new Vector2Int[MapTypes.AllTileDirectionsOneStep.Length * 2]; 
    int index = 0;
    for (; index < MapTypes.AllTileDirectionsOneStep.Length; ++index) {
      MapTypes.AllTileDirectionsTwoStep[index] = MapTypes.AllTileDirectionsOneStep[index] * 2; 
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

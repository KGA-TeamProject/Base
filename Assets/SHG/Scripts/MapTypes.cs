using System.Collections;
using System.Collections.Generic;
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
    ObstacleSmall
  }

  public enum TileEdge
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

}

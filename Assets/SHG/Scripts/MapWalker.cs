using System;
using UnityEngine;

struct MapWalker 
{
  static System.Random Random = new();
  public static float GetRandomPercentage() =>
    (float)MapWalker.Random.Next(0, 100) / 100f;
  public enum Direction
  {
    Up,
    Down,
    Left,
    Right
  }
  public bool IsActive;
  public Direction Dir;
  public Vector2Int Pos;

  static public bool IsInRange(Vector2Int pos, Vector2Int Size) 
  {
    return (pos.x > 0 && pos.y > 0 &&
      pos.x < Size.x &&
      pos.y < Size.y);
  }

  public bool IsInRange(Vector2Int Size) 
  {
    return (this.Pos.x > 0 && this.Pos.y > 0 &&
      this.Pos.x < Size.x &&
      this.Pos.y < Size.y);
  }

  public Direction GetDirection()
  {
    return ((Direction)MapWalker.Random.Next(0, 4));
  }

  public Vector2Int ProgressIn(Vector2Int MapSize)
  {
    var newPos = this.Pos;
    switch (this.Dir) {
      case Direction.Up:
        newPos.y += 1;
        break;
      case Direction.Down:
        newPos.y -= 1;
        break;
      case Direction.Left:
        newPos.x -= 1;
        break;
      case Direction.Right:
        newPos.x += 1;
        break;
    }
    newPos.x = Math.Clamp(newPos.x, 2, MapSize.x - 2);
    newPos.y = Math.Clamp(newPos.y, 2, MapSize.y - 2);
    return (newPos);
  }

  public Direction Redirect()
  {
    var dir = this.GetDirection();
    while (dir == this.Dir) {
      dir = this.GetDirection();
    }
    return (dir);
  }

  public int CountNeigborTile(MapTypes.TileType[] tilesToCount, MapTypes.TileType[] map, Vector2Int MapSize)
  {
    var count = 0;
    

    return (count);
  }
}

using System;
using System.Collections;
using UnityEngine;

public class TileMapGenerator 
{
  [Serializable]
  public class Config 
  {
    [Range(0f, 1.0f)]
    public float ChanceToCreateWalker; 
    [Range(0f, 1.0f)]
    public float ChanceToRemoveWalker;
    [Range(0f, 1.0f)]
    public float ChanceToRedirect;
    public Vector2Int MapSize;
    public Vector2Int StartPos;
    [Range(5, 20)]
    public int WalkerMaximum;
    [Range(0f, 1.0f)]
    public float FloorPercentage;
    [Range(1000, 10000)]
    public int maxIteration;
    public Config(float chanceToCreate, float chanceToRedirect, float chanceToRemove, Vector2Int mapSize, Vector2Int startPos, float floorPercentage, int walkerMaximum = 10, int maxIteration = 100000)
    {
      this.ChanceToCreateWalker = chanceToCreate;
      this.ChanceToRemoveWalker = chanceToRemove;
      this.ChanceToRedirect = chanceToRedirect;
      this.MapSize = mapSize;
      this.StartPos = startPos;
      this.FloorPercentage = floorPercentage;
      this.WalkerMaximum  = walkerMaximum;
      this.maxIteration = maxIteration;
    }
  }

  public enum Tile
  {
    None,
    Floor,
    Wall,
    ObstacleSmall
  }

  public enum Edge
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

  public Config config { get; private set; }
  public Tile[,] tiles { get; private set; }
  public Vector2Int[] EdgePositions { get; private set; }
  public Vector2Int CenterPosition { get; private set; }
  Walker[] walkers;
  int numberOfActiveWalkers = 0;
  int floorCount = 0;
  int maxFloorCount;
  int iteration = 0;
  int width;
  int height;

  public TileMapGenerator(Config config) 
  {
    this.config = config;
    this.Init();
  }

  void Init()
  {
    this.EdgePositions = new Vector2Int[Enum.GetValues(typeof(Edge)).Length];
    for (int i = 0; i < this.EdgePositions.Length; ++i) {
      this.EdgePositions[i] = this.config.StartPos;
    }
    this.width = this.config.MapSize.x;
    this.height = this.config.MapSize.y;
    this.walkers = new Walker[this.config.WalkerMaximum];
    this.tiles = new Tile[this.config.MapSize.x, this.config.MapSize.y];
    this.maxFloorCount = (int)((float)(this.config.MapSize.x * this.config.MapSize.y) * this.config.FloorPercentage);
  }

  public IEnumerator Generate(Action callback = null) 
  {
    var walker = this.AwakeWalker(this.config.StartPos); 
    while (this.floorCount < this.maxFloorCount && 
        this.iteration < this.config.maxIteration) {
      this.CreateFloors();
      if (this.floorCount >= this.maxFloorCount) {
        break;
      }
      this.RandomlyRemoveWalker();
      this.RandomlyRedirect();
      this.RandomlyCreateWalker();
      this.ProgressWalkers();
      this.iteration += 1;
    }
    this.FillWalls();
    yield return (null);
    this.FillHoles();
    this.SetCenter();
    yield return (null);
    callback?.Invoke();
  }

  void FillWalls()
  {
    var topLeft = new Vector2Int(0, this.config.MapSize.y - 1);
    var topRight = new Vector2Int(this.config.MapSize.x - 1, this.config.MapSize.y - 1);
    var bottomLeft = new Vector2Int(0, 0);
    var bottomRight = new Vector2Int(this.config.MapSize.x - 1, 0);
    bool[,] visited = new bool[this.config.MapSize.y, this.config.MapSize.x];
    this.FillWallFrom(topLeft, visited);
    this.FillWallFrom(topRight, visited);
    this.FillWallFrom(bottomLeft, visited);
    this.FillWallFrom(bottomRight, visited);
  }

  void FillWallFrom(Vector2Int pos, bool[,] visited)
  {
    if (visited[pos.y, pos.x]) {
      return ;
    }
    visited[pos.y, pos.x] = true;
    bool isNearFloor = false;
    bool isWall = this.IsTileType(Tile.Wall, pos);
    for (int i = -1; i < 2; ++i) {
      for (int j = -1; j < 2; ++j) {
        var cur = new Vector2Int(pos.x + i, pos.y + j);
        if ((i != 0 || j != 0) &&
            this.IsInRange(cur)) {
          if (this.IsTileType(Tile.Floor, cur)) {
            isNearFloor = true;
          }
          else if (!isWall) {
            this.FillWallFrom(cur, visited);
          }
        }
      }
    }
    if (isNearFloor && !this.IsTileType(Tile.Floor, pos)) {
      this.SetTile(Tile.Wall, pos);
    }
  }

  bool IsInRange(Vector2Int pos) 
  {
    return (pos.x > 0 && pos.y > 0 &&
      pos.x < this.config.MapSize.x &&
      pos.y < this.config.MapSize.y);
  }

  bool IsTileType(Tile tileType, Vector2Int pos) 
  {
    return (this.tiles[pos.y, pos.x] == tileType);
  }

  void SetTile(Tile tileType, Vector2Int pos) 
  {
    this.tiles[pos.y, pos.x] = tileType;
  }

  void CreateFloors()
  {
    for (int i = 0; i < this.numberOfActiveWalkers; ++i) {
      if (this.floorCount == this.maxFloorCount) {
        return ;
      }
      var pos = this.walkers[i].Pos; 
      if (this.tiles[pos.y, pos.x] == Tile.None) {
        this.SetTile(Tile.Floor, pos);
        this.UpdateEdge(pos);
        this.floorCount += 1;
      }
    }
  }

  void UpdateEdge(Vector2Int pos)
  {
    if (this.EdgePositions[(int)Edge.Top].y < pos.y) {
      this.EdgePositions[(int)Edge.Top] = pos;
    }
    if (this.EdgePositions[(int)Edge.Bottom].y > pos.y) {
      this.EdgePositions[(int)Edge.Bottom] = pos;
    }
    if (this.EdgePositions[(int)Edge.Left].x > pos.x) {
      this.EdgePositions[(int)Edge.Left] = pos;
    }
    if (this.EdgePositions[(int)Edge.Right].x < pos.x) {
      this.EdgePositions[(int)Edge.Right] = pos;
    }
    var topLeft = this.EdgePositions[(int)Edge.TopLeft];
    if (this.width - topLeft.x + topLeft.y < this.width - pos.x + pos.y) {
      this.EdgePositions[(int)Edge.TopLeft] = pos;
    }
    var topRight = this.EdgePositions[(int)Edge.TopRight];
    if (topRight.x + topRight.y < pos.x + pos.y) {
      this.EdgePositions[(int)Edge.TopRight] = pos;
    }
    var bottomLeft = this.EdgePositions[(int)Edge.BottomLeft];
    if (this.width - bottomLeft.x + this.height - bottomLeft.y 
        < this.width - pos.x + height - pos.y) {
      this.EdgePositions[(int)Edge.BottomLeft] = pos;
    }
    var bottomRight = this.EdgePositions[(int)Edge.BottomRight];
    if (bottomRight.x + this.height - bottomRight.y < 
        pos.x + this.height - pos.y) {
      this.EdgePositions[(int)Edge.BottomRight] = pos;
    }
  }

  void FillHoles()
  {
    bool[,] visited = new bool[this.config.MapSize.y, this.config.MapSize.x];
    this.FillHoleFrom(this.config.StartPos, visited);
  }

  void FillHoleFrom(Vector2Int pos, bool[,] visited)
  {
    if (visited[pos.y, pos.x]) {
      return ;
    }
    visited[pos.y, pos.x] = true;
    if (this.IsTileType(Tile.None, pos)) {
      this.SetTile(Tile.Floor, pos);
    }
    for (int i = -1; i < 2; ++i) {
      for (int j = -1; j < 2; ++j) {
        var cur = new Vector2Int(pos.x + i, pos.y + j);
        if ((i != 0 || j != 0) &&
            this.IsInRange(cur) &&
            !this.IsTileType(Tile.Wall, cur)) {
          this.FillHoleFrom(cur, visited);
        }
      }
    }
  }

  void SetCenter()
  {
    var center = new Vector2Int();
    foreach (var edge in this.EdgePositions) {
       center += edge; 
    }
    center /= this.EdgePositions.Length;
    this.CenterPosition = center;
  }

  void RandomlyRemoveWalker()
  {
    var removedWalkers = 0;
    for (int i = 0; i < this.numberOfActiveWalkers; i++) {
      if (this.numberOfActiveWalkers - removedWalkers == 1) {
        break;
      }
      var chance = (float)Walker.Random.Next(0, 100) / 100f;
      if (chance < this.config.ChanceToRemoveWalker) {
        var walker = this.walkers[i];
        walker.IsActive = false;
        this.walkers[i] = walker;
        removedWalkers += 1;
      }
    }
    this.numberOfActiveWalkers -= removedWalkers;
    Array.Sort(this.walkers, (lhs, rhs) => {
      if (lhs.IsActive)
        return (!rhs.IsActive ? -1: 0);
      if (rhs.IsActive)
        return (!lhs.IsActive ? 1: 0);
      return (0);
    });
  }

  void RandomlyRedirect()
  {
    for (int i = 0; i < this.numberOfActiveWalkers; i++) {
      var chance = (float)Walker.Random.Next(0, 100) / 100f;
      if (chance < this.config.ChanceToRedirect) {
        var walker = this.walkers[i];
        walker.Dir = walker.Redirect();
        this.walkers[i] = walker;
      }
    }
  }

  void RandomlyCreateWalker()
  {
    var numberOfWalkers = this.numberOfActiveWalkers;
    for (int i = 0; i < numberOfWalkers; i++) {
      if (this.numberOfActiveWalkers == this.config.WalkerMaximum) {
        break;
      }
      var chance = (float)Walker.Random.Next(0, 100) / 100f;
      if (chance < this.config.ChanceToCreateWalker) {
        var pos = this.walkers[i].Pos;
        this.AwakeWalker(pos);
      }
    }
  }

  void ProgressWalkers()
  {
    for (int i = 0; i < this.numberOfActiveWalkers; i++) {
      var walker = this.walkers[i]; 
      walker.Pos = walker.ProgressIn(this.config.MapSize);
      walkers[i] = walker;
    }
  }

  Walker AwakeWalker(Vector2Int pos) 
  { 
    var walker = this.walkers[this.numberOfActiveWalkers];
    walker.Dir = walker.GetDirection();
    walker.IsActive = true;
    walker.Pos = pos;
    this.walkers[this.numberOfActiveWalkers] = walker;
    this.numberOfActiveWalkers += 1;
    return (walker);
  }
}

struct Walker 
{
  public static System.Random Random = new();

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
  public Direction GetDirection()
  {
    return ((Direction)Walker.Random.Next(0, 4));
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
}

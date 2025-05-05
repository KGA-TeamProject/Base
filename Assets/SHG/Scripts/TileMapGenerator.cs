using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapGenerator 
{
  //[Serializable]
  public class Config 
  {
    [Range(0f, 1.0f)]
    public float ChanceToCreate; 
    [Range(0f, 1.0f)]
    public float ChanceToRemove;
    [Range(0f, 1.0f)]
    public float ChanceToRedirect;
    public Vector2Int MapSize;
    public Vector2Int StartPos;
    [Range(5, 20)]
    public int WalkerMaximum;
    [Range(0f, 1.0f)]
    public float FloorPercentage;
    [Range(1000, 10000)]
    public int MaxIteration;
    public int MinimumStepsForRedirect;
    public Config(float chanceToCreate, float chanceToRedirect, float chanceToRemove, Vector2Int mapSize, Vector2Int startPos, float floorPercentage, int minimumStepsForRedirect, int walkerMaximum = 10, int maxIteration = 100000)
    {
      this.ChanceToCreate = chanceToCreate;
      this.ChanceToRemove = chanceToRemove;
      this.ChanceToRedirect = chanceToRedirect;
      this.MapSize = mapSize;
      this.StartPos = startPos;
      this.FloorPercentage = floorPercentage;
      this.MinimumStepsForRedirect = minimumStepsForRedirect;
      this.WalkerMaximum  = walkerMaximum;
      this.MaxIteration = maxIteration;
    }
  }

  public const int SECTION_SIZE = 11;
  public const int SECTION_MARGIN = 4;
  public Config config { get; private set; }
  public MapTypes.TileType[,] tiles { get; private set; }
  public Vector2Int[] EdgePositions { get; private set; }
  public Vector2Int[] EdgeWallPositions { get; private set; }
  public Vector2Int CenterPosition { get; private set; }
  public List<Vector2Int> SectionCenters { get; private set; }
  public bool[,] SectionMask { get; private set; }
  public bool IsGenerated { get; private set; }

  MapWalker[] walkers;
  int[] walkerStepsAfterRedirect;
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
    this.EdgePositions = new Vector2Int[Enum.GetValues(typeof(MapTypes.TileDirection)).Length];
    this.EdgeWallPositions = new Vector2Int[Enum.GetValues(typeof(MapTypes.TileDirection)).Length];

    for (int i = 0; i < this.EdgePositions.Length; ++i) {
      this.EdgePositions[i] = this.config.StartPos;
    }
    this.width = this.config.MapSize.x;
    this.height = this.config.MapSize.y;
    this.walkers = new MapWalker[this.config.WalkerMaximum];
    this.walkerStepsAfterRedirect = new int[this.config.WalkerMaximum];
    this.tiles = new MapTypes.TileType[this.config.MapSize.x, this.config.MapSize.y];
    this.maxFloorCount = (int)((float)(this.config.MapSize.x * this.config.MapSize.y) * this.config.FloorPercentage);
    this.SectionCenters = new ();
    this.SectionMask = new bool [this.config.MapSize.x, this.config.MapSize.y];
  }

  public IEnumerator Generate(Action callback = null) 
  {
    this.AwakeWalker(this.config.StartPos); 
    this.GenerateFloors();
    yield return (null);
    this.FillWalls();
    yield return (null);
    this.FillHoles();
    yield return (null);
    this.GetSections();
    yield return (null);
    this.SetGeometry();
    this.IsGenerated = true;
    callback?.Invoke();
  }

  public Vector2Int FindSafeStarting()
  {
    var pos = this.CenterPosition;
    var offset = 1;
    while (!this.IsTileType(MapTypes.TileType.Floor, pos)) {
      for (int x = -offset; x < offset + 1; ++x) {
         for (int y = -offset; y < offset + 1; ++y) {
           var cur = new Vector2Int(pos.x + x, pos.y + y);
           if (cur == pos) {
             continue;
           }
           if (this.IsTileType(MapTypes.TileType.Floor, cur)) {
             pos = cur;
             break;
           }
         } 
      } 
      offset += 1;
    }
    return (pos);
  }

  void GetSections()
  {
    bool[,] visited = new bool [this.config.MapSize.x, this.config.MapSize.y];
    this.GetSectionsFrom(this.config.StartPos, visited);
  }

  void GetSectionsFrom(Vector2Int pos, bool[,] visited)
  {
    if (visited[pos.y, pos.x]) {
      return;
    }
    visited[pos.y, pos.x] = true;
    bool hasSpace = this.HasSpaceForSection(pos);
    if (hasSpace) {
      this.MarkSection(pos);
    }
    foreach (var dir in MapTypes.AllTileDirectionsOneStep) {
      if (dir.x * dir.y != 0) {
        continue;
      }
      var candidatePos = pos + ((TileMapGenerator.SECTION_SIZE + TileMapGenerator.SECTION_MARGIN ) * dir);
      if (MapWalker.IsInRange(candidatePos, this.config.MapSize) &&
          !this.SectionMask[candidatePos.y, candidatePos.x] &&
          this.IsTileType(MapTypes.TileType.Floor, candidatePos)) {
        this.GetSectionsFrom(candidatePos, visited);
      }
    }
  }

  bool HasSpaceForSection(Vector2Int pos) 
  {
    for (int x = -TileMapGenerator.SECTION_SIZE / 2; x <= TileMapGenerator.SECTION_SIZE / 2; ++x) {
      for (int y = -TileMapGenerator.SECTION_SIZE / 2; y <= TileMapGenerator.SECTION_SIZE / 2; ++y) {
        var cur = new Vector2Int(pos.x + x, pos.y + y);
        if(this.SectionMask[cur.y, cur.x] ||
          !this.IsTileType(MapTypes.TileType.Floor, cur)) {
          return (false);
        }
      }
    }
    return (true);
  }

  void MarkSection(Vector2Int pos)
  {
    this.SectionCenters.Add(pos);
    for (int x = -TileMapGenerator.SECTION_SIZE / 2; x <= TileMapGenerator.SECTION_SIZE / 2; ++x) {
      for (int y = -TileMapGenerator.SECTION_SIZE / 2; y <= TileMapGenerator.SECTION_SIZE / 2; ++y) {
        this.SectionMask[pos.y + y, pos.x + x] = true;
      }
    }
  }

  void GenerateFloors()
  {
    while (this.floorCount < this.maxFloorCount && 
        this.iteration < this.config.MaxIteration) {
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
    bool isWall = this.IsTileType(MapTypes.TileType.Wall, pos);
    foreach (var dir in MapTypes.AllTileDirectionsOneStep) {
      var cur = pos + dir;
      if (MapWalker.IsInRange(cur, this.config.MapSize)) {
        if (this.IsTileType(MapTypes.TileType.Floor, cur)) {
          isNearFloor = true;
          var edge = dir.x * dir.y == 0 ? Array.FindIndex(this.EdgePositions, (edgePos) => edgePos == cur) : -1;
          if (edge != -1 && this.IsEdgeWallPos(pos, cur, (MapTypes.TileDirection)edge)) {
            this.EdgeWallPositions[edge] = pos;
          }
        }
        else if (!isWall) {
          this.FillWallFrom(cur, visited);
        }
      }
    }
    if (isNearFloor && !this.IsTileType(MapTypes.TileType.Floor, pos)) {
      this.SetTile(MapTypes.TileType.Wall, pos);
    }
  }

  bool IsEdgeWallPos(Vector2Int wallPos, Vector2Int floorPos, MapTypes.TileDirection edgeDir)
  {
    var offset = wallPos - floorPos;
    if (offset == MapTypes.AllTileDirectionsOneStep[(int)edgeDir]) {
      return (true);
    }
    return (false);
  }

  bool IsTileType(MapTypes.TileType tileType, Vector2Int pos) 
  {
    return (this.tiles[pos.y, pos.x] == tileType);
  }

  void SetTile(MapTypes.TileType tileType, Vector2Int pos) 
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
      if (this.tiles[pos.y, pos.x] == MapTypes.TileType.None) {
        this.SetTile(MapTypes.TileType.Floor, pos);
        this.UpdateEdge(pos);
        this.floorCount += 1;
      }
    }
  }

  void UpdateEdge(Vector2Int pos)
  {
    if (this.EdgePositions[(int)MapTypes.TileDirection.Top].y < pos.y) {
      this.EdgePositions[(int)MapTypes.TileDirection.Top] = pos;
    }
    if (this.EdgePositions[(int)MapTypes.TileDirection.Bottom].y > pos.y) {
      this.EdgePositions[(int)MapTypes.TileDirection.Bottom] = pos;
    }
    if (this.EdgePositions[(int)MapTypes.TileDirection.Left].x > pos.x) {
      this.EdgePositions[(int)MapTypes.TileDirection.Left] = pos;
    }
    if (this.EdgePositions[(int)MapTypes.TileDirection.Right].x < pos.x) {
      this.EdgePositions[(int)MapTypes.TileDirection.Right] = pos;
    }
    var topLeft = this.EdgePositions[(int)MapTypes.TileDirection.TopLeft];
    if (this.width - topLeft.x + topLeft.y < this.width - pos.x + pos.y) {
      this.EdgePositions[(int)MapTypes.TileDirection.TopLeft] = pos;
    }
    var topRight = this.EdgePositions[(int)MapTypes.TileDirection.TopRight];
    if (topRight.x + topRight.y < pos.x + pos.y) {
      this.EdgePositions[(int)MapTypes.TileDirection.TopRight] = pos;
    }
    var bottomLeft = this.EdgePositions[(int)MapTypes.TileDirection.BottomLeft];
    if (this.width - bottomLeft.x + this.height - bottomLeft.y 
        < this.width - pos.x + height - pos.y) {
      this.EdgePositions[(int)MapTypes.TileDirection.BottomLeft] = pos;
    }
    var bottomRight = this.EdgePositions[(int)MapTypes.TileDirection.BottomRight];
    if (bottomRight.x + this.height - bottomRight.y < 
        pos.x + this.height - pos.y) {
      this.EdgePositions[(int)MapTypes.TileDirection.BottomRight] = pos;
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
    if (this.IsTileType(MapTypes.TileType.None, pos)) {
      this.SetTile(MapTypes.TileType.Floor, pos);
    }
    foreach (var dir in MapTypes.AllTileDirectionsOneStep) {
      var cur = pos + dir;
      if (MapWalker.IsInRange(cur, this.config.MapSize) &&
          !this.IsTileType(MapTypes.TileType.Wall, cur)) {
        this.FillHoleFrom(cur, visited);
      }
    }
  }

  void SetGeometry()
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
      var chance = MapWalker.GetRandomPercentage();
      if (chance < this.config.ChanceToRemove) {
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
      if (this.walkerStepsAfterRedirect[i] < this.config.MinimumStepsForRedirect) {
        continue;
      }
      var chance = MapWalker.GetRandomPercentage();
      if (chance < this.config.ChanceToRedirect) {
        var walker = this.walkers[i];
        walker.Dir = walker.Redirect();
        this.walkers[i] = walker;
        this.walkerStepsAfterRedirect[i] = 0;
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
      var chance = MapWalker.GetRandomPercentage();
      if (chance < this.config.ChanceToCreate) {
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
      this.walkerStepsAfterRedirect[i] += 1;
      walkers[i] = walker;
    }
  }

  MapWalker AwakeWalker(Vector2Int pos) 
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

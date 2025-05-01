using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjectPlacer 
{
  const int NUMBER_OF_WALKERS = 5;
  [System.Serializable]
  public class Config
  {
    public int NumberOfSmallObject;
    public int NumberOfMediumObject;
    public int NumberOfLargeObject;
    public int StepsBetweenPlacement;
    public float ChanceToCreate; 
    public float ChanceToRedirect;

    public Config(int numberOfSmallObject, int numberOfMediumObject,
        int numberOfLargeObject, int stepsBetweenPlacement, float chanceToCreate, float chanceToRedirect)
    {
      this.NumberOfSmallObject = numberOfSmallObject;
      this.NumberOfMediumObject = numberOfMediumObject;
      this.NumberOfLargeObject = numberOfLargeObject;
      this.StepsBetweenPlacement = stepsBetweenPlacement;
      this.ChanceToCreate = chanceToCreate;
      this.ChanceToRedirect = chanceToRedirect;
    }
  }
  public List<string>[] objectPrefabNames;
  public Vector2Int centerPos;
  public List<(Vector2Int pos, MapTypes.MapObjectSize size, string prefabName)> ObjectPlacement;
  Config config;
  int[] numberOfObjectsBySize;
  int[] maxNumberOfObjectsBySize;
  bool[,] objectPlaced;
  int stepsAfterPlacement = 0;
  MapTypes.TileType[,] map;
  bool[] tilemask;
  MapWalker[] walkers;
  Vector2Int mapSize;

  public MapObjectPlacer(
      Config config,
      MapTypes.TileType[,] map)
  {

    this.config = config;
    this.map = map;
    this.Init();
  }

  public void PlaceObjects()
  {
    var nextSize = MapTypes.MapObjectSize.Small;
    var walkerIndex = 0;
    while (!this.IsFinishPlacingSize(nextSize)) {
      var walker = this.walkers[walkerIndex]; 
      this.MoveWalker(ref walker);
      this.walkers[walkerIndex] = walker;
      walkerIndex = (walkerIndex + 1) % this.walkers.Length;
      if (this.stepsAfterPlacement < this.config.StepsBetweenPlacement) {
        continue;
      }
      if (this.HasChanceToPlace(nextSize, walker)) {
        var prefabName = this.GetRandomObjectPrefab(nextSize);
        this.SetObject(prefabName, walker.Pos, nextSize);
        this.numberOfObjectsBySize[(int)nextSize] += 1;
        nextSize = this.GetNextSize(nextSize);
        this.stepsAfterPlacement = 0;
      }
    }
  }
  public void SetMapCenter(Vector2Int center)
  {
    this.centerPos = center;
    for (int i = 0; i < this.walkers.Length; ++i) {
      this.walkers[i] = this.CreateWalker(centerPos); 
    }
  }

  void Init()
  {
    var numberOfSize = Enum.GetValues(typeof(MapTypes.MapObjectSize)
        ).Length;
    this.walkers = new MapWalker[MapObjectPlacer.NUMBER_OF_WALKERS];
    this.numberOfObjectsBySize = new int[numberOfSize];
    this.maxNumberOfObjectsBySize = new int[numberOfSize];
    this.maxNumberOfObjectsBySize[(int)MapTypes.MapObjectSize.Small] = this.config.NumberOfSmallObject;
    this.maxNumberOfObjectsBySize[(int)MapTypes.MapObjectSize.Medium] = this.config.NumberOfMediumObject;
    this.maxNumberOfObjectsBySize[(int)MapTypes.MapObjectSize.Large] = this.config.NumberOfLargeObject;
    this.objectPrefabNames = Enumerable.Repeat(
      new List<string>(), numberOfSize).ToArray();
    this.mapSize = new Vector2Int(this.map.GetLength(1), this.map.GetLength(0));
    this.objectPlaced = new bool[this.mapSize.x, this.mapSize.y];
    this.ObjectPlacement = new();

    this.tilemask= new bool[Enum.GetValues(typeof(MapTypes.TileType)).Length];
    this.tilemask[(int)MapTypes.TileType.Wall] = true;
    this.tilemask[(int)MapTypes.TileType.Obstacle] = true;
  }

  bool HasChanceToPlace(MapTypes.MapObjectSize size, in MapWalker walker)
  {
    var chance = MapWalker.GetRandomPercentage();
    if (chance > this.config.ChanceToCreate) {
      return (false);
    }
    return (this.IsAbleToPlace(walker.Pos, size));
  }

  void MoveWalker(ref MapWalker walker)
  {
    var chance = MapWalker.GetRandomPercentage();
    if (chance < this.config.ChanceToRedirect) {
      walker.Dir = walker.Redirect();
    }
    var nextPos = walker.ProgressIn(this.mapSize);
    while (
        !MapWalker.IsInRange(nextPos, this.mapSize) ||
        walker.IsFacingTile(MapTypes.TileType.Wall, this.map)) {
      walker.Dir = walker.Redirect();
      nextPos = walker.ProgressIn(this.mapSize);
    }
    walker.Pos = nextPos;
    this.stepsAfterPlacement += 1;
  }

  bool IsFinishPlacing()
  {
    foreach (var objectSize in MapTypes.AllObjectSizes) {
      if (!this.IsFinishPlacingSize(objectSize)) {
        return (false);
      }
    }
    return (true);
  }

  bool IsFinishPlacingSize(MapTypes.MapObjectSize size)
  {
    return (this.numberOfObjectsBySize[(int)size] >= 
        this.maxNumberOfObjectsBySize[(int)size]);
  }

  MapTypes.MapObjectSize GetNextSize(MapTypes.MapObjectSize current)
  {
    var next = current;
    do {
      next = (MapTypes.MapObjectSize)((int)(next + 1) %
          (int)MapTypes.AllObjectSizes.Length);
    } while (this.IsFinishPlacingSize(next) && next != current);
    return (next);
  }


  bool IsAbleToPlace(Vector2Int pos, MapTypes.MapObjectSize size)
  {
    if (this.objectPlaced[pos.y, pos.x]) {
      return (false);
    }
    if (size == MapTypes.MapObjectSize.Small) {
      return (true);
    }
    foreach (var dir in MapTypes.AllTileDirections) {
       var cur = pos + dir; 
       if (MapWalker.IsInRange(cur, this.mapSize) &&
           this.objectPlaced[cur.y, cur.x]) {
         return (false);
       }
    }
    if (size == MapTypes.MapObjectSize.Medium) {
      return (true);
    }
    foreach (var twoStepDir in MapTypes.AllTileDirectionsTwoStep) {
       var cur = pos + twoStepDir; 
       if (MapWalker.IsInRange(cur, this.mapSize) &&
           this.objectPlaced[cur.y, cur.x]) {
         return (false);
       }
    }    
    return (true);
  }

  void SetObject(string prefabName, Vector2Int pos, MapTypes.MapObjectSize size)
  {
    this.ObjectPlacement.Add((pos, size, prefabName));
    this.objectPlaced[pos.y, pos.x] = true;
    if (size == MapTypes.MapObjectSize.Small) {
      return;
    }
    foreach (var dir in MapTypes.AllTileDirections) {
       var cur = pos + dir; 
       if (MapWalker.IsInRange(cur, this.mapSize)) {
         this.objectPlaced[cur.y, cur.x] = true;
       }
    }
    if (size == MapTypes.MapObjectSize.Medium) {
      return ;
    }
    foreach (var dir in MapTypes.AllTileDirectionsTwoStep)
    {
       var cur = pos + dir; 
       if (MapWalker.IsInRange(cur, this.mapSize)) {
         this.objectPlaced[cur.y, cur.x] = true;
       }
    }
  }

  MapWalker CreateWalker(Vector2Int pos)
  {
    var walker = new MapWalker();
    walker.Dir = walker.GetDirection();
    walker.IsActive = true;
    walker.Pos = pos;
    return (walker);
  }

  string GetRandomObjectPrefab(MapTypes.MapObjectSize size)
  {
    var index = UnityEngine.Random.Range(0, this.objectPrefabNames[(int)size].Count);
    return (this.objectPrefabNames[(int)size][index]);
  }
}

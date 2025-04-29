using System;
using System.Collections.Generic;
using UnityEngine;

public class TileMapGenerator 
{
  public class Config 
  {
    public float ChanceToCreateWalker; 
    public float ChanceToRemoveWalker;
    public float ChanceToRedirect;
    public Vector2Int MapSize;
    public Vector2Int StartPos;
    public int WalkerMaximum;
    public float FloorPercentage;
    public Config(float chanceToCreate, float chanceToRedirect, float chanceToRemove, Vector2Int mapSize, Vector2Int startPos, int walkerMaximum, float floorPercentage)
    {
      this.ChanceToCreateWalker = chanceToCreate;
      this.ChanceToRemoveWalker = chanceToRemove;
      this.ChanceToRedirect = chanceToRedirect;
      this.MapSize = mapSize;
      this.StartPos = startPos;
      this.FloorPercentage = floorPercentage;
      this.WalkerMaximum  = walkerMaximum;
    }
  }

  public enum Tile
  {
    None,
    Floor,
    Wall
  }

  public Config config { get; private set; }
  Walker[] walkers;
  Tile[,] tiles;
  int numberOfActiveWalkers = 0;
  int floorCount = 0;
  int maxFloorCount;

  public TileMapGenerator(Config config) 
  {
    this.config = config;
    this.Init();
  }

  void Init()
  {
    this.walkers = new Walker[this.config.WalkerMaximum];
    this.tiles = new Tile[this.config.MapSize.x, this.config.MapSize.y];
    this.maxFloorCount = (int)((float)(this.config.MapSize.x * this.config.MapSize.y) * this.config.FloorPercentage);
  }

  public Tile[,] Generate() 
  {
    var walker = this.AwakeWalker(this.config.StartPos); 
    while (this.floorCount < this.maxFloorCount) {
      this.CreateFloors();
      this.RandomlyRemoveWalker();
      this.RandomlyRedirect();
      this.RandomlyCreateWalker();
      this.ProgressWalkers();
    }
    return (this.tiles);
  }

  void CreateFloors()
  {
    for (int i = 0; i < this.numberOfActiveWalkers; ++i) {
      if (this.floorCount == this.maxFloorCount) {
        return ;
      }
      var pos = this.walkers[i].Pos; 
      if (this.tiles[pos.y, pos.x] == Tile.None) {
        this.tiles[pos.y, pos.x] = Tile.Floor;
        this.floorCount += 1;
      }
    }
  }

  void RandomlyRemoveWalker()
  {
    var removedWalkers = 0;
    for (int i = 0; i < this.numberOfActiveWalkers; i++) {
      if (this.numberOfActiveWalkers == 1) {
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
    newPos.x = Math.Clamp(newPos.x, 1, MapSize.x - 2);
    newPos.y = Math.Clamp(newPos.y, 1, MapSize.y - 2);
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

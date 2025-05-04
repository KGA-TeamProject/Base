using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCorridor: MonoBehaviour
{
  static public float LENGTH = 20;
  const float TURNING_POINT = 0.7f;
  public Vector3 StartPos;
  public Vector3 EndPos;
  public string[] tilePrefabNames;
  public bool IsSpawned { get; private set; } = false;
  public Action OnSpawned;
  public bool IsHorizontal;
  Coroutine spawnRoutine;
  List<GameObject>[] pooledTiles;
  Vector3 current;
  Vector2 mainStep;
  Vector2 subStep;
  bool isWalkingMainStep;
  float currentDist;
  float turningDist;
  bool hasTurned;
  Nullable<(MapTypes.TileDirection dir, Vector2 before)> turnedDirection;

  public void DestroySelf()
  {
    this.IsSpawned = false;
    for (int i = 0; i < this.pooledTiles.Length; ++i) {
      var tileList = this.pooledTiles[i]; 
      if (tileList == null) {
        continue;
      }
      var prefabName = this.tilePrefabNames[i];
      foreach (var tile in tileList) {
        PrefabObjectPool.Shared.ReturnObject(tile, prefabName); 
      } 
    }
    this.pooledTiles = null;
    Destroy(this.gameObject);
  }

  void Awake()
  {
    this.gameObject.name = "MapCorridor";
    this.pooledTiles = new List<GameObject>[MapTypes.AllTileTypes.Length];
    this.hasTurned = false;
  }

  void Start()
  {
    this.current = this.StartPos;
    this.currentDist = Vector3.Distance(this.StartPos, this.EndPos);
    var totalMainDist = this.IsHorizontal ?
      Mathf.Abs(this.EndPos.x - this.StartPos.x) :
      Mathf.Abs(this.EndPos.z - this.StartPos.z);
    this.turningDist = MapCorridor.TURNING_POINT * totalMainDist;
    this.isWalkingMainStep = true;
    this.InitSteps();
    this.StartCoroutine(this.CreateSpawnRoutine());
  }

  void InitSteps()
  {
    Vector2 steps = new ( 
        Mathf.Ceil(this.EndPos.x - this.StartPos.x),
        Mathf.Ceil(this.EndPos.z - this.StartPos.z)
        );
    if (this.IsHorizontal) {
      this.mainStep = new (steps.x > 1 ? 1: -1, 0);
      this.subStep = new (0, steps.y > 1 ? 1: -1);
    }
    else {
      this.mainStep = new (0, steps.y > 1 ? 1: -1);
      this.subStep = new (steps.x > 1 ? 1: -1, 0);
    }
    for (int i = 0; i < this.pooledTiles.Length; ++i) {
       if (this.tilePrefabNames[i] != null) {
         this.pooledTiles[i] = new();
       }
    }
  }

  IEnumerator CreateSpawnRoutine()
  {
    while (Vector3.Distance(this.current, this.EndPos) > 1.5f)
    {
      this.WalkStep(this.isWalkingMainStep ? this.mainStep: this.subStep);

      this.SpwanFloor();
      if (this.turnedDirection == null) {
        this.SpwanWalls();
      }
      else {
        this.SpwanCorner();
      }
      yield return (null);
    }
    this.IsSpawned = true;
    this.spawnRoutine = null;
    if (this.OnSpawned != null) {
      this.OnSpawned.Invoke(); 
    }
    this.spawnRoutine = null;
  }

  void WalkStep(Vector2 step)
  {
    this.turnedDirection = null;
    this.current.x += step.x;
    this.current.z += step.y;
    this.currentDist = Vector3.Distance(this.current, this.EndPos);
    this.TurnIfNeeded();
  }

  void TurnIfNeeded()
  {
    var mainDist = this.IsHorizontal ? Math.Abs(this.current.x - this.EndPos.x):
      Math.Abs(this.current.z - this.EndPos.z) ;
    if (this.isWalkingMainStep && !this.hasTurned &&
        mainDist < this.turningDist
       ) {
      this.isWalkingMainStep = false;
      this.hasTurned = true;
      if (this.IsHorizontal) {
        this.turnedDirection = this.StartPos.z < this.EndPos.z ? 
          (MapTypes.TileDirection.Top, this.mainStep) :
          (MapTypes.TileDirection.Bottom, this.mainStep);
      }
      else {
        this.turnedDirection = this.StartPos.x < this.EndPos.x ?
          (MapTypes.TileDirection.Right, this.mainStep):
          (MapTypes.TileDirection.Left, this.mainStep);
      }
    }
    if (!this.isWalkingMainStep) {
      var subDist = this.IsHorizontal ? Math.Abs(this.current.z - this.EndPos.z): 
        Math.Abs(this.current.x - this.EndPos.x);
      if (subDist < 1.0f) {
        this.isWalkingMainStep = true;
        if (this.IsHorizontal) {
          this.turnedDirection = this.StartPos.x < this.EndPos.x ?
            (MapTypes.TileDirection.Right, this.subStep):
            (MapTypes.TileDirection.Left, this.subStep);
        }
        else {
          this.turnedDirection = this.StartPos.z < this.EndPos.z ?
            (MapTypes.TileDirection.Top, this.subStep) :
            (MapTypes.TileDirection.Bottom, this.subStep);
        }
      }
    }
  }

  void SpwanCorner()
  {
    Vector3 pos1 = this.current;
    Vector3 pos2 = this.current;
    var (dir, beforeTurn) = this.turnedDirection.Value;
    switch (dir) {
      case MapTypes.TileDirection.Top:
        pos1.x += beforeTurn.x > 0 ? 1.0f: -1.0f;
        pos2.z -= 1.0f;
        break;
      case MapTypes.TileDirection.Bottom:
        pos1.x += beforeTurn.x > 0 ? 1.0f: -1.0f;
        pos2.z += 1.0f;
        break;
      case MapTypes.TileDirection.Left:
        pos1.x += 1.0f;
        pos2.z += beforeTurn.y > 0 ? 1.0f: -1.0f;
        break;
      case MapTypes.TileDirection.Right:
        pos1.x -= 1.0f;
        pos2.z += beforeTurn.y > 0 ? 1.0f: -1.0f;
        break;
      default: throw new NotImplementedException();
    }
    this.SetWall(pos1);
    this.SetWall(pos2);
  }

  void SpwanFloor()
  {
    var floorName = this.tilePrefabNames[(int)MapTypes.TileType.Floor];
    var floorTile = PrefabObjectPool.Shared.GetPooledObject(floorName);
    this.SetTile(floorTile, this.current);
    this.pooledTiles[(int)MapTypes.TileType.Floor].Add(floorTile);
  }
  
  void SpwanWalls()
  {
    var pos1 = this.current;
    var pos2 = this.current;
    var offset = this.isWalkingMainStep ? this.subStep: this.mainStep;
    pos1.x += offset.x;
    pos1.z += offset.y;
    pos2.x -= offset.x;
    pos2.z -= offset.y;
    this.SetWall(pos1);
    this.SetWall(pos2);
  }

  void SetWall(Vector3 pos) 
  {
    var wallName = this.tilePrefabNames[(int)MapTypes.TileType.Wall];
    var wall1 = PrefabObjectPool.Shared.GetPooledObject(wallName);
    this.SetTile(wall1, pos);
    var wall2 = PrefabObjectPool.Shared.GetPooledObject(wallName);
    this.SetTile(wall2, new (pos.x, pos.y + 1.0f, pos.z)); 
    this.pooledTiles[(int)MapTypes.TileType.Wall].Add(wall1);
    this.pooledTiles[(int)MapTypes.TileType.Wall].Add(wall2);
  }

  void SetTile(GameObject tile, Vector3 pos)
  {
    tile.transform.SetParent(this.transform);
    tile.transform.position = pos;
    tile.SetActive(true); 
  }

}


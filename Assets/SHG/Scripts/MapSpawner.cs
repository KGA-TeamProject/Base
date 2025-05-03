using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
  public bool IsReady { get; private set; } = false;
  public event Action OnSpawned;
  public Vector3 Center;
  public string[] TilePrefabNames {
    get => this.tilePrefabNames;
    set {
      this.tilePrefabNames = value;
      this.CreatePooledObjectList(value);
    }
  }
  public List<string>[] ObjectPrefabNames {
    get => this.objectPrefabNames;
    set {
      this.objectPrefabNames = value;
      foreach (var prefabNames in value) {
         this.CreatePooledObjectList(prefabNames); 
      }
    }
  }
  public List<string> SectionNames {
    get => this.sectionNames;
    set {
      this.sectionNames = value;
      this.CreatePooledObjectList(value); 
    }
  }
  public (GameObject wallObj, Vector3 wallPos)[] EdgeWalls { get; private set; }

  string[] tilePrefabNames;
  List<string>[] objectPrefabNames;
  List<string> sectionNames;
  MapObjectPlacer.Config placingConfig;
  TileMapGenerator.Config mapConfig;
  const float smallObjectPercentage = 0.005f;
  const float mediumObjectPercentage = 0.002f;
  const float largeObjectPercentage = 0.0001f;
  TileMapGenerator mapGenerator;
  MapObjectPlacer objectPlacer;
  int WallPosY = 1;
  float halfTileHeight = 0.5f;
  Grid grid;
  Vector2Int halfMapSize;
  Coroutine SpawningRoutine;
  Dictionary<string, List<GameObject>> pooledObjects = new();

  public void DestroySelf()
  {
    this.ReleasePooledObjects();
    this.objectPlacer = null;
    Destroy(this.gameObject);
  }

  public Vector3 ConvertTilePos(Vector2Int tilePos, int height = 0)
  {
    var cellPos = new Vector3Int(tilePos.x, tilePos.y, height);  
    var pos = (this.grid.GetCellCenterWorld(cellPos));
    return (pos + this.Center);
  }

  void Awake() 
  {
    this.Init();
  }

  // Start is called before the first frame update
  void Start()
  {
    this.StartCoroutine(this.mapGenerator.Generate(this.OnMapGenerated));
  }

  void Init()
  {
    // TODO: Load object count
    this.grid = this.GetComponent<Grid>();
    this.grid.transform.position = this.Center;
    this.EdgeWalls = new (GameObject, Vector3)[MapTypes.AllTileDirection.Length];
  }

  public void SetTileMap(TileMapGenerator map)
  {
    this.mapGenerator = map;
    this.mapConfig = map.config;
    this.objectPlacer = new MapObjectPlacer(this.mapGenerator.tiles, this.mapGenerator.SectionMask);
    this.objectPlacer.ObjectPrefabNames = this.objectPrefabNames;
    this.objectPlacer.SectionNames = this.SectionNames;
  }

  void OnMapGenerated() 
  { 
    this.objectPlacer.SectionCenters = this.mapGenerator.SectionCenters;
    this.SpawningRoutine = this.StartCoroutine(
      this.CreateSpawningRoutine(() => {
          this.IsReady = true;
          if (this.OnSpawned != null) {
            this.OnSpawned.Invoke();
          }
          this.SpawningRoutine = null;
        })
      );        
  }

  IEnumerator CreateSpawningRoutine(Action OnEnded)
  {
    yield return (this.SpawnTiles());
    var tileCount = this.mapConfig.FloorPercentage * this.mapConfig.MapSize.x * this.mapConfig.MapSize.y;
    this.placingConfig = new (
        numberOfSmallObject: (int)(tileCount * MapSpawner.smallObjectPercentage),
        numberOfMediumObject: (int)(tileCount * MapSpawner.mediumObjectPercentage),
        numberOfLargeObject: (int)(tileCount * MapSpawner.largeObjectPercentage),
        stepsBetweenPlacement: (int)(tileCount / 10),
        chanceToCreate: 0.2f,
        chanceToRedirect: 0.2f
        );
    this.objectPlacer.SetConfig(this.placingConfig);
    this.objectPlacer.SetStartPoints(this.mapGenerator.CenterPosition, this.mapGenerator.EdgePositions);
    yield return (this.objectPlacer.PlaceObjects());
    yield return (this.SpawnObjects());
    if (OnEnded != null) {
      OnEnded.Invoke();
    }
  }

  IEnumerator SpawnTiles() 
  {
    for (int y = 0; y < this.mapGenerator.tiles.GetLength(0); y++) {
      for (int x = 0; x < this.mapGenerator.tiles.GetLength(1); x++) {
        var tile = this.mapGenerator.tiles[y, x];
        if (tile != MapTypes.TileType.None) {
          this.SpawnTile(tile, new(x, y));
        }
      }
      yield return (null);
    }
  }

  void SpawnTile(MapTypes.TileType tileType, Vector2Int pos)
  {
    string groundPrefab = tileType switch {
      MapTypes.TileType.Floor => this.tilePrefabNames[(int)MapTypes.TileType.Floor],
      MapTypes.TileType.Wall => this.tilePrefabNames[(int)MapTypes.TileType.Wall],
      MapTypes.TileType.Obstacle => this.tilePrefabNames[(int)MapTypes.TileType.Floor],
      MapTypes.TileType.None => null,
      _ => null
    };

    if (tileType == MapTypes.TileType.Wall) {
      var wallPrefab = this.tilePrefabNames[(int)MapTypes.TileType.Wall];
      var wall = this.GetPooledObject(wallPrefab); 
      var wallPos = this.ConvertTilePos(pos, this.WallPosY);
      wallPos.y -= this.halfTileHeight;
      var edgeWallIndex = Array.FindIndex(
          this.mapGenerator.EdgeWallPositions, 
            (edgeWallPos) => edgeWallPos == pos);
      if (edgeWallIndex != -1) {
        this.EdgeWalls[(int)edgeWallIndex] = (wall, wallPos);
        groundPrefab = this.tilePrefabNames[(int)MapTypes.TileType.Floor];
      }
      this.PutObject(wall, wallPos);
    }
    var worldPos = this.ConvertTilePos(pos);
    worldPos.y -= this.halfTileHeight;
    var tile = this.GetPooledObject(groundPrefab);
    this.PutObject(tile, worldPos);
  }

  IEnumerator SpawnObjects()
  {
    foreach(var (pos, size, prefabName) in this.objectPlacer.ObjectPlacement) {
      this.PutMapObject(prefabName, pos);
      yield return (null);
    }
  }

  GameObject GetPooledObject(string name)
  {
    var obj = PrefabObjectPool.Shared.GetPooledObject(name);
    this.pooledObjects[name].Add(obj);
    return (obj);
  }

  void PutMapObject(string prefabName, Vector2Int pos)
  {
    var obj = this.GetPooledObject(prefabName);
    var worldPos = this.ConvertTilePos(pos);
    int rotation = UnityEngine.Random.Range(0, 4);
    obj.transform.Rotate(new Vector3(0, rotation * 90, 0));
    this.PutObject(obj, worldPos); 
  }

  void PutObject(GameObject obj, Vector3 pos)
  {
    obj.transform.position = pos;
    obj.transform.SetParent(this.transform);
    obj.SetActive(true);
  }

  void CreatePooledObjectList(IList<string> prefabNames)
  {
    foreach (string prefabName in prefabNames) {
      if (prefabName != null) {
        this.pooledObjects[prefabName] = new();
      }
    }
  }

  void ReleasePooledObjects(params string[] prefabNames)
  {
    var pooledObjects = this.pooledObjects;
    this.pooledObjects = null;
    foreach (var (prefabName, objectList) in pooledObjects) {
      if (objectList == null) {
        continue;
      }
      foreach (var pooledObject in objectList) {
        if (pooledObject != null) {
          PrefabObjectPool.Shared.ReturnObject(pooledObject, prefabName);
        }
      }
    }
  }
}


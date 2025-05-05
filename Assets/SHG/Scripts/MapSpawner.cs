using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
  public bool IsGenerated { get; private set; }
  public bool IsSpawned { get; private set; }
  public event Action OnGenerated;
  public event Action OnSpawned;
  public event Action<MapTypes.TileDirection, Action> OnActivateDoor;
  public Vector3 Center { get; private set; }
  public bool IsStarting;
  public string[] TilePrefabNames {
    get => this.tilePrefabNames;
    set {
      this.tilePrefabNames = value;
      this.CreatePooledObjectList(value);
    }
  }
  public string DoorPrefabName = "FenceDoor";
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
  public (MapCorridorDoor door, Vector3 doorPos)[] Doors { get; private set; }

  string[] tilePrefabNames;
  List<string>[] objectPrefabNames;
  List<string> sectionNames;
  MapObjectPlacer.Config placingConfig;
  TileMapGenerator.Config mapConfig;
  const float SMALL_OBJECT_PERCENTAGE = 0.007f;
  const float MEDIUM_OBJECT_PERCENTAGE = 0.004f;
  const float LARGE_OBJECT_PERCENTAGE = 0.0001f;
  TileMapGenerator mapGenerator;
  MapObjectPlacer objectPlacer;
  int WallPosY = 1;
  float halfTileHeight = 0.5f;
  Grid grid;
  Coroutine SpawningRoutine;
  Dictionary<string, List<GameObject>> pooledObjects = new();
  bool isCenterSet;
  GameObject doorPrefab;

  public void DestroySelf()
  {
    if (this.IsSpawned) {
      this.UnSpawn();
    }
    this.objectPlacer = null;
    this.DestroyDoors();
    Destroy(this.gameObject);
  }

  public void UnSpawn()
  {
    this.IsSpawned = false;
    this.ReleasePooledObjects();
    foreach (var (door, _) in this.Doors) {
      if (door != null) {
        door.Hide(); 
      }
    }
  }

  public void SetCenter(Vector3 pos) {
    if (!this.isCenterSet) {
      this.Center = pos;
      this.isCenterSet = true;
    }
  }

  public List<Vector3> GetFreePositions()
  {
    var positions = this.objectPlacer.FreePositions.ConvertAll(pos => 
        this.ConvertTilePos(pos));
    foreach (var sectionCenter in this.objectPlacer.GetUnUsedSections()) {
      for (int x = -TileMapGenerator.SECTION_SIZE / 2; 
          x < -TileMapGenerator.SECTION_SIZE / 2 + 1; ++x) {
        for (int y = -TileMapGenerator.SECTION_SIZE / 2; 
            y < -TileMapGenerator.SECTION_SIZE / 2 + 1; ++y) {
          positions.Add(new (sectionCenter.x + x, sectionCenter.y + y));
        }
      }
    }
    return (positions);
  }

  public Vector3 ConvertTilePos(Vector2Int tilePos, int height = 0)
  {
    var cellPos = new Vector3Int(tilePos.x, tilePos.y, height);  
    var pos = (this.grid.GetCellCenterWorld(cellPos));
    return (pos + this.Center);
  }

  public void CreateDoor(MapTypes.TileDirection dir)
  {
    this.EdgeWalls[(int)dir].wallObj?.SetActive(false);
    if (this.Doors[(int)dir].door != null) {
      this.Doors[(int)dir].door.SetAsUnActivated();
      return ;
    }
    var container = (GameObject)UnityEngine.GameObject.Instantiate(this.doorPrefab);
    var door = container.GetComponent<MapCorridorDoor>();
    door.Dir = dir;
    this.Doors[(int)dir] = (door, this.EdgeWalls[(int)dir].wallPos);
    door.transform.position = new (
      this.Doors[(int)dir].doorPos.x,
      0,
      this.Doors[(int)dir].doorPos.z
      );
    door.OnActivated += (callback) => this.OnActivateDoor(dir, callback);
  }

  public void Spawn(bool background = false) 
  { 
    this.objectPlacer.UsableSectionCenters = this.mapGenerator.SectionCenters;
    if (!background) {
      this.SpawningRoutine = this.StartCoroutine(
        this.CreateSpawningRoutine(this.OnFisnishSpawn)
      );        
    }
    else {
      this.SpawningRoutine = this.StartCoroutine(
        this.CreateBackgroundSpawningRoutine(this.OnFisnishSpawn)
      );        
    }
  }

  public void SetTileMap(TileMapGenerator map)
  {
    this.mapGenerator = map;
    this.mapConfig = map.config;
    this.objectPlacer = new MapObjectPlacer(this.mapGenerator.tiles, this.mapGenerator.SectionMask);
    this.objectPlacer.IsStarting = this.IsStarting;
    this.objectPlacer.ObjectPrefabNames = this.objectPrefabNames;
    this.objectPlacer.SectionNames = this.SectionNames;
  }

  public Vector2 GetActualSize()
  {
    var top = this.ConvertTilePos(this.mapGenerator.EdgePositions[(int)MapTypes.TileDirection.Top]);
    var bottom = this.ConvertTilePos(this.mapGenerator.EdgePositions[(int)MapTypes.TileDirection.Bottom]);
    var left = this.ConvertTilePos(this.mapGenerator.EdgePositions[(int)MapTypes.TileDirection.Left]);
    var right = this.ConvertTilePos(this.mapGenerator.EdgePositions[(int)MapTypes.TileDirection.Right]);
    return (new (right.x - left.x, top.z - bottom.z));
  }

  void Awake() 
  {
    this.Init();
  }

  // Start is called before the first frame update
  void Start()
  {
    if (!this.mapGenerator.IsGenerated) {
      this.StartCoroutine(
        this.mapGenerator.Generate(() => {
          this.IsGenerated = true;
          this.OnGenerated?.Invoke();
      }));
    }
  }

  void Init()
  {
    this.grid = this.GetComponent<Grid>();
    this.grid.transform.position = this.Center;
    this.EdgeWalls = new (GameObject, Vector3)[MapTypes.AllTileDirection.Length];
    this.Doors = new (MapCorridorDoor, Vector3)[MapTypes.AllTileDirection.Length];
    this.doorPrefab = (GameObject)Resources.Load($"Prefabs/MapObjects/{this.DoorPrefabName}");
  }

  IEnumerator CreateSpawningRoutine(Action OnEnded)
  {
    yield return (this.SpawnTiles());
    var tileCount = this.mapConfig.FloorPercentage * this.mapConfig.MapSize.x * this.mapConfig.MapSize.y;
    this.placingConfig = new (
        numberOfSmallObject: (int)(tileCount * MapSpawner.SMALL_OBJECT_PERCENTAGE),
        numberOfMediumObject: (int)(tileCount * MapSpawner.MEDIUM_OBJECT_PERCENTAGE),
        numberOfLargeObject: (int)(tileCount * MapSpawner.LARGE_OBJECT_PERCENTAGE),
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

  IEnumerator CreateBackgroundSpawningRoutine(Action OnEnded)
  {
    yield return (this.BackgroundSpawnTiles());
    var tileCount = this.mapConfig.FloorPercentage * this.mapConfig.MapSize.x * this.mapConfig.MapSize.y;
    this.placingConfig = new (
        numberOfSmallObject: (int)(tileCount * MapSpawner.SMALL_OBJECT_PERCENTAGE),
        numberOfMediumObject: (int)(tileCount * MapSpawner.MEDIUM_OBJECT_PERCENTAGE),
        numberOfLargeObject: (int)(tileCount * MapSpawner.LARGE_OBJECT_PERCENTAGE),
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
  IEnumerator BackgroundSpawnTiles() 
  {
    for (int y = 0; y < this.mapGenerator.tiles.GetLength(0); y++) {
      for (int x = 0; x < this.mapGenerator.tiles.GetLength(1); x++) {
        var tile = this.mapGenerator.tiles[y, x];
        if (tile != MapTypes.TileType.None) {
          this.SpawnTile(tile, new(x, y));
        }
        if (x % 10 == 0) {
          yield return (null);
        }
      }
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
        if (this.Doors[(int)edgeWallIndex].door != null) {
          wall.SetActive(false);
        }
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

  void OnFisnishSpawn()
  {
    this.IsSpawned = true;
    this.SpawningRoutine = null;
    if (this.OnSpawned != null) {
      this.OnSpawned.Invoke();
    }
  }

  void ReleasePooledObjects(params string[] prefabNames)
  {
    foreach (var (prefabName, objectList) in this.pooledObjects) {
      if (objectList == null) {
        continue;
      }
      foreach (var pooledObject in objectList) {
        PrefabObjectPool.Shared.ReturnObject(pooledObject, prefabName);
      }
    }
    var names = new List<string>(this.pooledObjects.Keys);
    foreach (var prefabName in names) {
      this.pooledObjects[prefabName] = new();
    }
  }
  
  void DestroyDoors()
  {
    for (int i = 0; i < this.Doors.Length; ++i) {
       if (this.Doors[i].door != null) {
         this.Doors[i].door.DestorySelf();
       }
    }
  }
}


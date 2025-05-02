using System;
using System.Collections;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
  public bool IsReady { get; private set; } = false;
  public event Action OnSpawned;
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
  string[] tilePrefabNames;
  Vector2Int halfMapSize;
  Coroutine SpawningRoutine;

  public void SetTilePrefabs(MapTypes.TileType tileType, string prefabName) 
  {
    this.tilePrefabNames[(int)tileType] = prefabName;
  }

  public void ReleaseTilePrefab(params (MapTypes.TileType tileType, string prefabName)[] tiles)
  {
  }

  public void SetObjectPrefab(MapTypes.MapObjectSize size, string prefabName) 
  {
    this.objectPlacer.objectPrefabNames[(int)size].Add(prefabName);
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
    this.tilePrefabNames = new string[Enum.GetValues(typeof(MapTypes.TileType)).Length];
    // TODO: Load object count
    this.grid = this.GetComponent<Grid>();
  }

  public void SetTileMap(TileMapGenerator map)
  {
    this.mapGenerator = map;
    this.mapConfig = map.config;
    this.objectPlacer = new MapObjectPlacer(this.mapGenerator.tiles);
  }

  void OnMapGenerated() 
  {
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
    var tile = PrefabObjectPool.Shared.GetPooledObject(groundPrefab);
    var cellPos = new Vector3Int(pos.x, pos.y, 0);
    var worldPos = this.grid.GetCellCenterWorld(cellPos);
    worldPos.y -= this.halfTileHeight;
    this.PutObject(tile, worldPos);

    if (tileType == MapTypes.TileType.Wall) {
      var wall = PrefabObjectPool.Shared.GetPooledObject(this.tilePrefabNames[(int)MapTypes.TileType.Wall]); 
      var wallPos = this.grid.GetCellCenterWorld(
          new (cellPos.x, cellPos.y, this.WallPosY)
          );
      wallPos.y -= this.halfTileHeight;
      this.PutObject(wall, wallPos);
    }
  }

  IEnumerator SpawnObjects()
  {
    foreach(var (pos, size, prefabName) in this.objectPlacer.ObjectPlacement) {
      this.PutMapObject(prefabName, pos);
      yield return (null);
    }
  }

  void PutMapObject(string prefabName, Vector2Int pos)
  {
    var obj = PrefabObjectPool.Shared.GetPooledObject(prefabName);
    var cellPos = new Vector3Int(pos.x, pos.y, 0);  
    var worldPos = this.grid.GetCellCenterWorld(cellPos);
    this.PutObject(obj, worldPos); 
  }

  void PutObject(GameObject obj, Vector3 pos)
  {
    obj.transform.position = pos;
    obj.transform.SetParent(this.transform);
    obj.SetActive(true);
  }
}


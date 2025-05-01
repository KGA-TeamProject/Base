using System;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
  public bool IsReady { get; private set; } = false;
  //[SerializeField]
  TileMapGenerator.Config mapConfig = new (
      chanceToCreate: 0.4f,
      chanceToRedirect: 0.7f,
      chanceToRemove: 0.15f,
      mapSize: new (100, 100),
      startPos: new (50, 50),
      floorPercentage: 0.35f
      );
  //[SerializeField]
  MapObjectPlacer.Config placingConfig = new(
      numberOfSmallObject: 15,
      numberOfMediumObject: 10,
      numberOfLargeObject: 5,
      stepsBetweenPlacement: 50,
      chanceToCreate: 0.2f,
      chanceToRedirect: 0.2f
      );
  TileMapGenerator mapGenerator;
  MapObjectPlacer objectPlacer;
  int WallPosY = 1;
  float halfTileHeight = 0.5f;
  Grid grid;
  string[] tilePrefabNames;
  Vector2Int halfMapSize;

  public void SetTilePrefabs(MapTypes.TileType tileType, string prefabName) 
  {
    this.tilePrefabNames[(int)tileType] = prefabName;
    PrefabObjectPool.Shared.RegisterByName(prefabName, $"MapTiles/{prefabName}");
  }

  public void ReleaseTilePrefab(params (MapTypes.TileType tileType, string prefabName)[] tiles)
  {
  }

  public void SetObjectPrefab(MapTypes.MapObjectSize size, string prefabName) 
  {
    this.objectPlacer.objectPrefabNames[(int)size].Add(prefabName);
    PrefabObjectPool.Shared.RegisterByName(prefabName, $"MapObjects/{prefabName}");
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

  // Update is called once per frame
  void Update()
  {

  }

  void Init()
  {
    this.tilePrefabNames = new string[Enum.GetValues(typeof(MapTypes.TileType)).Length];
    // TODO: Load object count
    this.halfMapSize = new (this.mapConfig.MapSize.x / 2, this.mapConfig.MapSize.y / 2);
    this.grid = this.GetComponent<Grid>();
    this.mapGenerator = new TileMapGenerator(this.mapConfig);
    this.objectPlacer = new MapObjectPlacer(
        this.placingConfig,
        this.mapGenerator.tiles);
    this.ScalePrefabs();
  }

  void OnMapGenerated() 
  {
    
    this.SpawnTiles();
    this.objectPlacer.SetMapCenter(this.mapGenerator.CenterPosition);
    this.objectPlacer.PlaceObjects();
    this.SpawnObjects();
    this.IsReady = true;
    // TODO: Move Start Game
    GameManager.Shared.StartGame();
  }

  void ScalePrefabs()
  {
    
  }

  void SpawnTiles() 
  {
    for (int y = 0; y < this.mapGenerator.tiles.GetLength(0); y++) {
      for (int x = 0; x < this.mapGenerator.tiles.GetLength(1); x++) {
        var tile = this.mapGenerator.tiles[y, x];
        if (tile != MapTypes.TileType.None) {
          this.SpawnTile(tile, new(x, y));
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

  void SpawnObjects()
  {
    foreach(var (pos, size, prefabName) in this.objectPlacer.ObjectPlacement) {
      this.PutMapObject(prefabName, pos);
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


using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapSpawner : MonoBehaviour
{
  [SerializeField]
  TileMapGenerator.Config config = new (
      chanceToCreate: 0.4f,
      chanceToRedirect: 0.7f,
      chanceToRemove: 0.15f,
      mapSize: new (200, 200),
      startPos: new (100, 100),
      floorPercentage: 0.15f
      );
  Grid grid;
  Dictionary<MapTypes.TileType, string> tilePrefabNames = new();
  TileMapGenerator mapGenerator;
  Vector3 tileSize = new(1.0f, 1.0f, 1.0f);
  int WallPosY = 1;
  Vector2Int halfMapSize;
  bool isMapReady = false;

  public void SetTilePrefabs(MapTypes.TileType tileType, string prefabName) 
  {
    this.tilePrefabNames[tileType] = prefabName;
    PrefabObjectPool.Shared.RegisterByName(prefabName, $"MapTiles/" + prefabName);
  }

  public void ReleaseTilePrefab(params (MapTypes.TileType tileType, string prefabName)[] tiles)
  {
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
    this.halfMapSize = new (this.config.MapSize.x / 2, this.config.MapSize.y / 2);
    this.grid = this.GetComponent<Grid>();
    this.mapGenerator = new TileMapGenerator(this.config);
    this.ScalePrefabs();
  }

  void OnMapGenerated() 
  {
    this.isMapReady = true;
    this.SpawnTiles();
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

  void SpawnTile(MapTypes.TileType tile, Vector2Int pos)
  {
    string groundPrefab = tile switch {
      MapTypes.TileType.Floor => this.tilePrefabNames[MapTypes.TileType.Floor],
      MapTypes.TileType.Wall => this.tilePrefabNames[MapTypes.TileType.Wall],
      MapTypes.TileType.ObstacleSmall => this.tilePrefabNames[MapTypes.TileType.Floor],
      MapTypes.TileType.None => null,
      _ => null
    };
    var tileObj = PrefabObjectPool.Shared.GetPooledObject(groundPrefab);
    var cellPos = new Vector3Int(
        pos.x - this.halfMapSize.x,
        pos.y - this.halfMapSize.y, 0);
    var worldPos = this.grid.GetCellCenterWorld(cellPos);
    this.PutTileObj(tileObj, worldPos);

    if (tile == MapTypes.TileType.Wall) {
      var wallObj = PrefabObjectPool.Shared.GetPooledObject(groundPrefab); 
      var wallPos = this.grid.GetCellCenterWorld(
          new (cellPos.x, cellPos.y, this.WallPosY)
          );
      this.PutTileObj(wallObj, wallPos);
    }
  }

  void PutTileObj(GameObject tileObj, Vector3 pos)
  {
    tileObj.transform.position = pos;
    tileObj.SetActive(true);
    tileObj.transform.SetParent(this.transform);
  }
}


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
  Dictionary<TileMapGenerator.Tile, string> tilePrefabNames = new();
  TileMapGenerator mapGenerator;
  Vector3 tileSize = new(1.0f, 1.0f, 1.0f);
  int WallPosY = 1;
  Vector2Int halfMapSize;
  bool isMapReady = false;

  public void SetTilePrefabs(TileMapGenerator.Tile tileType, string prefabName) 
  {
    this.tilePrefabNames[tileType] = prefabName;
    PrefabObjectPool.Shared.RegisterByName(prefabName, $"MapTiles/" + prefabName);
  }
  public void ReleaseTimePrefab(params (TileMapGenerator.Tile tileType, string prefabName)[] tiles)
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
        if (tile != TileMapGenerator.Tile.None) {
          this.SpawnTile(tile, new(x, y));
        }
      }
    }
  }

  void SpawnTile(TileMapGenerator.Tile tile, Vector2Int pos)
  {
    string prefabName = tile switch {
      TileMapGenerator.Tile.Floor => this.tilePrefabNames[TileMapGenerator.Tile.Floor],
      TileMapGenerator.Tile.Wall => this.tilePrefabNames[TileMapGenerator.Tile.Wall],
      TileMapGenerator.Tile.Obstacle => this.tilePrefabNames[TileMapGenerator.Tile.Floor],
      TileMapGenerator.Tile.None => null,
      _ => null
    };
    var tileObj = PrefabObjectPool.Shared.GetPooledObject(prefabName);
    var cellPos = new Vector3Int(
        pos.x - this.halfMapSize.x,
        pos.y - this.halfMapSize.y, 0);
    var worldPos = this.grid.GetCellCenterWorld(cellPos);
    this.PutTileObj(tileObj, worldPos);

    if (tile == TileMapGenerator.Tile.Wall) {
      var wallObj = PrefabObjectPool.Shared.GetPooledObject(prefabName); 
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


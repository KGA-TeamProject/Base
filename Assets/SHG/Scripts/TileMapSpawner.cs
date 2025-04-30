using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapSpawner : MonoBehaviour
{
  [SerializeField]
  GameObject floorTile;
  [SerializeField]
  GameObject wallTile;
  [SerializeField]
  TileMapGenerator.Config config = new (
      chanceToCreate: 0.4f,
      chanceToRedirect: 0.4f,
      chanceToRemove: 0.15f,
      mapSize: new (100, 100),
      startPos: new (50, 50),
      floorPercentage: 0.3f
      );
  Grid grid;
  TileMapGenerator.Tile [,] tiles;
  TileMapGenerator mapGenerator;
  Vector3 tileSize = new(1.0f, 1.0f, 1.0f);
  int WallPosY = 1;

  void Awake() 
  {
    this.Init();
  }

  // Start is called before the first frame update
  void Start()
  {
    this.tiles = this.mapGenerator.Generate();
    this.SpawnTiles();
  }

  // Update is called once per frame
  void Update()
  {

  }

  void Init()
  {
    this.grid = this.GetComponent<Grid>();
    this.mapGenerator = new TileMapGenerator(this.config);
    this.ScalePrefabs();
    PrefabObjectPool.Shared.RegisterPrefab("floorTile", this.floorTile, 100);
    PrefabObjectPool.Shared.RegisterPrefab("wallTile", this.wallTile, 100);
  }

  void ScalePrefabs()
  {
    
  }

  void SpawnTiles() 
  {
    for (int y = 0; y < this.tiles.GetLength(0); y++) {
      for (int x = 0; x < this.tiles.GetLength(1); x++) {
        var tile = this.tiles[y, x];
        if (tile != TileMapGenerator.Tile.None) {
          this.SpawnTile(tile, new(x, y));
        }
      }
    }
  }

  void SpawnTile(TileMapGenerator.Tile tile, Vector2Int pos)
  {
    GameObject tileObj = tile switch  {
      TileMapGenerator.Tile.Floor => PrefabObjectPool.Shared.GetPooledObject("floorTile"),
      TileMapGenerator.Tile.Wall => PrefabObjectPool.Shared.GetPooledObject("wallTile"),
      TileMapGenerator.Tile.None => null,
      _ => null
    };
    var cellPos = new Vector3Int(pos.x, pos.y, 0);
    if (tile == TileMapGenerator.Tile.Wall) {
      cellPos.z = this.WallPosY;
    }
    var worldPos = this.grid.GetCellCenterWorld( cellPos);
    tileObj.transform.position = worldPos;
    tileObj.SetActive(true);
  }

}


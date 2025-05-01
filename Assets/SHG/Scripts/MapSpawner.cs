using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
  public bool IsReady { get; private set; } = false;
  [SerializeField]
  TileMapGenerator.Config config = new (
      chanceToCreate: 0.4f,
      chanceToRedirect: 0.7f,
      chanceToRemove: 0.15f,
      mapSize: new (200, 200),
      startPos: new (100, 100),
      floorPercentage: 0.15f
      );
  int[] numberOfObjectsBySize;
  int[] maxNumberOfObjectsBySize;
  int WallPosY = 1;
  float halfTileHeight = 0.5f;
  TileMapGenerator mapGenerator;
  Grid grid;
  string[] tilePrefabNames;
  List<string>[] objectPrefabNames;
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
    this.objectPrefabNames[(int)size].Add(prefabName);
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
    var numberOfSize = Enum.GetValues(typeof(MapTypes.MapObjectSize)
        ).Length;
    this.tilePrefabNames = new string[Enum.GetValues(typeof(MapTypes.TileType)).Length];
    this.objectPrefabNames = Enumerable.Repeat(
      new List<string>(), numberOfSize).ToArray();
    this.numberOfObjectsBySize = new int[numberOfSize];
    this.maxNumberOfObjectsBySize = new int[numberOfSize];
    // TODO: Load object count
    this.maxNumberOfObjectsBySize[(int)MapTypes.MapObjectSize.Small] = 5;
    this.halfMapSize = new (this.config.MapSize.x / 2, this.config.MapSize.y / 2);
    this.grid = this.GetComponent<Grid>();
    this.mapGenerator = new TileMapGenerator(this.config);
    this.ScalePrefabs();
  }

  void OnMapGenerated() 
  {
    this.SpawnTiles();
    this.SetMapObjects();
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

  void PutSmallMapObject(string prefabName, Vector2Int pos)
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

  void SetMapObjects()
  {
    if (this.objectPrefabNames[(int)MapTypes.MapObjectSize.Small].Count ==0 ) {
      return ;
    }
    var walker = this.CreateWalker(this.mapGenerator.CenterPosition); 
    while (this.numberOfObjectsBySize[(int)MapTypes.MapObjectSize.Small] < this.maxNumberOfObjectsBySize[(int)MapTypes.MapObjectSize.Small]) {
      var chance = MapWalker.GetRandomPercentage();
      if (chance < this.config.ChanceToRedirect) {
        walker.Dir = walker.Redirect();
      }
      walker.Pos = walker.ProgressIn(this.config.MapSize);
      if (chance < 0.01f) {
        var prefabName = this.GetRandomObjectPrefab(MapTypes.MapObjectSize.Small);
        this.PutSmallMapObject(prefabName, walker.Pos);
        this.numberOfObjectsBySize[(int)MapTypes.MapObjectSize.Small] += 1;
      }
    }
  }

  string GetRandomObjectPrefab(MapTypes.MapObjectSize size)
  {
    var index = UnityEngine.Random.Range(0, this.objectPrefabNames[(int)size].Count);
    return (this.objectPrefabNames[(int)size][index]);
  }

  MapWalker CreateWalker(Vector2Int pos)
  {
    var walker = new MapWalker();
    walker.Dir = walker.GetDirection();
    walker.IsActive = true;
    walker.Pos = pos;
    return (walker);
  }
}


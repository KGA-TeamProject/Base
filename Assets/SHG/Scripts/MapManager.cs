using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MapManager 
{
  public enum State
  {
    None,
    SpawningStartNode,
    SpawningNextNode,
  }
  struct MapNode
  {
    public GameObject Container;
    public MapSpawner Spawner;
    public TileMapGenerator Tilemap;
    public Vector3 Center;

    public MapNode(
        GameObject container,
        TileMapGenerator tilemap,
        Vector3 center,
        string[] tilePrefabNames, 
        List<string>[] objectPrefabNames
        )
    {
      this.Container = container;
      this.Spawner = this.Container.GetComponent<MapSpawner>();
      this.Center = center;
      this.Spawner.Center = center;
      this.Tilemap = tilemap;
      this.Spawner.TilePrefabNames = tilePrefabNames;
      this.Spawner.ObjectPrefabNames = objectPrefabNames;
      this.Spawner.SetTileMap(this.Tilemap);
    }
  }

  //[SerializeField]
  TileMapGenerator.Config mapConfig = new (
      chanceToCreate: 0.4f,
      chanceToRedirect: 0.7f,
      chanceToRemove: 0.15f,
      mapSize: new (100, 100),
      startPos: new (50, 50),
      floorPercentage: 0.35f
      );
  public State CurrentState { get; private set; }
  public Camera minimapCamera { get; private set ;}
  const string CONTAINER_NAME = "MapContainer";
  const string MINIMAP_CAMERA_NAME = "Minimap Camera";
  MapNode startNode;
  GameObject containerPrefab;
  string[] tilePrefabNames;
  public List<string>[] objectPrefabNames;

  public MapManager()
  {
    this.Init();
  }

  void Init()
  {
    var minimapCameraPrefab = (GameObject)Resources.Load("Prefabs/" + MapManager.MINIMAP_CAMERA_NAME);
    this.minimapCamera = Object.Instantiate(minimapCameraPrefab).GetComponent<Camera>();
    this.containerPrefab = (GameObject)Resources.Load("Prefabs/" + MapManager.CONTAINER_NAME);
    this.tilePrefabNames = new string[System.Enum.GetValues(typeof(MapTypes.TileType)).Length];
    var numberOfSize = System.Enum.GetValues(typeof(MapTypes.MapObjectSize)).Length;
    this.objectPrefabNames = Enumerable.Repeat(
      new List<string>(), numberOfSize).ToArray();
  }

  public void SpawnMap()
  {
    this.CurrentState = State.SpawningStartNode;
    this.startNode = this.CreateNode(new Vector3());
    this.startNode.Spawner.OnSpawned += this.OnSpawnedStartNode;
  }

  void OnSpawnedStartNode()
  {
    GameManager.Shared.StartGame();
    this.CurrentState = State.SpawningNextNode;
  }

  MapNode CreateNode(Vector3 center)
  {
    var node = new MapNode(
         Object.Instantiate(containerPrefab),
         new TileMapGenerator(this.mapConfig),
         center,
         this.tilePrefabNames,
         this.objectPrefabNames
        );
    return (node);
  }

  public void SetMapTiles(params (MapTypes.TileType tileType, string prefabName)[] tiles)
  {
    foreach (var (tileType, prefabName) in tiles) {
      this.tilePrefabNames[(int)tileType] = prefabName;
      PrefabObjectPool.Shared.RegisterByName(prefabName, $"MapTiles/{prefabName}", this.InitTile, 100);
    }
  }

  public void SetMapObjects(MapTypes.MapObjectSize size, params string[] prefabNames) 
  {
    foreach (var prefabName in prefabNames) {
      this.objectPrefabNames[(int)size].Add(prefabName);
      PrefabObjectPool.Shared.RegisterByName(prefabName, $"MapObjects/{prefabName}", this.InitMapObject);
    }
  }

  GameObject InitTile(GameObject tile)
  {
    tile.AddComponent<BoxCollider>();
    return (tile);
  }
  
  GameObject InitMapObject(GameObject obj)
  {
    obj.AddComponent<BoxCollider>();
    return (obj);
  }
}


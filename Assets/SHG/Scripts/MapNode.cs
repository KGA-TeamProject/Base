using System;
using System.Collections.Generic;
using UnityEngine;

class MapNode
{
  // Tilemap settings
  const float CHANCE_TO_CREATE = 0.2f;
  const float CHANCE_TO_REDIRECT = 0.4f;
  const float CHANCE_TO_REMOVE = 0.1f;
  const float FLOOR_PERCENTAGE = 0.35f;
  const int MINIMUM_STEPS_FOR_REDIRECT = 5;
  static System.Random rand = new ();
  static int nextId = 0;

  const int MAXIMUM_CONNECTIONS = 3;

  public MapSpawner Spawner;
  public TileMapGenerator Tilemap;
  public bool IsSpawned => this.Spawner != null && this.Spawner.IsSpawned;
  public bool IsDestroyed { get; private set; }
  public Vector2Int MapSize { get; private set; }
  public Action OnGeneratedTilemap;
  public Action OnSpawned;
  public (MapNode node, MapCorridor corridor)[] Connections;
  public MapTypes.RoomType Type;
  public int DepthFromStart;
  public Func<MapNode, MapTypes.TileDirection, bool> OnMoveToNextNode;
  public bool IsStarting;
  public int Id { get; private set; }
  GameObject containerPrefab;
  GameObject Container;
  string[] tilePrefabNames;
  List<string>[] objectPrefabNames;
  List<string> sectionNames;
  int maxConnections;

  public MapNode(
      Vector2Int size,
      MapTypes.RoomType type,
      GameObject containerPrefab,
      string[] tilePrefabNames,
      List<string>[] objectPrefabNames,
      List<string> sectionNames
      )
  {
    this.Id = MapNode.nextId++;
    this.Type = type;
    this.MapSize = size;
    this.Container = null;
    this.Spawner = null;
    this.OnSpawned = null;
    this.IsDestroyed = false;
    this.containerPrefab = containerPrefab;
    this.tilePrefabNames = tilePrefabNames;
    this.objectPrefabNames = objectPrefabNames;
    this.sectionNames = sectionNames;
    this.Init();
  }

  void Init()
  {
    this.maxConnections = MapNode.rand.Next(2, MapNode.MAXIMUM_CONNECTIONS);
    this.Connections = new (MapNode, MapCorridor)[MapTypes.AllPerpendicularDirections.Length];
    this.Tilemap = new TileMapGenerator(this.CreateConfig(this.MapSize)); 
    this.Container = UnityEngine.Object.Instantiate(this.containerPrefab);
    this.Spawner = this.Container.GetComponent<MapSpawner>();
    this.Spawner.IsStarting = this.IsStarting;
    this.Spawner.OnActivateDoor += this.OnActiaveDoor;
    this.Spawner.OnGenerated += () => this.OnGeneratedTilemap?.Invoke();
    this.Spawner.TilePrefabNames = this.tilePrefabNames;
    this.Spawner.ObjectPrefabNames = this.objectPrefabNames;
    this.Spawner.SectionNames = this.sectionNames;
    this.Spawner.SetTileMap(this.Tilemap);
    this.Spawner.OnSpawned += this.OnMapSpawned;
  }

  public void Spawn(bool background)
  {
    this.Spawner.Spawn(background);
  }

  public void DestorySelf()
  {
    if (this.IsDestroyed) {
      return ;
    }
    this.IsDestroyed = true;
    if (this.IsSpawned) {
      this.UnSpawn();
    }
    this.Spawner.DestroySelf();
    this.Tilemap = null;
    this.Spawner = null;
  }

  public void UnSpawn()
  {
    this.Spawner.UnSpawn();
    for (int i = 0; i < this.Connections.Length; ++i) {
      var connection = this.Connections[i];
      if (connection.node != null) {
        connection.node.ResetDoor(MapTypes.GetOppositeDir(
              (MapTypes.TileDirection)i));
      } 
    }
  }

  public void ResetDoor(MapTypes.TileDirection dir)
  {
    if (this.IsSpawned) {
      this.Spawner.CreateDoor(dir);
    }
  }

  public void SetConnection(MapTypes.TileDirection dir, MapNode node, MapCorridor corridor)
  {
    this.Connections[(int)dir] = (node, corridor);
  }

  public bool[] GetRandomDirections()
  {
    bool[] directions = new bool[MapTypes.AllPerpendicularDirections.Length];
    var dirCounts = Array.FindAll(this.Connections, (connection => connection.node != null)).Length;
    for (int i = 0; dirCounts < this.maxConnections &&
        i < directions.Length; ++i) {
      if ((MapTypes.TileDirection)i != MapTypes.TileDirection.Bottom &&
          this.Connections[i].node == null) {
        directions[i] = true;
        dirCounts += 1;
      }
    }
    return (directions);
  }

  TileMapGenerator.Config CreateConfig(Vector2Int mapSize )
  {
    var center = new Vector2Int(mapSize.x / 2, mapSize.y / 2);
    return(new (
          chanceToCreate: CHANCE_TO_CREATE,
          chanceToRedirect: CHANCE_TO_REDIRECT,
          chanceToRemove: CHANCE_TO_REMOVE,
          mapSize: mapSize,
          startPos: center,
          floorPercentage: FLOOR_PERCENTAGE,
          minimumStepsForRedirect: MINIMUM_STEPS_FOR_REDIRECT
          )
        );
  }

  void OnMapSpawned()
  {
    if (this.OnSpawned != null) {
      this.OnSpawned.Invoke();
    }
  }

  public Vector3 GetEdgePosition(MapTypes.TileDirection dir)
  {
    var tilePos = this.Tilemap.EdgePositions[(int)dir];
    var converted = this.Spawner.ConvertTilePos(tilePos);
    return (new (converted.x,
          this.Spawner.Center.y,
           converted.z));
  }

  void OnActiaveDoor(MapTypes.TileDirection dir, Action callback)
  {
    var connection = this.Connections[(int)dir];

    if (connection.node != null &&
        this.OnMoveToNextNode != null &&
        this.OnMoveToNextNode.Invoke(connection.node, dir)) {
      callback?.Invoke();
    }
  }
}


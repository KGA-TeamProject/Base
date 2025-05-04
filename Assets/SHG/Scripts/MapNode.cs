using System;
using System.Collections.Generic;
using UnityEngine;

class MapNode
{
  public enum RoomType
  {
    None,
    Combat
  }
  // Tilemap settings
  const float CHANCE_TO_CREATE = 0.2f;
  const float CHANCE_TO_REDIRECT = 0.4f;
  const float CHANCE_TO_REMOVE = 0.1f;
  const float FLOOR_PERCENTAGE = 0.35f;
  const int MINIMUM_STEPS_FOR_REDIRECT = 5;

  public MapSpawner Spawner;
  public TileMapGenerator Tilemap;
  public Vector3 Center;
  public bool IsSpawned { get; private set; }
  public bool IsDestroyed { get; private set; }
  public Vector3[] EdgePositions;
  public Action OnGeneratedTilemap;
  public Action OnSpawned;
  public (MapNode node, MapCorridor corridor)[] Connections;
  public RoomType Type;
  public Vector2Int MapSize { get; private set; }
  public Action<MapNode, MapTypes.TileDirection> OnMoveToNextNode;
  GameObject containerPrefab;
  GameObject Container;
  string[] tilePrefabNames;
  List<string>[] objectPrefabNames;
  List<string> sectionNames;


  public MapNode(
      Vector2Int size,
      Vector3 center,
      RoomType type,
      GameObject containerPrefab,
      string[] tilePrefabNames,
      List<string>[] objectPrefabNames,
      List<string> sectionNames
      )
  {
    this.Type = type;
    this.MapSize = size;
    this.IsSpawned = false;
    this.Container = null;
    this.Spawner = null;
    this.OnSpawned = null;
    this.IsDestroyed = false;
    this.Center = center;
    this.containerPrefab = containerPrefab;
    this.tilePrefabNames = tilePrefabNames;
    this.objectPrefabNames = objectPrefabNames;
    this.sectionNames = sectionNames;
    this.Init();
  }

  void Init()
  {
    this.EdgePositions = new Vector3[MapTypes.AllTileDirectionsOneStep.Length];
    this.Connections = new (MapNode, MapCorridor)[MapTypes.AllTileDirectionsOneStep.Length];
    this.Tilemap = new TileMapGenerator(this.CreateConfig(this.MapSize)); 
    this.Container = UnityEngine.Object.Instantiate(this.containerPrefab);
    this.Spawner = this.Container.GetComponent<MapSpawner>();
    this.Spawner.OnActivateDoor += this.OnActiaveDoor;
    this.Spawner.OnGenerated += () => this.OnGeneratedTilemap?.Invoke();
    this.Spawner.Center = this.Center;
    this.Spawner.TilePrefabNames = this.tilePrefabNames;
    this.Spawner.ObjectPrefabNames = this.objectPrefabNames;
    this.Spawner.SectionNames = this.sectionNames;
    this.Spawner.SetTileMap(this.Tilemap);
    this.Spawner.OnGenerated += this.SetEdgePositions;
    this.Spawner.OnSpawned += this.OnMapSpawned;
  }

  public void Spawn(bool background)
  {
    this.Spawner.Spawn(background);
  }

  public void DestorySelf()
  {
    this.IsDestroyed = true;
    if (this.IsSpawned) {
      this.UnSpawn();
    }
    this.Tilemap = null;
  }

  public void UnSpawn()
  {
    this.IsSpawned = false;
    this.Spawner.DestroySelf();;
    this.Spawner = null;
  }

  public void SetConnection(MapTypes.TileDirection dir, MapNode node, MapCorridor corridor)
  {
    this.Connections[(int)dir] = (node, corridor);
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
    this.IsSpawned = true; 
    if (this.OnSpawned != null) {
      this.OnSpawned.Invoke();
    }
  }

  void SetEdgePositions()
  {
    foreach (var edge in MapTypes.AllTileDirection) {
      var tilePos = this.Tilemap.EdgePositions[(int)edge];
      this.EdgePositions[(int)edge] = this.Spawner.ConvertTilePos(tilePos);
    }
  }

  bool OnActiaveDoor(MapTypes.TileDirection dir)
  {
    var connection = this.Connections[(int)dir];
    
    if (connection.node == null) {
      return (false);
    }
    if (this.OnMoveToNextNode != null) {
      this.OnMoveToNextNode.Invoke(connection.node, dir);
      return (true);
    } 
    return (false);
  }
}


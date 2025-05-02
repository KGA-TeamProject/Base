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
  const float CHANCE_TO_CREATE = 0.4f;
  const float CHANCE_TO_REDIRECT = 0.7f;
  const float CHANCE_TO_REMOVE = 0.15f;
  const float FLOOR_PERCENTAGE = 0.35f;

  public MapSpawner Spawner;
  public TileMapGenerator Tilemap;
  public Vector3 Center;
  public bool IsSpawned { get; private set; }
  public Vector3[] EdgePositions;
  public Action OnSpawned;
  public (MapNode node, MapCorridor corridor)[] Connections;
  public RoomType Type;
  public Vector2Int MapSize { get; private set; }
  private GameObject Container;

  public MapNode(
      Vector2Int size,
      Vector3 center,
      RoomType type
      )
  {
    this.Type = type;
    this.MapSize = size;
    this.IsSpawned = false;
    this.Container = null;
    this.Spawner = null;
    this.OnSpawned = null;
    this.EdgePositions = new Vector3[MapTypes.AllTileDirectionsOneStep.Length];
    this.Connections = new (MapNode, MapCorridor)[MapTypes.AllTileDirectionsOneStep.Length];
    this.Center = center;
    this.Tilemap = new TileMapGenerator(this.CreateConfig(this.MapSize)); 
  }

  public void Spawn(
      GameObject containerPrefab, 
      string[] tilePrefabNames, 
      List<string>[] objectPrefabNames
      )
  {
    this.Container = UnityEngine.Object.Instantiate(containerPrefab);
    this.Spawner = this.Container.GetComponent<MapSpawner>();
    this.Spawner.Center = this.Center;
    this.Spawner.TilePrefabNames = tilePrefabNames;
    this.Spawner.ObjectPrefabNames = objectPrefabNames;
    this.Spawner.SetTileMap(this.Tilemap);
    this.Spawner.OnSpawned += this.OnMapSpawned;
  }

  public void Hide()
  {
    this.IsSpawned = false;
    this.Spawner.DestorySelf();
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
          floorPercentage: FLOOR_PERCENTAGE
          )
        );
  }

  void OnMapSpawned()
  {
    this.IsSpawned = true; 
    this.SetEdgePositions();
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
}


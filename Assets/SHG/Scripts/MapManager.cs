using System;
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

  public State CurrentState { get; private set; }
  public Camera minimapCamera { get; private set ;}
  const string CONTAINER_NAME = "MapContainer";
  const string MINIMAP_CAMERA_NAME = "Minimap Camera";
  MapNode startNode;
  GameObject containerPrefab;
  string[] tilePrefabNames;
  List<string>[] objectPrefabNames;
  List<string> sectionNames;
  public Vector2Int StartRoomSize = new (50, 50);
  public Vector2Int CombatRoomSize = new (70, 70);

  public MapManager()
  {
    this.Init();
  }

  void Init()
  {
    var minimapCameraPrefab = (GameObject)Resources.Load("Prefabs/" + MapManager.MINIMAP_CAMERA_NAME);
    this.minimapCamera = UnityEngine.Object.Instantiate(minimapCameraPrefab).GetComponent<Camera>();
    this.containerPrefab = (GameObject)Resources.Load("Prefabs/" + MapManager.CONTAINER_NAME);
    this.tilePrefabNames = new string[System.Enum.GetValues(typeof(MapTypes.TileType)).Length];
    var numberOfSize = System.Enum.GetValues(typeof(MapTypes.MapObjectSize)).Length;
    this.objectPrefabNames = Enumerable.Repeat(
      new List<string>(), numberOfSize).ToArray();
    this.sectionNames = new();
  }

  public void SpawnMap()
  {
    this.CurrentState = State.SpawningStartNode;
    this.startNode = this.CreateNode(this.StartRoomSize, new Vector3());
    this.startNode.Spawn(
        this.containerPrefab,
         this.tilePrefabNames,
         this.objectPrefabNames,
         this.sectionNames
        );
    this.startNode.OnSpawned += this.OnSpawnedStartNode;
  }

  public void ReleaseCurrent()
  {
    this.ReleaseMap();
    this.ReleasePrefabs();
  }

  void ReleasePrefabs()
  {
    for (int i = 0; i < this.objectPrefabNames.Length; ++i) {
      foreach (var prefabName in this.objectPrefabNames[i]) {
        PrefabObjectPool.Shared.ReleasePrefab(prefabName);
      }
      this.objectPrefabNames[i] = new();
    }
    for (int i = 0; i < this.tilePrefabNames.Length; ++i) {
      if (this.tilePrefabNames[i] != null) {
        PrefabObjectPool.Shared.ReleasePrefab(this.tilePrefabNames[i]);
      }
      this.tilePrefabNames[i] = null;
    }
    foreach (var prefabName in this.sectionNames) {
      PrefabObjectPool.Shared.ReleasePrefab(prefabName);
    }
    this.sectionNames = new();
  }

  void ReleaseMap()
  {
    var visitedNode = new HashSet<MapNode>(); 
     
    this.ReleaseFromNode(this.startNode, visitedNode);
    this.startNode = null;
  }

  void ReleaseFromNode(MapNode node, HashSet<MapNode> visited)
  {
    if (visited.Contains(node)) {
      return;
    }
    visited.Add(node);
    if (!node.IsDestroyed) {
      node.DestorySelf();
    }
    foreach (var connection in node.Connections) {
      if (connection.corridor != null &&
          connection.corridor.IsSpawned) {
        connection.corridor.DestroySelf();
      }
      if (connection.node != null &&
          !connection.node.IsDestroyed) {
        this.ReleaseFromNode(connection.node, visited);
      }
    }
  }

  void OnSpawnedStartNode()
  {
    GameManager.Shared.StartGame();
    var dir = MapTypes.TileDirection.Bottom;
    this.CurrentState = State.SpawningNextNode;
    var newCenter = this.CalcNewRoomOffsetFrom(this.startNode,
        dir, this.CombatRoomSize);
    var newNode = this.CreateNode(
        this.CombatRoomSize,
        newCenter);
    newNode.Spawn(
        this.containerPrefab,
        this.tilePrefabNames,
        this.objectPrefabNames,
        this.sectionNames
        );
    newNode.OnSpawned += () => this.ConnectNode(this.startNode, newNode, dir);
  }

  void ConnectNode(MapNode a, MapNode b, MapTypes.TileDirection dirFromA)
  {
    var dirFromB = MapTypes.GetOppositeDir(dirFromA);
    var container = new GameObject();
    var corridor = container.AddComponent<MapCorridor>();
    corridor.IsHorizontal = dirFromA == MapTypes.TileDirection.Left || dirFromA == MapTypes.TileDirection.Right;
    corridor.StartPos = a.EdgePositions[(int)dirFromA];
    corridor.StartPos.y = a.Center.y;
    corridor.EndPos = b.EdgePositions[(int)dirFromB];
    corridor.EndPos.y = b.Center.y;
    //FIXME: manually move endpos
    //corridor.EndPos.x += 0.5f;
    corridor.tilePrefabNames = this.tilePrefabNames;
    a.SetConnection(dirFromA, b, corridor);
    b.SetConnection(dirFromB, a, corridor);
    // TODO: create door
    UnityEngine.Object.Destroy(a.Spawner.EdgeWalls[(int)dirFromA].wallObj);
    UnityEngine.Object.Destroy(b.Spawner.EdgeWalls[(int)dirFromB].wallObj);
  }

  Vector3 CalcNewRoomOffsetFrom(MapNode start, MapTypes.TileDirection dir, Vector2Int newSize)
  {
    var pos = start.EdgePositions[(int)dir];

    pos.y = start.Center.y;
    var length = MapCorridor.LENGTH;
    switch (dir) {
      case MapTypes.TileDirection.Top: 
        pos.z += (float)newSize.y + length;
        break;
      case MapTypes.TileDirection.Bottom:
        pos.z -= (float)newSize.y + length;
        break;
      case MapTypes.TileDirection.Left:
        pos.x -= (float)newSize.x + length;
        break;
      case MapTypes.TileDirection.Right:
        pos.x += (float)newSize.x + length;
        break;
      default: throw (new NotImplementedException());
    }
    return new(Mathf.Floor(pos.x), Mathf.Floor(pos.y), Mathf.Floor(pos.z));
  }

  MapNode CreateNode(Vector2Int size, Vector3 center)
  {
    var node = new MapNode(
        size,
        center,
        MapNode.RoomType.None
        );
    return (node);
  }

  public void SetMapTiles(params (MapTypes.TileType tileType, string prefabName)[] tiles)
  {
    foreach (var (tileType, prefabName) in tiles) {
      this.tilePrefabNames[(int)tileType] = prefabName;
      PrefabObjectPool.Shared.RegisterByName(prefabName, $"Prefabs/MapTiles/{prefabName}", this.InitTile, 200);
    }
  }

  public void SetMapObjects(MapTypes.MapObjectSize size, params string[] prefabNames) 
  {
    foreach (var prefabName in prefabNames) {
      this.objectPrefabNames[(int)size].Add(prefabName);
      PrefabObjectPool.Shared.RegisterByName(prefabName, $"Prefabs/MapObjects/{prefabName}", this.InitMapObject, 200);
    }
  }

  public void SetMapSections(string[] names)
  {
    foreach (var prefabName in names) {
      this.sectionNames.Add(prefabName); 
      PrefabObjectPool.Shared.RegisterByName(prefabName, $"Prefabs/MapSections/{prefabName}", null, 10);
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


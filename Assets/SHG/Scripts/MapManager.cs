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
  public Camera MinimapCamera { 
    get {
      if (this.minimapCamera == null) {
        this.minimapCamera = ((GameObject)UnityEngine.Object.Instantiate(this.minimapCameraPrefab)).GetComponent<Camera>();
      }
      return (this.minimapCamera);
    } 
    private set {
      this.minimapCamera = value;
    }
  }
  int MAXIMUM_NODES = 3;
  int currentNodes = 0;
  public Action OnStartingSpawned;
  public Action OnDestinationSpawned;
  public Action OnDestinationUnSpawned;
  bool onFinishSpawnMapIsInvoked;
  const string CONTAINER_NAME = "MapContainer";
  const string MINIMAP_CAMERA_NAME = "Minimap Camera";
  MapNode startNode;
  GameObject containerPrefab;
  GameObject minimapCameraPrefab;
  Camera minimapCamera;
  string[] tilePrefabNames;
  List<string>[] objectPrefabNames;
  List<string> sectionNames;
  public Vector2Int StartRoomSize = new (40, 40);
  public Vector2Int CombatRoomSize = new (50, 50);
  List<MapNode> SpawnedNodes;
  (int depth, MapNode node) furthest;

  public MapManager()
  {
    this.Init();
  }

  public Vector3 GetStaringPos()
  {
    var tileStarting = this.startNode.Tilemap.FindSafeStarting();
    return (this.startNode.Spawner.ConvertTilePos(tileStarting, 1));
  }

  public Vector3 GetFinishPos()
  {
    var positions = (this.furthest.node.Spawner.GetFreePositions());
    return (positions[0]);
  }

  void Init()
  {
    this.minimapCameraPrefab = (GameObject)Resources.Load("Prefabs/" + MapManager.MINIMAP_CAMERA_NAME);
    this.containerPrefab = (GameObject)Resources.Load("Prefabs/" + MapManager.CONTAINER_NAME);
    this.tilePrefabNames = new string[System.Enum.GetValues(typeof(MapTypes.TileType)).Length];
    var numberOfSize = System.Enum.GetValues(typeof(MapTypes.MapObjectSize)).Length;
    this.objectPrefabNames = Enumerable.Repeat(
      new List<string>(), numberOfSize).ToArray();
    this.sectionNames = new();
    this.SpawnedNodes = new();
  }

  public void SpawnMap()
  {
    this.CurrentState = State.SpawningStartNode;
    this.CreateNodes();
  }

  void CreateNodes()
  {
    this.startNode = this.CreateNode(this.StartRoomSize);
    this.startNode.DepthFromStart = 0;
    this.startNode.IsStarting = true;
    this.furthest = (0, this.startNode);
    this.startNode.OnSpawned += () => {
      this.CurrentState = State.None;
      if (!this.onFinishSpawnMapIsInvoked) {
        this.OnFinishSpawnStartNode();
      }
    };
    this.startNode.OnGeneratedTilemap += () => {
      this.SpawnedNodes.Add(this.startNode);
      this.startNode.Spawner.SetCenter(new());
      this.startNode.Spawn(false);
    };
  }

  void OnFinishSpawnStartNode()
  {
    this.OnStartingSpawned?.Invoke();
    this.onFinishSpawnMapIsInvoked = true; 
    HashSet<MapNode> visited = new ();
    this.CreateNeighborNodes(this.startNode, visited, 1);
    for (int i = 0; i < this.startNode.Connections.Length; ++i) {
      var connection = this.startNode.Connections[i];
      if (connection.node != null) {
        this.startNode.Spawner.CreateDoor((MapTypes.TileDirection)i);
      } 
    }
  }

  void CreateNeighborNodes(MapNode startNode, HashSet<MapNode> visited, int depth)
  {
    if (visited.Contains(startNode)) {
      return ;
    }
    visited.Add(startNode);
    var childNodes = this.CreateNeighborNodesNear(startNode);
    foreach (var node in childNodes) {
      node.DepthFromStart = depth;
      if (node.DepthFromStart > this.furthest.depth) {
        this.furthest = (depth, node);
      }
      this.CreateNeighborNodes(node, visited, depth + 1);
    }
  }

  List<MapNode> CreateNeighborNodesNear(MapNode node) 
  {
    List<MapNode> nodes = new();
    var directions = node.GetRandomDirections();
    for (int i = 0; this.currentNodes < this.MAXIMUM_NODES &&
        i < directions.Length; ++i) {
      if (directions[i]) {
        var dir = (MapTypes.TileDirection)i;
        var newNode = this.CreateNeighborNodeFrom(node, dir);
        nodes.Add(newNode);
      }
    }
    return (nodes);
  }

  MapNode CreateNeighborNodeFrom(MapNode node, MapTypes.TileDirection dir)
  {
    var newNode = this.CreateNode(
        this.CombatRoomSize);
    var oppositeDir = MapTypes.GetOppositeDir(dir);
    node.SetConnection(dir, newNode, null);
    newNode.SetConnection(oppositeDir, node, null);
    return (newNode);
  }

  public void ReleaseCurrent()
  {
    this.currentNodes = 0;
    this.SpawnedNodes.Clear();
    this.ReleaseMap();
    this.ReleasePrefabs();
    this.onFinishSpawnMapIsInvoked = false;
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

  void SpawnConnectedNode(MapNode a, MapNode b, MapTypes.TileDirection dirFromA)
  {
    this.LayoffNode(a, b, dirFromA);
    var dirFromB = MapTypes.GetOppositeDir(dirFromA);
    var corridor = this.CreateCorridor(a, b, dirFromA);
    a.SetConnection(dirFromA, b, corridor);
    b.SetConnection(dirFromB, a, corridor);
    b.Spawner.OnSpawned += () => {
      b.Spawner.CreateDoor(dirFromB);
      if (b == this.furthest.node && 
          this.OnDestinationSpawned != null) {
        this.OnDestinationSpawned();
      }
    };
    this.StartSpawnNextNode(b, dirFromA);
  }

  void LayoffNode(MapNode datum, MapNode target, MapTypes.TileDirection dir)
  {
    var size = target.Spawner.GetActualSize();
    var targetCenter = target.Spawner.ConvertTilePos(target.Tilemap.CenterPosition);
    var center = datum.GetEdgePosition(dir);
    center = new (
        MathF.Floor(center.x),
        MathF.Floor(center.y),
        MathF.Floor(center.z)
        );
    targetCenter = new (
        MathF.Floor(targetCenter.x),
        MathF.Floor(targetCenter.y),
        MathF.Floor(targetCenter.z)
        );
    center -= targetCenter;
    switch (dir) {
      case MapTypes.TileDirection.Top:
        center.z += MathF.Round(size.y * 0.5f) + MapCorridor.LENGTH;
        break;
      case MapTypes.TileDirection.Bottom:
        center.z -= (MathF.Round(size.y * 0.5f) + MapCorridor.LENGTH);
        break;
      case MapTypes.TileDirection.Left:
        center.x -= (MathF.Round(size.x * 0.5f) + MapCorridor.LENGTH);
        break;
      case MapTypes.TileDirection.Right:
          center.x += MathF.Round(size.x * 0.5f) + MapCorridor.LENGTH;
        break;
      default: throw (new NotImplementedException());
    }
    target.Spawner.SetCenter(center);
  }

  MapCorridor CreateCorridor(MapNode a, MapNode b, MapTypes.TileDirection dirFromA)
  {
    var dirFromB = MapTypes.GetOppositeDir(dirFromA);
    var container = new GameObject();
    var corridor = container.AddComponent<MapCorridor>();
    corridor.IsHorizontal = dirFromA == MapTypes.TileDirection.Left || dirFromA == MapTypes.TileDirection.Right;
    corridor.StartPos = a.GetEdgePosition(dirFromA);
    corridor.EndPos = b.GetEdgePosition(dirFromB);
    corridor.tilePrefabNames = this.tilePrefabNames;
    return (corridor);
  }

  void StartSpawnNextNode(MapNode nextNode, MapTypes.TileDirection dir)
  {
    this.CurrentState = State.SpawningNextNode;
    nextNode.Spawn(true);
    nextNode.OnSpawned += () => {
      for (int i = 0; i < nextNode.Connections.Length; ++i) {
        var connection = nextNode.Connections[i];
        if (connection.node != null) {
          nextNode.Spawner.CreateDoor((MapTypes.TileDirection)i);
        }
      }
      this.CurrentState = State.None;
    };
  }

  MapNode CreateNode(Vector2Int size)
  {
    this.currentNodes += 1;
    var node = new MapNode(
        size,
        MapNode.RoomType.None,
        this.containerPrefab,
        this.tilePrefabNames,
        this.objectPrefabNames,
        this.sectionNames
        );
    node.OnSpawned += () => {
      this.SpawnedNodes.Add(node);
    };
    node.OnMoveToNextNode += (dest, dir) => { 
      if (this.CurrentState != State.None) {
        return (false);
      }
      for (int i = this.SpawnedNodes.Count - 1 ; i > 0; --i) {
        var spawnedNode = this.SpawnedNodes[i];
        if (spawnedNode != node && spawnedNode != dest) {
          for (int j = 0; j < spawnedNode.Connections.Length; ++j) {
            var connection = spawnedNode.Connections[j];
            if (connection.corridor != null) {
              connection.corridor.DestroySelf();
            }  
            if (connection.node == node || connection.node == dest) {
              connection.node.Spawner.CreateDoor(
                  MapTypes.GetOppositeDir((MapTypes.TileDirection)j));
            }
          }
          spawnedNode.UnSpawn();
          this.SpawnedNodes.RemoveAt(i);
        }
      }
      if (!dest.IsSpawned) {
        this.SpawnConnectedNode(node, dest, dir);
      }
      return (true);
    };
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


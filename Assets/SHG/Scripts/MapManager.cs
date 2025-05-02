using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager 
{
  public enum State
  {
    None,
    GeneratingRoot,
    SpawningRoot,
    GeneratingNext,
    SpawningNext,
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
  GameObject containerPrefab;
  GameObject currentContainer;
  MapSpawner currentSpawner;
  TileMapGenerator currentMap;

  public MapManager()
  {
    this.Init();
  }

  void Init()
  {
    var minimapCameraPrefab = (GameObject)Resources.Load("Prefabs/" + MapManager.MINIMAP_CAMERA_NAME);
    this.minimapCamera = Object.Instantiate(minimapCameraPrefab).GetComponent<Camera>();
    this.currentMap = new TileMapGenerator(this.mapConfig);
    this.containerPrefab = (GameObject)Resources.Load("Prefabs/" + MapManager.CONTAINER_NAME);
    this.CurrentState = State.GeneratingRoot;
    this.currentContainer= Object.Instantiate(containerPrefab);
    this.currentSpawner= this.currentContainer.GetComponent<MapSpawner>();
    this.currentSpawner.SetTileMap(this.currentMap);
    this.currentSpawner.OnSpawned += () => {
      GameManager.Shared.StartGame();
    };
  }

  public void SetMapTiles(params (MapTypes.TileType tileType, string prefabName)[] tiles)
  {
    foreach (var (tileType, prefabName) in tiles) {
      this.currentSpawner.SetTilePrefabs(tileType, prefabName);
      PrefabObjectPool.Shared.RegisterByName(prefabName, $"MapTiles/{prefabName}", this.InitTile, 100);
    }
  }

  public void SetMapObjects(MapTypes.MapObjectSize size, params string[] prefabNames) 
  {
    foreach (var prefabName in prefabNames) {
      this.currentSpawner.SetObjectPrefab(size, prefabName);
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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager 
{
  public Camera minimapCamera { get; private set ;}
  const string CONTAINER_NAME = "MapContainer";
  const string MINIMAP_CAMERA_NAME = "Minimap Camera";
  GameObject container;
  MapSpawner tileMapSpawner;

  public MapManager()
  {
    this.Init();
  }

  void Init()
  {
    var minimapCameraPrefab = (GameObject)Resources.Load("Prefabs/" + MapManager.MINIMAP_CAMERA_NAME);
    this.minimapCamera = Object.Instantiate(minimapCameraPrefab).GetComponent<Camera>();
    var containerPrefab = (GameObject)Resources.Load("Prefabs/" + MapManager.CONTAINER_NAME);
    this.container = Object.Instantiate(containerPrefab);
    this.tileMapSpawner = this.container.GetComponent<MapSpawner>();
  }

  public void SetMapTiles(params (MapTypes.TileType tileType, string prefabName)[] tiles)
  {
    foreach (var (tileType, prefabName) in tiles) {
      this.tileMapSpawner.SetTilePrefabs(tileType, prefabName);
      PrefabObjectPool.Shared.RegisterByName(prefabName, $"MapTiles/{prefabName}", this.InitTile, 100);
    }
  }

  public void SetMapObjects(MapTypes.MapObjectSize size, params string[] prefabNames) 
  {
    foreach (var prefabName in prefabNames) {
      this.tileMapSpawner.SetObjectPrefab(size, prefabName);
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


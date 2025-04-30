using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator 
{
  public Camera minimapCamera { get; private set ;}
  const string CONTAINER_NAME = "MapContainer";
  const string MINIMAP_CAMERA_NAME = "Minimap Camera";
  GameObject container;
  TileMapSpawner tileMapSpawner;

  public MapGenerator()
  {
    this.Init();
  }

  void Init()
  {
    var minimapCameraPrefab = (GameObject)Resources.Load("Prefabs/" + MapGenerator.MINIMAP_CAMERA_NAME);
    this.minimapCamera = Object.Instantiate(minimapCameraPrefab).GetComponent<Camera>();
    var containerPrefab = (GameObject)Resources.Load("Prefabs/" + MapGenerator.CONTAINER_NAME);
    this.container = Object.Instantiate(containerPrefab);
    this.tileMapSpawner = this.container.GetComponent<TileMapSpawner>();
    this.SetMapTiles();
  }

  void SetMapTiles()
  {
    this.tileMapSpawner.SetTilePrefabs(
        (TileMapGenerator.Tile.Floor, "floor_grass"),
        (TileMapGenerator.Tile.Wall, "wall_brick_brown")
        );
  }
}

using System;
using System.Collections.Generic;
using UnityEngine;

// 임시 몬스터 interface
interface IMonster
{
  int Id { get; }
  public event Action<IMonster> OnDie;
}

public class StageManager : Singleton<StageManager> 
{

  public int CurrentStage { get; private set; }
  public event Action OnStageClear;
  HashSet<int> currentStageMonsterIds;
  MapManager map;
  StageConfig config;

  private int nextMonsterId = 0;

  void Awake()
  {
    this.LoadConfigs();
    this.map = new MapManager();
    this.ApplyStageConfig(3);
  }

  void Start()
  {
    UIManager.Shared.MinimapCamera = this.map.minimapCamera;
  }

  void LoadConfigs()
  {
    var json = Resources.Load<TextAsset>("Configs/StageConfigs").text;
    this.config = JsonUtility.FromJson<StageConfig>(json);
  }

  void ApplyStageConfig(int stage)
  {
    var maps = this.config.Maps[stage - 1];
    this.map.SetMapTiles(
        (MapTypes.TileType.Floor, maps.Tiles.Floor),
        (MapTypes.TileType.Wall , maps.Tiles.Wall)
        );
    this.map.SetMapObjects(
        MapTypes.MapObjectSize.Small, maps.Objects.Small
        );
    this.map.SetMapObjects(
        MapTypes.MapObjectSize.Medium, maps.Objects.Medium
        );
    this.map.SetMapObjects(
        MapTypes.MapObjectSize.Large, maps.Objects.Large
        );
  }
  
  void OnClear() 
  {
    this.CurrentStage += 1;
    this.currentStageMonsterIds.Clear();
    if (this.OnStageClear != null) {
      this.OnStageClear.Invoke();
    }
  }

  int RegisterMonster(IMonster monster) 
  {
    var id = this.nextMonsterId;
    this.currentStageMonsterIds.Add(id);
    this.nextMonsterId += 1;
    monster.OnDie += this.RemoveMonster;
    return (id);
  }

  void RemoveMonster(IMonster monster)
  {
    this.currentStageMonsterIds.Remove(monster.Id);
  }
}


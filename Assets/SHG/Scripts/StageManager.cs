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
  MapGenerator map;
  StageConfig config;

  private int nextMonsterId = 0;

  void Awake()
  {
    this.LoadConfigs();
    this.map = new MapGenerator();
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
        (TileMapGenerator.Tile.Floor, maps.Floor),
        (TileMapGenerator.Tile.Wall , maps.Wall)
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

[System.Serializable]
public class StageConfig
{
  public MapConfig[] Maps;
  [System.Serializable]
  public struct MapConfig
  {
    public string Floor;
    public string Wall;
  }
}

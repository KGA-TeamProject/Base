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

  public int CurrentStage => this.currentStage;
  public event Action OnStartStage;
  public event Action OnStageClear;
  [SerializeField]
  int currentStage = 1;
  HashSet<int> currentStageMonsterIds;
  MapManager map;
  StageConfig config;

  private int nextMonsterId = 0;

  void Awake()
  {
    this.LoadConfigs();
    this.map = new MapManager();
  }

  void LoadConfigs()
  {
    var json = Resources.Load<TextAsset>("Configs/StageConfigs").text;
    this.config = JsonUtility.FromJson<StageConfig>(json);
  }

  public void StartStage()
  {
    this.ApplyStageConfig(this.CurrentStage);
    this.map.SpawnMap();
    this.map.OnFinishSpawnMap += () => {
      var player = this.SpawnPlayer();
      if (this.OnStartStage != null) {
        this.OnStartStage.Invoke();
      }
      UIManager.Shared.MinimapCamera = this.map.MinimapCamera;
      UIManager.Shared.combatUI.Player = player.transform;
      Camera.main.GetComponent<MainCamera>().Player = player;
    };
  }

  Transform SpawnPlayer()
  {
    var playerPrefab = Resources.Load<GameObject>("TestPlayer");
    var player = Instantiate(playerPrefab, this.map.GetStaringPos(), Quaternion.identity);
    if (player.GetComponent<Collider>() == null) {
      var collider = player.AddComponent<CapsuleCollider>();
      collider.center = new(0, 0.7f, 0);
      collider.radius = 0.5f;
      collider.height = 1.8f;
    }
    if (player.GetComponent<Rigidbody>() == null) {
      var rigidbody = player.AddComponent<Rigidbody>();
      rigidbody.constraints = RigidbodyConstraints.FreezeRotationX |
        RigidbodyConstraints.FreezeRotationZ;
    }
    if (player.tag != "Player") {
      player.tag = "Player";
    }
    return (player.transform);
  }

  void FinishStage()
  {
    this.map.ReleaseCurrent();
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
    this.map.SetMapSections(maps.Sections);
  }
  
  void OnClear() 
  {
    this.currentStage += 1;
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

  /*
   * Editor
   */
  [InspectorButton("StartStage")]
  public bool StartButton;
  [InspectorButton("FinishStage")]
  public bool EndButton;
}


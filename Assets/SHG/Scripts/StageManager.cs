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
  public const int MAX_STAGE = 3;
  [SerializeField]
  int currentStage = 1;
  HashSet<int> currentStageMonsterIds;
  MapManager map;
  StageConfig config;
  GameObject flagPrefab;
  StageFinisher finisher;

  private int nextMonsterId = 0;

  void Awake()
  {
    this.LoadConfigs();
    this.currentStageMonsterIds = new ();
    this.map = new MapManager();
    this.flagPrefab = Resources.Load<GameObject>("Prefabs/MapObjects/Flag");
  }

  void LoadConfigs() {
    var json = Resources.Load<TextAsset>("Configs/StageConfigs").text;
    this.config = JsonUtility.FromJson<StageConfig>(json);
  }

  public void StartStage()
  {
    this.ApplyStageConfig(this.CurrentStage);
    this.map.SpawnMap();
    this.map.OnStartingSpawned += this.OnStartingMapSpawned;
    this.map.OnDestinationSpawned += this.OnSpawnDestination;
    this.map.OnDestinationUnSpawned += this.OnUnSpawnDestination;
  }

  void OnStartingMapSpawned()
  {
    var player = GameObject.FindWithTag("Player");
    if (player != null) {
      this.ShowPlayer(player);
    }
    else {
      player = this.SpawnPlayer();
    }
    if (this.OnStartStage != null) {
      this.OnStartStage.Invoke();
    }
    player.transform.position = this.map.GetStaringPos();
    UIManager.Shared.MinimapCamera = this.map.MinimapCamera;
    UIManager.Shared.combatUI.Player = player.transform;
    Camera.main.GetComponent<MainCamera>().Player = player.transform;
  }

  void OnSpawnDestination()
  {
    if (this.finisher == null) {
      var pos = this.map.GetFinishPos();
      var flag = Instantiate(this.flagPrefab, pos, Quaternion.identity);
      this.finisher = flag.AddComponent<StageFinisher>();
      this.finisher.OnActivate += () => this.OnClear();
    }
    this.finisher.Show();
  }

  void OnUnSpawnDestination()
  {
    if (this.finisher != null) {
      this.finisher.Show();
    }
  }

  GameObject SpawnPlayer()
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
    return (player);
  }

  void HidePlayer(GameObject player)
  {
    player.SetActive(false);
  }
  
  void ShowPlayer(GameObject player)
  {
    player.SetActive(true);
  }

  void FinishStage()
  {
    this.finisher.DestroySelf();
    this.finisher = null;
    this.HidePlayer(GameObject.FindWithTag("Player"));
    this.map.ReleaseCurrent();
    this.currentStageMonsterIds.Clear();
    if (this.OnStageClear != null) {
      this.OnStageClear.Invoke();
    }
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
    this.FinishStage();
    if (this.currentStage < StageManager.MAX_STAGE) {
      this.currentStage += 1;
      this.StartStage();
    }
    else {
      GameManager.Shared.EndGame();
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
  [InspectorButton("OnClear")]
  public bool NextStageButton;
}


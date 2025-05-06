using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
  GameObject playerPrefab;
  GameObject monsterPrefab;
  [SerializeField]
  int currentStage = 1;
  MapManager map;
  StageConfig config;
  GameObject flagPrefab;
  StageFinisher finisher;
  int MAX_MOSNTERS = 40;
  int MAX_NUMBER_OF_MONSTERS_IN_ROOM = 10;
  int remain_monsters;
  Dictionary<int, HashSet<GameObject>> monstersInRooms;
  Dictionary<int, List<Vector3>> storedMonsterPos;
  System.Random monsterRand = new ();

  private int nextMonsterId = 0;

  void Awake()
  {
    this.LoadConfigs();
    this.map = new MapManager();
    this.playerPrefab = Resources.Load<GameObject>("TestPlayer");
    this.monsterPrefab = Resources.Load<GameObject>("TestMonster");
    this.flagPrefab = Resources.Load<GameObject>("Prefabs/MapObjects/Flag");
    this.map.OnStartingSpawned += this.OnStartingMapSpawned;
    this.map.OnDestinationSpawned += this.OnSpawnDestination;
    this.map.OnDestinationUnSpawned += this.OnUnSpawnDestination;
    this.map.OnRoomSpawned += this.OnRoomSpawned;
    this.map.OnRoomUnSpawned += this.OnRoomUnSpawned;
  }

  void LoadConfigs() {
    var json = Resources.Load<TextAsset>("Configs/StageConfigs").text;
    this.config = JsonUtility.FromJson<StageConfig>(json);
  }

  public void StartStage()
  {
    this.ApplyStageConfig(this.CurrentStage);
    this.ChangeBgm();
    this.map.SpawnMap();
    this.remain_monsters = this.MAX_MOSNTERS;
    this.monstersInRooms = new ();
    this.storedMonsterPos = new ();
  }

  void ChangeBgm()
  {
    var bgmName = this.config.Musics[this.CurrentStage - 1];
    AudioManager.Shared.ChangeBgmTo(bgmName);
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
      var minimapIcon = UIManager.Shared.combatUI.minimap.AddMinimapIconTo(flag, UIManager.Shared.combatUI.minimap.FlagIcon, 5);
      minimapIcon.name = "FlagMinimapIcon";
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
    var player = Instantiate(this.playerPrefab, 
        this.map.GetStaringPos(), Quaternion.identity);
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
    var lifeTimePublisher = player.AddComponent<LifeTimePublisher>();
    lifeTimePublisher.AfterDestroyed = GameManager.Shared.EndGame;
    if (player.tag != "Player") {
      player.tag = "Player";
    }
    UIManager.Shared.combatUI.minimap.AddMinimapIconTo(player, UIManager.Shared.combatUI.minimap.PlayerIcon, 2);
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
    this.finisher?.DestroySelf();
    this.finisher = null;
    this.HidePlayer(GameObject.FindWithTag("Player"));
    foreach (var room in this.monstersInRooms.Keys) {
      this.UnSpawnMonsterAt(room);
    }
    this.monstersInRooms.Clear();
    this.storedMonsterPos.Clear();
    this.map.ReleaseCurrent();
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

  void OnRoomSpawned(MapTypes.RoomType type, int roomId)
  {
    if (type == MapTypes.RoomType.Combat) {
      var positions = this.map.GetFreePositions(roomId);
      if (!this.monstersInRooms.ContainsKey(roomId)) {
        this.monstersInRooms.Add(roomId, new());
        this.SpawnMonsters(roomId, positions);
      }
      else if (this.storedMonsterPos.ContainsKey(roomId)) {
        this.RestoreMonstersInRoom(roomId);
        this.storedMonsterPos.Remove(roomId);
      }
    }
  }
  void OnRoomUnSpawned(MapTypes.RoomType type, int roomId) 
  {
    if (type == MapTypes.RoomType.Combat) {
      this.StoreMonsterPositions(roomId);
      this.UnSpawnMonsterAt(roomId);
    }
  }

  void SpawnMonsters(int roomId, List<Vector3> positions)
  {
    var monsterCount = Math.Min(this.MAX_NUMBER_OF_MONSTERS_IN_ROOM, this.remain_monsters);
    HashSet<Vector3> picked = new(monsterCount);
    for (int i = 0; 
        picked.Count < monsterCount && i < positions.Count; ++i) {
      var pos = positions[this.monsterRand.Next(positions.Count)]; 
      if (!picked.Contains(pos)) {
        picked.Add(pos);
        var monster = this.SpawnMonsterAt(pos);
        monster.GetComponent<LifeTimePublisher>().AfterDisabled =
          () => {
            this.RemoveMonsterFrom(monster, roomId);
          };
        this.monstersInRooms[roomId].Add(monster);
      }
    }
  }

  GameObject SpawnMonsterAt(Vector3 pos) 
  {
    var monster = Instantiate(this.monsterPrefab, pos, Quaternion.identity);  
    this.remain_monsters -= 1;
    monster.layer = LayerMask.NameToLayer("Monster");
    var lifeTimePublisher = monster.AddComponent<LifeTimePublisher>();
    return (monster);
  }

  void StoreMonsterPositions(int roomId) 
  {
    var monsters = this.monstersInRooms[roomId];
    this.storedMonsterPos[roomId] = new (monsters.Count);
    foreach (var monster in monsters) {
       this.storedMonsterPos[roomId].Add(monster.transform.position); 
    }
  }

  void RemoveMonsterFrom(GameObject monster, int roomId) {
    this.monstersInRooms[roomId].Remove(monster);
  }

  void RestoreMonstersInRoom(int roomId)
  {
    foreach (var pos in this.storedMonsterPos[roomId]) {
      var monster = this.ReSpawnMonsterAt(pos);
      this.monstersInRooms[roomId].Add(monster);
    }
  }

  GameObject ReSpawnMonsterAt(Vector3 pos)
  {
    var monster = Instantiate(this.monsterPrefab, pos, Quaternion.identity);  
    monster.layer = LayerMask.NameToLayer("Monster");
    var lifeTimePublisher = monster.AddComponent<LifeTimePublisher>();
    return (monster);
  }

  void UnSpawnMonsterAt(int roomId)
  {
    List<GameObject> monsters = new (this.monstersInRooms[roomId]);
    foreach (var monster in monsters) {
      this.RemoveMonsterFrom(monster, roomId);
      this.UnSpawnMonster(monster);            
    }
    this.monstersInRooms[roomId].Clear();
  }

  void UnSpawnMonster(GameObject monster)
  {
    // TODO: Return to pool
    Destroy(monster.gameObject);
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


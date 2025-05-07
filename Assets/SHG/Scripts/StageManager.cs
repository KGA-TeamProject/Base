using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : Singleton<StageManager> 
{

  public int CurrentStage => this.currentStage;
  public event Action OnStartStage;
  public event Action OnStageClear;
  public const int MAX_STAGE = 3;
  GameObject playerPrefab;
  [SerializeField]
  int currentStage = 1;
  MapManager map;
  StageConfig config;
  GameObject flagPrefab;
  StageFinisher finisher;
  int MAX_MOSNTERS = 40;
  int MAX_NUMBER_OF_MONSTERS_IN_ROOM = 10;
  int remain_monsters;
  Dictionary<int, HashSet<GameObject>> coinsInRooms;

  public void StartStage()
  {
    this.SetMonstersInPool(this.CurrentStage);
    this.ApplyStageConfig(this.CurrentStage);
    this.ChangeBgm();
    this.map.MAXIMUM_NODES = this.CurrentStage + 2;
    this.map.SpawnMap();
    this.remain_monsters = this.MAX_MOSNTERS;
    this.monstersInRooms = new ();
    this.storedMonsterPos = new ();
    this.coinsInRooms = new ();
  }

  void Awake()
  {
    this.LoadConfigs();
    this.map = new MapManager(this.CurrentStage + 2);
    this.map.OnStartingSpawned += this.OnStartingMapSpawned;
    this.map.OnDestinationSpawned += this.OnSpawnDestination;
    this.map.OnDestinationUnSpawned += this.OnUnSpawnDestination;
    this.map.OnRoomSpawned += this.OnRoomSpawned;
    this.map.OnRoomUnSpawned += this.OnRoomUnSpawned;
    this.SetCoins();
    this.SetPrefabs();
  }

  void SetPrefabs()
  {
    this.playerPrefab = Resources.Load<GameObject>("Prefabs/Player") ?? Resources.Load<GameObject>("TestPlayer");

    this.flagPrefab = Resources.Load<GameObject>("Prefabs/MapObjects/arrow_up");
  }

  void LoadConfigs() {
    var json = Resources.Load<TextAsset>("Configs/StageConfigs").text;
    this.config = JsonUtility.FromJson<StageConfig>(json);
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
      pos.y += 1;
      var flag = Instantiate(this.flagPrefab, pos, Quaternion.identity);
      flag.transform.localScale = new (2, 2, 2); 
      var minimapIcon = UIManager.Shared.combatUI.Minimap.AddMinimapIconTo(flag, UIManager.Shared.combatUI.Minimap.FlagIcon, 5);
      minimapIcon.name = "FlagMinimapIcon";
      this.finisher = flag.AddComponent<StageFinisher>();
      this.finisher.OnActivate += () => this.OnClear();
    }
    this.finisher.Show();
  }

  void OnUnSpawnDestination()
  {
    if (this.finisher != null) {
      this.finisher.Hide();
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
    lifeTimePublisher.AfterDestroyed = GameManager.Shared.OnPlayerDied;
    var light = Instantiate(Resources.Load<GameObject>("Prefabs/Player Light"));
    if (player.tag != "Player") {
      player.tag = "Player";
    }
    light.transform.parent = player.transform;
    light.transform.localPosition = new (0, 7, 0);
    UIManager.Shared.combatUI.Minimap.AddMinimapIconTo(player, UIManager.Shared.combatUI.Minimap.PlayerIcon, 2);
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
      this.UnSpawnMonstersIn(room);
      this.UnSpawnCoinsIn(room);
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
    if (this.currentStage < StageManager.MAX_STAGE) {
      AudioManager.Shared.PlayEffect(AudioManager.Shared.ClearEffect);
    }
    this.FinishStage();
    if (this.currentStage < StageManager.MAX_STAGE) {
      this.currentStage += 1;
      this.StartStage();
    }
  }

  void OnRoomSpawned(MapTypes.RoomType type, int roomId)
  {
    if (type == MapTypes.RoomType.Combat) {
      var positions = this.map.GetFreePositions(roomId);
      if (!this.monstersInRooms.ContainsKey(roomId)) {
        this.monstersInRooms.Add(roomId, new());
        this.coinsInRooms.Add(roomId, new());
        this.SpawnMonstersIn(roomId, positions);
      }
      else if (this.storedMonsterPos.ContainsKey(roomId)) {
        this.RestoreMonstersInRoom(roomId, this.CurrentStage);
        this.storedMonsterPos.Remove(roomId);
      }
    }
  }
  void OnRoomUnSpawned(MapTypes.RoomType type, int roomId) 
  {
    if (type == MapTypes.RoomType.Combat) {
      this.StoreMonsterPositions(roomId);
      this.UnSpawnCoinsIn(roomId);
      this.UnSpawnMonstersIn(roomId);
    }
  }

  void SpawnMonstersIn(int roomId, List<Vector3> positions)
  {
    var monsterCount = Math.Min(this.MAX_NUMBER_OF_MONSTERS_IN_ROOM, this.remain_monsters);
    HashSet<Vector3> picked = new(monsterCount);
    for (int i = 0; 
        picked.Count < monsterCount && i < positions.Count; ++i) {
      var pos = positions[this.monsterRand.Next(positions.Count)]; 
      if (!picked.Contains(pos)) {
        picked.Add(pos);
        var (monster, name) = this.SpawnMonsterAt(pos, this.CurrentStage);
        this.monstersInRooms[roomId].Add((monster, name));
        monster.GetComponent<LifeTimePublisher>().AfterDisabled =
          () => {
            if (this.monstersInRooms[roomId].Contains((monster, name))) {
              var coin = this.SpawnCoinAt(monster.transform.position, roomId);
              this.coinsInRooms[roomId].Add(coin.gameObject);
            }
            this.RemoveMonsterFrom(monster, name, roomId);
          };
      }
    }
  }

  Collectible SpawnCoinAt(Vector3 pos, int roomId)
  {
    var coin = PrefabObjectPool.Shared.GetPooledObject("coin").GetComponent<Collectible>();
    coin.transform.parent = this.transform;
    coin.transform.position = new (
        pos.x, pos.y + 1f, pos.z
        );
    coin.OnCollected = () => this.OnCoinCollectedIn(coin, roomId);
    coin.gameObject.SetActive(true);
    return (coin.GetComponent<Collectible>());
  }

  void OnCoinCollectedIn(Collectible coin, int roomId) 
  {
    this.coinsInRooms[roomId].Remove(coin.gameObject);
    PrefabObjectPool.Shared.ReturnObject(coin.gameObject, "coin");
    GameManager.Shared.CollectCoin();
  }


  void UnSpawnCoinsIn(int roomId)
  {
    GameObject[] coins = new GameObject[this.coinsInRooms[roomId].Count];
    this.coinsInRooms[roomId].CopyTo(coins);
    foreach (var coin in coins) {
      this.coinsInRooms[roomId].Remove(coin);
      PrefabObjectPool.Shared.ReturnObject(coin, "coin");
    }
    this.coinsInRooms[roomId].Clear();
  }

  void SetCoins()
  {
    PrefabObjectPool.Shared.RegisterByName(
        "coin", "Prefabs/MapObjects/coin", 
        (loaded) => {
        loaded.transform.localScale = new (0.7f, 0.7f, 0.7f);
        var trigger = loaded.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.center = new();
        trigger.includeLayers = LayerMask.NameToLayer("Player");
        trigger.radius = 3; 
        return (loaded);
        }, 
        50);
  }

  // TODO: Monster class
  #region Monster

  static List<string>[] MONSTER_PREFAB_NAMES = {
    new () {
      "BatPBR",
      "DragonPBR",
      "EvilMagePBR",
    },
    new (){
      "GolemPBR",
      "MonsterPlantPBR",
      "OrcPBR",
      "SkeletonPBR",
    },
    new () {
      "SlimePBR",
      "SpiderPBR",
      "TurtleShellPBR",
    }
  };
  Dictionary<int, HashSet<(GameObject, string)>> monstersInRooms;
  Dictionary<int, List<(Vector3, string)>> storedMonsterPos;
  private int nextMonsterId = 0;
  string monsterName = "DragonPBR";

  void ClearMonstersInPool() {}

  void SetMonstersInPool(int stage)
  {
    foreach (var monsterName in StageManager.MONSTER_PREFAB_NAMES[stage -1]) {
      var path = $"Prefabs/Monsters/{monsterName}";
      PrefabObjectPool.Shared.RegisterByName(
          monsterName,
          path,
          this.InitMonster,
          this.MAX_NUMBER_OF_MONSTERS_IN_ROOM
          );  
    } 
  }

  GameObject InitMonster(GameObject monster)
  {
    monster.AddComponent<LifeTimePublisher>();
    monster.layer = LayerMask.NameToLayer("Monster");
    if (monster.GetComponent<CapsuleCollider>() == null) {
      var collider = monster.AddComponent<CapsuleCollider>();
      collider.radius = 2f;
      collider.center = new (0, 1f, 0);
    }
    return (monster);
  }

  (GameObject, string) SpawnMonsterAt(Vector3 pos, int stage) 
  {
    var name = MONSTER_PREFAB_NAMES[stage - 1][
      monsterRand.Next(0, MONSTER_PREFAB_NAMES[stage - 1].Count)
    ];
    var monster = PrefabObjectPool.Shared.GetPooledObject(name);
    this.remain_monsters -= 1;
    monster.transform.position = pos;
    monster.SetActive(true);
    return (monster, name);
  }

  void StoreMonsterPositions(int roomId) 
  {
    var monsters = this.monstersInRooms[roomId];
    this.storedMonsterPos[roomId] = new (monsters.Count);
    foreach (var (monster, name) in monsters) {
      this.storedMonsterPos[roomId].Add((monster.transform.position, name)); 
    }
  }

  void RemoveMonsterFrom(GameObject monster, string name, int roomId) {
    this.monstersInRooms[roomId].Remove((monster, name));
  }

  void RestoreMonstersInRoom(int roomId, int stage)
  {
    foreach (var (pos, name) in this.storedMonsterPos[roomId]) {
      var monster = this.ReSpawnMonsterAt(pos, stage);
      this.monstersInRooms[roomId].Add((monster, name));
    }
  }

  GameObject ReSpawnMonsterAt(Vector3 pos, int stage)
  {

    var name = MONSTER_PREFAB_NAMES[stage - 1][
      monsterRand.Next(0, MONSTER_PREFAB_NAMES[stage - 1].Count)
    ];
    var monster = PrefabObjectPool.Shared.GetPooledObject(name);
    monster.transform.position = pos;  
    monster.SetActive(true);
    return (monster);
  }

  void UnSpawnMonstersIn(int roomId)
  {
    (GameObject, string)[] monsters = new (GameObject, string)[this.monstersInRooms[roomId].Count];
    this.monstersInRooms[roomId].CopyTo(monsters);
    foreach (var (monster, name) in monsters) {
      this.RemoveMonsterFrom(monster, name, roomId);
      this.UnSpawnMonster(monster, name);            
    }
    this.monstersInRooms[roomId].Clear();
  }

  void UnSpawnMonster(GameObject monster, string name)
  {
    PrefabObjectPool.Shared.ReturnObject(monster, name);
  }

  System.Random monsterRand = new ();

  #endregion
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


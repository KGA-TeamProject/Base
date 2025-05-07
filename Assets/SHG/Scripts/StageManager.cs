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
  Dictionary<int, HashSet<GameObject>> coinsInRooms;
  System.Random monsterRand = new ();

  private int nextMonsterId = 0;

  public void StartStage()
  {
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
    this.playerPrefab = Resources.Load<GameObject>("Prefabs/Player") ?? Resources.Load<GameObject>("TestPlayer");
    this.monsterPrefab = Resources.Load<GameObject>("Prefabs/TestMonster");

    this.flagPrefab = Resources.Load<GameObject>("Prefabs/MapObjects/arrow_up");
    this.map.OnStartingSpawned += this.OnStartingMapSpawned;
    this.map.OnDestinationSpawned += this.OnSpawnDestination;
    this.map.OnDestinationUnSpawned += this.OnUnSpawnDestination;
    this.map.OnRoomSpawned += this.OnRoomSpawned;
    this.map.OnRoomUnSpawned += this.OnRoomUnSpawned;
    this.SetCoins();
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
        this.RestoreMonstersInRoom(roomId);
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
        var monster = this.SpawnMonsterAt(pos);
        this.monstersInRooms[roomId].Add(monster);
        monster.GetComponent<LifeTimePublisher>().AfterDisabled =
          () => {
            if (this.monstersInRooms[roomId].Contains(monster)) {
              var coin = this.SpawnCoinAt(monster.transform.position, roomId);
              this.coinsInRooms[roomId].Add(coin.gameObject);
            }
            this.RemoveMonsterFrom(monster, roomId);
          };
        this.monstersInRooms[roomId].Add(monster);
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

  void UnSpawnMonstersIn(int roomId)
  {
    GameObject[] monsters = new GameObject[this.monstersInRooms[roomId].Count];
    this.monstersInRooms[roomId].CopyTo(monsters);
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


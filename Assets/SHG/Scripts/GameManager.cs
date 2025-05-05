using System;
using System.Collections.Generic;
using UnityEngine;

// 임시 플레이어 interface
public interface IPlayer
{
  public event Action OnDie; 
}

public class GameManager: Singleton<GameManager> 
{
  public enum GameState 
  {
    InCombat,
    SelectItem
  }

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
  new public static void CreateInstance() 
  {
    if (Debugging.Mode == Debugging.DebugMode.DeactivateAll) {
      return ;
    }
    Singleton<GameManager>.CreateInstance();
    PrefabObjectPool.CreateInstance();
    UIManager.CreateInstance();
    StageManager.CreateInstance();
    GameSceneManager.CreateInstance();
  }

  new public static GameManager Shared => Singleton<GameManager>.Shared;

  public GameState State { get; private set; }

  /// <summary> 게임 일시 정지일 때 false </summary>
  public bool IsPlaying => this.isPlaying;
  public event Action OnGameStart;
  public event Action OnGamePaused;
  public event Action OnGameResumed;

  [SerializeField]
  bool isPlaying;

  /// <summary> player에는 OnDie, OnLevelUp등의 이벤트가 존재해야 함 </summary>
  public void SetPlayerCharacter(IPlayer player) 
  {
    player.OnDie += this.OnGameOver; 
  }

  void Start() {
    this.Init();
    this.State = GameState.InCombat;
  }

  void Init() 
  {
    if (Debugging.Mode == Debugging.DebugMode.None) {
      GameSceneManager.Shared.OnSceneLoaded += this.OnGameSceneLoaded;
      GameSceneManager.Shared.StartLoadScene(GameSceneManager.SceneName.CombatScene);
    }
  }

  public void OnGameSceneLoaded(GameSceneManager.SceneName sceneName)
  {
    this.isPlaying = true;
    switch (sceneName)
    {
      case GameSceneManager.SceneName.CombatScene:
        StageManager.Shared.OnStartStage += UIManager.Shared.ShowCombatUI;
        StageManager.Shared.StartStage();  
        break;
      default:
        break;
    }
  }

  public void EndGame()
  {
    Debug.Log("EndGame");
  }

  /*
   * event
   */
  void OnStart() 
  {
    if (this.OnGameStart != null) {
      this.OnGameStart.Invoke();
    }
  }

  void OnPaused() 
  {
    if (this.OnGamePaused != null) {
      this.OnGamePaused.Invoke();
    }
  }

  void OnResumed() 
  {
    if (this.OnGameResumed != null) {
      this.OnGameResumed.Invoke();
    }
  }

  void OnGameOver() 
  {
  }
}

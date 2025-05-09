using System;
using System.Collections.Generic;
using UnityEngine;

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
    AudioManager.CreateInstance();
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
  public int CollectedCoins { get; private set; }

  void Start() {
    this.Init();
    this.State = GameState.InCombat;
  }

  void Init() 
  {
    this.CollectedCoins = 0;
    if (Debugging.Mode == Debugging.DebugMode.None) {
      GameSceneManager.Shared.OnSceneLoaded += this.OnGameSceneLoaded;
    }
  }

  public void OnGameSceneLoaded(GameSceneManager.SceneName sceneName)
  {
    this.isPlaying = true;
    switch (sceneName)
    {
      case GameSceneManager.SceneName.CombatScene:
        StageManager.Shared.OnStartStage += () => {
          this.isPlaying = true; 
          this.State = GameState.InCombat;
          UIManager.Shared.loadingUI.Hide();
          UIManager.Shared.combatUI.Show();
        };
        StageManager.Shared.OnStageClear += () => {
          this.isPlaying = false;
          UIManager.Shared.combatUI.Hide();
          if (StageManager.Shared.CurrentStage < StageManager.MAX_STAGE) {
            UIManager.Shared.loadingUI.Show();
          }
          else {
            GameSceneManager.Shared.StartLoadScene(GameSceneManager.SceneName.EndingScene);
          }
        };
        StageManager.Shared.StartStage();  
        break;
      default:
        break;
    }
  }

  public void CollectCoin()
  {
    this.CollectedCoins += 1;
  }

  public void EndGame()
  {
    this.isPlaying = false;
    #if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
    #else
    Application.Quit();
    #endif
  }

  public void OnPlayerDied()
  {
    if (this.isPlaying) {
      GameSceneManager.Shared.StartLoadScene(GameSceneManager.SceneName.EndingScene);
    }
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

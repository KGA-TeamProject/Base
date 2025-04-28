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
    UIManager.CreateInstance();
    StageManager.CreateInstance();
    SceneManager.CreateInstance();
  }

  new public GameManager Shared => Singleton<GameManager>.Shared;

  public GameState State { get; private set; }
  /// <summary> 게임 일시 정지일 때 false </summary>
  public bool IsPlaying { get; }
  public event Action OnGameStart;
  public event Action OnGamePaused;
  public event Action OnGameResumed;

  /// <summary> player에는 OnDie, OnLevelUp등의 이벤트가 존재해야 함 </summary>
  public void SetPlayerCharacter(IPlayer player) 
  {
    player.OnDie += this.OnGameOver; 
  }


  void Init() 
  {
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

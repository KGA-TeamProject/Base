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

  private int nextMonsterId = 0;

  void Awake()
  {
    this.map = new MapGenerator();
  }

  void Start()
  {
    UIManager.Shared.MinimapCamera = this.map.minimapCamera;
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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : Singleton<UIManager>
{

  public PopupUI? CurrentPopup { get; private set; }
  public CombatUI combatUI;
  public Camera MinimapCamera { set {
    if (this.combatUI.minimap == null) {
      return ;
    }
    this.combatUI.minimap.Camera = value;
    this.combatUI.ZoomMinimap();
  }}

  public enum PopupUI
  {
    ItemSelect,
    LevelUp,
    StageEnd
  }
  GameObject combatUIPrefab;

  void Awake()
  {
    this.Init();
  }

  void Init()
  {
    this.combatUIPrefab = ((GameObject)Resources.Load("Prefabs/" + CombatUI.PREFAB_NAME));
    this.combatUI = Instantiate(this.combatUIPrefab).GetComponent<CombatUI>();
    this.combatUI.transform.parent = this.gameObject.transform;
    this.combatUI.Hide();
  }

  public void ShowCombatUI()
  {
    this.combatUI.Show();
  }
}

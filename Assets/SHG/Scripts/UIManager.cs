using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : Singleton<UIManager>
{

  public PopupUI? CurrentPopup { get; private set; }
  public CombatUI combatUI;
  public LoadingUI loadingUI;
  public Vector2 JoystickInput => this.combatUI.Joystick.Input;
  public Camera MinimapCamera { set {
    if (this.combatUI.Minimap == null) {
      return ;
    }
    this.combatUI.Minimap.Camera = value;
    this.combatUI.ZoomMinimap();
  }}

  public enum PopupUI
  {
    ItemSelect,
    LevelUp,
    StageEnd
  }
  GameObject combatUIPrefab;
  GameObject loadingUIPrefab;

  void Awake()
  {
    this.Init();
  }

  void Init()
  {
    this.loadingUIPrefab = ((GameObject)Resources.Load("Prefabs/" + LoadingUI.PREFAB_NAME));
    this.loadingUI = Instantiate(this.loadingUIPrefab).GetComponent<LoadingUI>();
    this.loadingUI.transform.parent = this.transform;
    this.loadingUI.Hide();
    this.combatUIPrefab = ((GameObject)Resources.Load("Prefabs/" + CombatUI.PREFAB_NAME));
    this.combatUI = Instantiate(this.combatUIPrefab).GetComponent<CombatUI>();
    this.combatUI.transform.parent = this.transform;
    this.combatUI.Hide();
  }
}

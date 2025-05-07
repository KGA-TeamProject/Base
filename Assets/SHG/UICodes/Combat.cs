using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatUI : MonoBehaviour 
{
  public const string PREFAB_NAME = "CombatUI";
  public bool IsShowing { get; private set; }
  public Transform Player;
  public Minimap Minimap { get; private set; }
  public StatusView Status { get; private set; }
  public Joystick Joystick { get; private set; }
  const string CONTAINER_NAME = "combatUI_container";
  VisualElement root;
  Coroutine zoomMinimapRoutine;

  public void Show() 
  { 
    this.root.visible = true;
    this.IsShowing = true;
    this.Player = GameObject.FindWithTag("Player").transform;
  }

  public void Hide() 
  { 
    this.root.visible = false;
    this.IsShowing = false;
  }

  void Awake()
  {
    this.Init();
  }

  // Update is called once per frame
  void Update()
  {
    if (!this.IsShowing) {
      return ;
    }
    if (GameManager.Shared.State == GameManager.GameState.InCombat &&
        GameManager.Shared.IsPlaying) {
      this.UpdateMinimap();
      this.Status.CoinCount = GameManager.Shared.CollectedCoins;
    }
  }

  void Init()
  {
    this.root = this.GetComponent<UIDocument>().rootVisualElement;
    this.root.AddToClassList(CombatUI.CONTAINER_NAME);
    this.Minimap = new ();
    this.Status = new ();
    this.Joystick = new ();
    this.root.Add(this.Minimap);
    this.root.Add(this.Status);
    this.root.Add(this.Joystick);
  }

  public void ZoomMinimap()
  {
    if (this.zoomMinimapRoutine != null) {
      this.StopCoroutine(this.zoomMinimapRoutine);
    }
    this.zoomMinimapRoutine = this.StartCoroutine(
        this.Minimap.Zoom(Minimap.DEFAULT_ZOOM, () => this.zoomMinimapRoutine = null)
        );
  }

  void UpdateMinimap()
  {
    var newPos = new Vector3(
        this.Player.position.x,
        this.Minimap.Camera.transform.position.y,
        this.Player.position.z
        );
    this.Minimap.MoveCameraCenterTo(newPos);
  }
}


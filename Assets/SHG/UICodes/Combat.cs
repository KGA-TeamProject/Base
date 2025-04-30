using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatUI : MonoBehaviour 
{
  public bool IsShowing { get; private set; }
  IUIComponent CharacterHpUI;
  IUIComponent SkillListUI;
  const string CONTAINER_NAME = "combatUI_container";
  Minimap minimap;
  VisualElement root;
  Transform playerTransform;
  Coroutine zoomMinimapRoutine;

  void Awake()
  {
    this.Init();
  }
  // Start is called before the first frame update
  void Start()
  {
    this.playerTransform = GameObject.FindWithTag("Player").transform;
    this.Show();
    this.ZoomMinimap();
  }

  // Update is called once per frame
  void Update()
  {
    if (GameManager.Shared.State == GameManager.GameState.InCombat &&
        GameManager.Shared.IsPlaying) {
      this.UpdateMinimap();
    }
  }

  void Init()
  {
    this.minimap = new();
    this.minimap.Camera = GameObject.Find("Minimap Camera").GetComponent<Camera>();
    this.root = this.GetComponent<UIDocument>().rootVisualElement;
    this.root.AddToClassList(CombatUI.CONTAINER_NAME);
    this.root.Add(this.minimap);
  }

  void Show() 
  { 
    this.root.visible = true;
    this.IsShowing = true;
  }

  void Hide() 
  { 
    this.root.visible = false;
    this.IsShowing = false;
  }

  void ZoomMinimap()
  {
    if (this.zoomMinimapRoutine != null) {
      this.StopCoroutine(this.zoomMinimapRoutine);
    }
    this.zoomMinimapRoutine = this.StartCoroutine(
        this.minimap.Zoom(10f, () => this.zoomMinimapRoutine = null)
        );
  }

  void UpdateMinimap()
  {
    var newPos = new Vector3(
        this.playerTransform.position.x,
        this.minimap.Camera.transform.position.y,
        this.playerTransform.position.z
        );
    this.minimap.MoveCameraCenterTo(newPos);
  }
}

public interface IUIComponent 
{
}

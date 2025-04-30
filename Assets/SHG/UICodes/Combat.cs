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
  Camera minimapCamera;
  Transform playerTransform;

  void Awake()
  {
    this.Init();
    this.minimapCamera = GameObject.Find("Minimap Camera").GetComponent<Camera>();
  }
  // Start is called before the first frame update
  void Start()
  {
    this.playerTransform = GameObject.FindWithTag("Player").transform;
    this.Show();
  }

  // Update is called once per frame
  void Update()
  {
    if (GameManager.Shared.State == GameManager.GameState.InCombat &&
        GameManager.Shared.IsPlaying) {
      this.MinimapCameraFollowPlayer();
    }
  }

  void Init()
  {
    this.minimap = new();
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

  void MinimapCameraFollowPlayer()
  {
    var newPos = new Vector3(
        this.playerTransform.position.x,
        this.minimapCamera.transform.position.y,
        this.playerTransform.position.z
        );
    this.minimapCamera.transform.position = newPos;
    this.minimapCamera.transform.LookAt(this.playerTransform.transform.position);
  }

}

public interface IUIComponent 
{
}

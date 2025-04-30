using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatUI : MonoBehaviour 
{
  public bool IsShowing { get; private set; }
  IUIComponent CharacterHpUI;
  IUIComponent SkillListUI;
  Minimap minimap;
  const string CONTAINER_NAME = "combatUI_container";
  VisualElement root;

  void Awake()
  {
    this.Init();
  }
  // Start is called before the first frame update
  void Start()
  {
    this.Show();
  }

  // Update is called once per frame
  void Update()
  {

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

}

public interface IUIComponent 
{
}

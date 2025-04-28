using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUI : MonoBehaviour {

  UIComponent CharacterHpUI;
  UIComponent SkillListUI;
  
  public bool IsShowing { get; private set; }

  void Init() 
  { 
  }

  void Show() 
  { 
    this.IsShowing = true;
  }

  void Hide() 
  { 
    this.IsShowing = false;
  }

}

public interface UIComponent 
{
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : Singleton<UIManager>
{

  public PopupUI? CurrentPopup { get; private set; }

  public enum PopupUI
  {
    ItemSelect,
    LevelUp,
    StageEnd
  }

  public void ShowCombatUI()
  {
    var uiObject = GameObject.Find("CombatUI");
  }
}

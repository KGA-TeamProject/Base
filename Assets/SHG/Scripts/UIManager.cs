using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{

  public PopupUI? CurrentPopup { get; private set; }

  public enum PopupUI
  {
    ItemSelect,
    LevelUp,
    StageEnd
  }
}
